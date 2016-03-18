using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

using System.Threading.Tasks;

using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyRetailKioskApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly EmotionServiceClient emotionServiceClient = new EmotionServiceClient("{Emotion API Primary Key}");

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task<Emotion[]> UploadAndDetectEmotions(StorageFile imageFile)
        {
            using (var imageFileStream = await imageFile.OpenStreamForReadAsync())
            {
                // Calls the Emotion API to detect the emotions in the image                
                var emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);
                return emotionResult;
            }
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // The supported input image formats includes JPEG, PNG, GIF(the first frame), BMP.Image file size should be no larger than 4MB
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            
            var storageFile = await filePicker.PickSingleFileAsync();
            var stream = await storageFile.OpenAsync(FileAccessMode.Read);

            var bitmapSource = new BitmapImage();
            await bitmapSource.SetSourceAsync(stream);

            FacePhoto.Source = bitmapSource;

            // Emotion API call
            var emotionText = "Face [{0}]{1} Anger:{2}{1} Contempt:{3}{1} Disgust:{4}{1} Fear:{5}{1} Happiness:{6}{1} Neutral:{7}{1} Sadness:{8}{1} Surprise:{9}{1}";

            var emotionResult = await UploadAndDetectEmotions(storageFile);

            for (int i = 0; i < emotionResult.Length; i++)
            {
                var scores = emotionResult[i].Scores;
                var textBlock = new TextBlock() { TextWrapping = TextWrapping.WrapWholeWords };
                textBlock.Text = string.Format(emotionText, i, Environment.NewLine, scores.Anger, scores.Contempt, scores.Disgust, scores.Fear, scores.Happiness, scores.Neutral, scores.Sadness, scores.Surprise);                
                EmotionList.Items.Add(textBlock);
            }
        }
    }
}
