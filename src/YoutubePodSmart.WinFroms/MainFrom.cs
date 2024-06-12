using YoutubePodSmart.Audio;
using YoutubePodSmart.Video;

namespace YoutubePodSmart.WinForms;

public partial class MainFrom : Form
{
    private const string Folder = "D:\\AppFolder";
    private string _videoFileName = null!;

    public MainFrom()
    {
        InitializeComponent();
    }

    private async void GetVideoButton_Click(object sender, EventArgs e)
    {
        var videoFileUrl = videoSourceTextBox.Text;

        if (string.IsNullOrEmpty(videoFileUrl))
        {
            statusLabel.Text = @"Please provide a link to the video!";
            return;
        }

        progressBar.Value = new Random().Next(5,17);
        progressBar.Maximum = 100;

        var videoFilePath = Path.Combine(Folder);

        try
        {
            statusLabel.Text = @"Getting ready...";
            var progressHandler = new Progress<double>(val =>
            {
                progressBar.Value = (int)(val * 100);
                statusLabel.Text = $@"Downloading video... {progressBar.Value}%";

                if(progressBar.Value == 100)
                    statusLabel.Text = @"Video downloaded";
            });

            _videoFileName = await new Download().GetYoutubeVideo(videoFileUrl, videoFilePath, progressHandler);
        }
        catch (Exception ex)
        {
            statusLabel.Text = $@"Error: {ex.Message}";
            progressBar.Value = 0;
        }
    }

    private void GetAudioButton_Click(object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(_videoFileName))
        {
            statusLabel.Text = @"Please download a video first!";
            return;
        }

        progressBar.Value = new Random().Next(25, 37);
        progressBar.Maximum = 100;

        try
        {
            statusLabel.Text = @"Analyzing source file...";
            var audioFileName = Path.ChangeExtension(_videoFileName, ".mp3");

            var progressHandler = new Progress<double>(val =>
            {
                progressBar.Value = (int)(val * 100);
                statusLabel.Text = $@"Extracting audio... {progressBar.Value}%";

                if (progressBar.Value == 100)
                    statusLabel.Text = @"Audio extracted";
            });

            new Extract().GetAudioFromVideo(_videoFileName, audioFileName, progressHandler);
        }
        catch (Exception ex)
        {
            statusLabel.Text = $@"Error: {ex.Message}";
            progressBar.Value = 0;
        }
    }

    private void label1_Click(object sender, EventArgs e)
    {
    }
}
