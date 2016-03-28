$maxRows = 1000
$minRows = 400
$daysCount = 4
$filesPerDay = Get-Random -minimum 1 -maximum 5

$prods = @(
	@{ title="Halogen Headlights (2 Pack)"; cat="Lighting"; },
	@{ title="Bugeye Headlights (2 Pack)"; cat="Lighting"; },
	@{ title="Turn Signal Light Bulb"; cat="Lighting"; },
	@{ title="Matte Finish Rim"; cat="Wheels & Tires"; },
	@{ title="Blue Performance Alloy Rim"; cat="Wheels & Tires"; },
	@{ title="High Performance Rim"; cat="Wheels & Tires"; },
	@{ title="Wheel Tire Combo"; cat="Wheels & Tires"; },
	@{ title="Chrome Rim Tire Combo"; cat="Wheels & Tires"; },
	@{ title="Wheel Tire Combo (4 Pack)"; cat="Wheels & Tires"; },
	@{ title="Disk and Pad Combo"; cat="Wheels & Tires"; },
	@{ title="Brake Rotor"; cat="Brakes"; },
	@{ title="Brake Disk and Calipers"; cat="Brakes"; },
	@{ title="12-Volt Calcium Battery"; cat="Batteries"; },
	@{ title="Spiral Coil Battery"; cat="Batteries"; },
	@{ title="Jumper Leads"; cat="Batteries"; },
	@{ title="Filter Set"; cat="Oil"; },
	@{ title="Oil and Filter Combo"; cat="Oil"; },
	@{ title="Synthetic Engine Oil"; cat="Oil"; }
)

for($d = 0; $d -lt $daysCount; $d++) {
	$folder = "Assets\logs\$(Get-Date (Get-Date).AddDays(-$d) -f yyyy\\MM\\dd)";
	Write-Host "Creating folder $folder..."
	New-Item -ItemType directory -Path $folder -Force
	
	for($i = 1; $i -le $filesPerDay; $i++) {
		$path = "$folder\data$i.txt";

		$items = ""
		$totalRows = Get-Random -minimum $minRows -maximum $maxRows

		for($j = 1; $j -le $totalRows; $j++)
		{
			$p = Get-Random -minimum 0 -maximum $prods.length
			$items += "{""productid"":""$($p+1)"",""title"":""$($prods[$p].title)"",""category"":""$($prods[$p].cat)"",""type"":""$(("add", "view") | Get-Random)"",""total"":""$(Get-Random -minimum 0 -maximum 200)""}`n"
		}
		
		$items = $items.Substring(0, $items.length - 1) 
		
		Write-Host "Saving data into $path..."
		[System.IO.File]::WriteAllText($path, $items)
	}
}