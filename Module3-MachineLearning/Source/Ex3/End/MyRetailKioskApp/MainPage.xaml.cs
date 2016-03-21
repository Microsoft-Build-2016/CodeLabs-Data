using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading.Tasks;

using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyRetailKioskApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string EmotionApiKey = "b097491b178844a0a985fd7f5b5bc728";
        private MediaCapture mediaCapture;
        private bool isCameraFound;
        
        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeMediaCapture();
        }

        private async void InitializeMediaCapture()
        {
            try
            {
                this.mediaCapture = new MediaCapture();
                var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                // Use the front camera if found one 
                if (devices == null || devices.Count == 0)
                {
                    this.isCameraFound = false;
                    return;
                }

                MediaCaptureInitializationSettings settings;
                settings = new MediaCaptureInitializationSettings { VideoDeviceId = devices[0].Id }; // 0 => front, 1 => back 
                settings.StreamingCaptureMode = StreamingCaptureMode.Video;

                await this.mediaCapture.InitializeAsync(settings);
                VideoEncodingProperties resolutionMax = null;
                int max = 0;
                var resolutions = this.mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo);

                for (var i = 0; i < resolutions.Count; i++)
                {
                    var res = (VideoEncodingProperties)resolutions[i];
                    if (res.Width * res.Height > max)
                    {
                        max = (int)(res.Width * res.Height);
                        resolutionMax = res;
                    }
                }

                await this.mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, resolutionMax);
                this.capturePreview.Source = this.mediaCapture;
                this.isCameraFound = true;
                await this.mediaCapture.StartPreviewAsync();
            }
            catch (Exception ex)
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Error while initializing media capture device: " + ex.Message);
                await dialog.ShowAsync();
                GC.Collect();
            }
        }

        private async Task<Emotion[]> UploadAndDetectEmotions(StorageFile imageFile)
        {
            using (var imageFileStream = await imageFile.OpenStreamForReadAsync())
            {
                // Calls the Emotion API to detect the emotions in the image
                var emotionServiceClient = new EmotionServiceClient(EmotionApiKey);
                var emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);

                return emotionResult;
            }
        }

        private async void TakePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (!isCameraFound)
            {
                return;
            }

            try
            {   
                using (var imageStream = new InMemoryRandomAccessStream())
                {
                    // capture photo and encode it
                    var encodingProperties = ImageEncodingProperties.CreateJpeg();
                    await this.mediaCapture.CapturePhotoToStreamAsync(encodingProperties, imageStream);
                    await imageStream.FlushAsync();
                    imageStream.Seek(0);

                    // display photo preview
                    var img = new BitmapImage();
                    img.SetSource(imageStream);
                    this.TakenPhoto.Source = img;

                    imageStream.Seek(0);

                    // call emotion API
                    var emotionServiceClient = new EmotionServiceClient(EmotionApiKey);
                    var emotionResult = await emotionServiceClient.RecognizeAsync(imageStream.AsStreamForRead());
                    var emotionText = "Face [{0}]{1} Anger: {2:P2}{1} Contempt: {3:P2}{1} Disgust: {4:P2}{1} Fear: {5:P2}{1} Happiness: {6:P2}{1} Neutral: {7:P2}{1} Sadness: {8:P2}{1} Surprise: {9:P2}{1}";
                    
                    // display emotion results
                    this.EmotionList.Items.Clear();
                    for (int i = 0; i < emotionResult.Length; i++)
                    {
                        var scores = emotionResult[i].Scores;
                        var textBlock = new TextBlock() { TextWrapping = TextWrapping.WrapWholeWords };
                        textBlock.Text = string.Format(emotionText, i, Environment.NewLine, scores.Anger, scores.Contempt, scores.Disgust, scores.Fear, scores.Happiness, scores.Neutral, scores.Sadness, scores.Surprise);
                        this.EmotionList.Items.Add(textBlock);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Error while taking photo: " + ex.Message);
                await dialog.ShowAsync();
                GC.Collect();
            }
        }
    }
}
