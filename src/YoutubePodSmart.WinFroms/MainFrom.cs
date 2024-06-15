using Microsoft.Extensions.Configuration;
using YoutubePodSmart.Audio;
using YoutubePodSmart.OpenAi;
using YoutubePodSmart.OpenAi.Contracts;
using YoutubePodSmart.Video;
using YoutubePodSmart.WinForms.SettingsModels;

namespace YoutubePodSmart.WinForms;

public partial class MainForm : Form
{
    private const string Folder = "D:\\AppFolder";
    private string _videoFileName;
    private string _audioFileName;
    private string _textFileName;
    private string _summaryFileName;

    private readonly IConfiguration _configuration;
    private readonly IAiService _aiService;

    private readonly PromptSettings _prompts;

    public MainForm(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        var apiKey = _configuration["OpenAiSettings:ApiKey"] ?? throw new ArgumentNullException("ApiKey", "ApiKey not found in the configuration!");

        var models = new AiModelSettings();
        _configuration.GetSection("AiModelSettings").Bind(models);

        _prompts = new PromptSettings();
        _configuration.GetSection("PromptSettings").Bind(_prompts);

        _aiService = new OpenAiService(apiKey, models.AudioModel, models.CompletionModel);

        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

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

        statusLabel.Text = @"Getting ready...";
        progressBar.Value = new Random().Next(5, 17);

        var videoFilePath = Path.Combine(Folder);

        try
        {
            var youtube = new Youtube(videoFileUrl);

            _videoFileName = await youtube.GetVideoFileName(videoFilePath);

            if (File.Exists(_videoFileName))
            {
                statusLabel.Text = @"Video already downloaded";
                progressBar.Value = 100;
                return;
            }

            var progressHandler = new Progress<double>(val =>
            {
                progressBar.Value = (int)(val * 100);
                statusLabel.Text = $@"Downloading video... {progressBar.Value}%";

                if (progressBar.Value == 100)
                    statusLabel.Text = @"Video downloaded";
            });

            await youtube.GetYoutubeVideo(_videoFileName, progressHandler);
        }
        catch (Exception ex)
        {
            statusLabel.Text = $@"Error: {ex.Message}";
            progressBar.Value = 0;
        }
    }

    private async void GetAudioButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_videoFileName))
        {
            statusLabel.Text = @"Please download a video first!";
            return;
        }

        statusLabel.Text = @"Analyzing source file...";
        progressBar.Value = new Random().Next(15, 37);

        _audioFileName = Path.ChangeExtension(_videoFileName, ".mp3");

        if (File.Exists(_audioFileName))
        {
            statusLabel.Text = @"Audio already extracted";
            progressBar.Value = 100;
            return;
        }

        try
        {
            progressTimer.Start();
            statusLabel.Text = @"Extracting audio...";

            await Task.Run(() => new Extract().GetAudioFromVideo(_videoFileName, _audioFileName));

            progressBar.Value = 100;
            statusLabel.Text = @"Audio extracted";
        }
        catch (Exception ex)
        {
            statusLabel.Text = $@"Error: {ex.Message}";
            progressBar.Value = 0;
        }
    }

    private async void TranscribeButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_audioFileName))
        {
            statusLabel.Text = @"Please extract audio first!";
            return;
        }

        _textFileName = Path.ChangeExtension(_audioFileName, ".txt");
        transcriptTextBox.Text = string.Empty;
        progressBar.Value = 3;

        try
        {
            if (!File.Exists(_textFileName))
            {
                statusLabel.Text = @"Transcribing audio...";
                progressBar.Value = new Random().Next(8, 21);
                progressTimer.Start();

                var transcription = await _aiService.TranscribeAudioAsync(_audioFileName);

                statusLabel.Text = @"Normalizing transcription...";
                transcription = await _aiService.GetCompletionForPromptAsync(transcription, _prompts.PromptForAudioTranscriptionTextNormalization);

                await File.WriteAllTextAsync(_textFileName, transcription);
            }

            var text = await File.ReadAllTextAsync(_textFileName);

            PopulateTextBox(text);

            statusLabel.Text = @"Audio transcribed";
            progressBar.Value = 100;
        }
        catch (Exception ex)
        {
            statusLabel.Text = $@"Error: {ex.Message}";
            progressBar.Value = 0;
        }
    }

    private async void GetSummaryButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_textFileName))
        {
            statusLabel.Text = @"Please transcribe audio first!";
            return;
        }

        _summaryFileName = Path.ChangeExtension(_textFileName, ".sum");
        transcriptTextBox.Text = string.Empty;
        progressBar.Value = 6;

        try
        {
            statusLabel.Text = @"Analyzing transcription...";
            progressBar.Value = new Random().Next(11, 34);

            var transcription = await File.ReadAllTextAsync(_textFileName);

            var summary = await _aiService.GetCompletionForPromptAsync(transcription, _prompts.PromptForSummary);

            await File.WriteAllTextAsync(_summaryFileName, summary);
            progressBar.Value = 100;

            PopulateTextBox(summary);

            statusLabel.Text = @"Transcription summarized";
        }
        catch (Exception ex)
        {
            statusLabel.Text = $@"Error: {ex.Message}";
            progressBar.Value = 0;
        }
    }

    private void PopulateTextBox(string text)
    {
        foreach (var item in text.Split(new string[] { "\n\n" }, StringSplitOptions.None))
        {
            transcriptTextBox.Text += item + Environment.NewLine + Environment.NewLine;
        }
    }

    private void StatusLabel_Click(object sender, EventArgs e)
    {
    }

    private void ProgressTimer_Tick(object sender, EventArgs e)
    {
        if (progressBar.Value < 90)
        {
            progressBar.Value += new Random().Next(1,6);
        }
        else
        {
            progressTimer.Stop();
        }
    }
}