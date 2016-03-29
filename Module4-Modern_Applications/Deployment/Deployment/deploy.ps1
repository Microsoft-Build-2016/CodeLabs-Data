param (
    [Parameter(Mandatory = $true, HelpMessage = 'The DemoName will be used to construct URLs and names for deployed Azure components. Should be globally unique.')]
    [string] $DemoName,

	[Parameter(Mandatory = $false, HelpMessage = 'Package Url to deploy web site')]
    [string] $PackageUrl = "http://tjliveaw01012030.blob.core.windows.net/deployments/MSCorp.AdventureWorks.Web.zip",

	[Parameter(Mandatory = $false, HelpMessage = 'Web.Config to update')]
    [string] $WebConfigPath = "C:\Users\Public\Desktop\Website\MSCorp.AdventureWorks.Web\Web.config",
	
	[Parameter(Mandatory = $false, HelpMessage = 'The location the resource group will be created in if it doesnt already exist.')]
    [string] $ResourceGroupLocation = "West US"
)

#------------------#
# Load Environment #
#------------------#

Write-Host 'Preparing environment'

$script_dir = Split-Path $MyInvocation.MyCommand.Path

#Import utilities
. "$script_dir\deploy_utils.ps1"

#Import-AzureRM 
Import-Module AzureRM.Profile
Import-Module AzureRM.Resources
Import-Module Azure

#------------------------------#
# Ensure the user is signed in #
#------------------------------#

Write-Host 'Verifying authentication'

$rm_context = 
    try {
        Get-AzureRmContext
    } 
    catch {
        if ($_.Exception -and $_.Exception.Message.Contains('Login-AzureRmAccount')) { $null } else { throw }
    }

if (-not $rm_context) {
    
    $title = 'You must sign in with your Azure account to continue'
    $message = 'Sign in?'
    
    if ((Confirm-Host -Title $title -Message $message) -eq 1) {
        # User declined
        return
    }
    
    $rm_context = Add-AzureRmAccount
}

if (-not $rm_context) {
    Write-Warning 'Unable to sign in?'
    return
}

#-----------------------#
# Select a subscription #
#-----------------------#

Write-Host 'Selecting subscription'

$azure_subscriptions = Get-AzureRmSubscription
$rm_subscription_id = $null;

if ($azure_subscriptions.Count -eq 1) {
    $rm_subscription_id = $azure_subscriptions[0].SubscriptionId
    Write-Host 'Selected single ' $rm_subscription_id
} elseif ($azure_subscriptions.Count -gt 0) {
    # Build an array of bare subscription IDs for lookups
    $subscription_ids = $azure_subscriptions | % { $_.SubscriptionId }
        
    Write-Host 'Available subscriptions:'
    $azure_subscriptions | Format-Table SubscriptionId,SubscriptionName -AutoSize
    
    # Loop until the user selects a valid subscription Id
    while (-not $rm_subscription_id -or -not $subscription_ids -contains $rm_subscription_id) {
        $rm_subscription_id = Read-Host 'Please select a valid SubscriptionId from list'
    }

    Write-Host 'Selected subscription ' $rm_subscription_id
}

if (-not $rm_subscription_id) {
    Write-Warning 'No subscription available'
    return
}

Select-AzureRmSubscription -SubscriptionId $rm_subscription_id | out-null

$rm_resource_group_name = $DemoName

#---------------------------------#
# Start resource group deployment #
#---------------------------------#

Write-Host "Acquiring resource group ($rm_resource_group_name)"

$rm_resource_group = try { Get-AzureRmResourceGroup -Name $rm_resource_group_name -Verbose }
                     catch { if ($_.Exception.Message -eq 'Provided resource group does not exist.') { $null } else { throw } }

if (-not $rm_resource_group) {
    Write-Host "Creating resource group $rm_resource_group_name..."
    $rm_resource_group = New-AzureRmResourceGroup  -Name $rm_resource_group_name -Location $ResourceGroupLocation -Verbose
}

if (-not $rm_resource_group) {    
    Write-Warning 'No resource group!'
    return
}


Write-Host 'Starting resource group deployment'

$template = "$(Split-Path $MyInvocation.MyCommand.Path)\deployment.json"

$params = @{
    'demo_name' = $DemoName;
	'package_url' = $PackageUrl;
}

$result = New-AzureRmResourceGroupDeployment -ResourceGroupName $rm_resource_group_name -TemplateFile $template -TemplateParameterObject $params -Verbose

if (-not $result -or $result.ProvisioningState -ne 'Succeeded') {
    
    Write-Warning 'An error occured during provisioning'
    Write-Output $result
    return
}

$OUTPUTS = @{}

foreach ($name in $result.Outputs.Keys) {
    $OUTPUTS[$name] = $result.Outputs[$name].Value
}

if ($WebConfigPath -ne '') {
	if (Test-Path $WebConfigPath){ 
		foreach ($name in $result.Outputs.Keys) {
			$placeHolder = '[' + $name + ']'
			(Get-Content $WebConfigPath).replace($placeHolder, $result.Outputs[$name].Value) | Set-Content -Path $WebConfigPath
		}
	}
}

#----------------#
# Setting up Demo#
#----------------#

Write-Host 'Populating site data'
$resttest = ''
$demo_setup_url = "http://" + $OUTPUTS["WebsiteUrl"] + "/DemoSetup/SimpleSetup"

while ($resttest -eq ''){

    Start-Sleep -s 10
    
    try{
        
        Write-Host "Setting up demo data " + $demo_setup_url
    
        $resttest = Invoke-RestMethod -TimeoutSec 10000 -Uri $demo_setup_url
    }catch{
        Write-Host "Setting up demo data failed trying again"
        $resttest = ''
    }
}

#----------------#
# Setting up sync#
#----------------#
Write-Host 'Syncing reviews into search index'
$demo_sync_url = "http://" + $OUTPUTS["WebsiteUrl"] + "/Admin/sync"
Invoke-RestMethod -TimeoutSec 10000 -Uri $demo_sync_url

#----------------#
# Report results #
#----------------#

foreach ($name in $result.Outputs.Keys) {
    $keyAndValue = $name + " : " +  $result.Outputs[$name].Value
    Write-Host $keyAndValue
}

Write-Host "Deployment successful."