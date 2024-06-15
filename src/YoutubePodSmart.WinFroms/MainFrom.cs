using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<MainForm> _logger;

    public MainForm(IConfiguration configuration, ILogger<MainForm> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var apiKey = _configuration["OpenAiSettings:ApiKey"] ??
                     throw new ArgumentNullException("ApiKey", "ApiKey not found in the configuration!");
        _logger.LogInformation("API key retrieved from configuration.");

        var models = new AiModelSettings();
        _configuration.GetSection("AiModelSettings").Bind(models);
        _logger.LogInformation("AI model settings retrieved from configuration.");

        _prompts = new PromptSettings();
        _configuration.GetSection("PromptSettings").Bind(_prompts);
        _logger.LogInformation("Prompt settings retrieved from configuration.");

        _aiService = new OpenAiService(apiKey, models.AudioModel, models.CompletionModel);
        _logger.LogInformation("OpenAiService initialized.");

        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        InitializeComponent();
    }

    private async void GetVideoButton_Click(object sender, EventArgs e)
    {
        var videoFileUrl = videoSourceTextBox.Text;

        if (string.IsNullOrEmpty(videoFileUrl))
        {
            UpdateStatus("Please provide a link to the video!", 0);
            _logger.LogWarning("No video URL provided.");
            return;
        }

        UpdateStatus("Getting ready...", GetRandomProgress(1, 4));
        _logger.LogInformation("Preparing to download video from URL: {VideoUrl}", videoFileUrl);

        var videoFilePath = Path.Combine(Folder);
        var youtube = new Youtube(videoFileUrl);

        try
        {
            _videoFileName = await youtube.GetVideoFileName(videoFilePath);

            if (File.Exists(_videoFileName))
            {
                UpdateStatus("Video already downloaded", 100);
                _logger.LogInformation("Video already exists at path: {VideoPath}", _videoFileName);
                return;
            }

            var progressHandler = new Progress<double>(val =>
            {
                var progressValue = (int)(val * 100);
                UpdateStatus($"Downloading video... {progressValue}%", progressValue);

                if (progressBar.Value == 100)
                    UpdateStatus("Video downloaded", 100);
            });

            await youtube.GetYoutubeVideo(_videoFileName, progressHandler);
            UpdateStatus("Video downloaded", 100);
            _logger.LogInformation("Video downloaded successfully to path: {VideoPath}", _videoFileName);
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error: {ex.Message}", 0);
            _logger.LogError(ex, "Failed to download video.");
        }
    }

    private async void GetAudioButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_videoFileName))
        {
            UpdateStatus("Please download a video first!", 0);
            _logger.LogWarning("Attempted to extract audio without a downloaded video.");
            return;
        }

        UpdateStatus("Analyzing source file...", GetRandomProgress(15, 37));
        _logger.LogInformation("Preparing to extract audio from video file: {VideoFile}", _videoFileName);

        _audioFileName = Path.ChangeExtension(_videoFileName, ".mp3");

        if (File.Exists(_audioFileName))
        {
            UpdateStatus("Audio already extracted", 100);
            _logger.LogInformation("Audio already exists at path: {AudioPath}", _audioFileName);
            return;
        }

        try
        {
            progressTimer.Start();
            UpdateStatus("Extracting audio...");

            await Task.Run(() => new Extract().GetAudioFromVideo(_videoFileName, _audioFileName));

            UpdateStatus("Audio extracted", 100);
            progressTimer.Stop();
            _logger.LogInformation("Audio extracted successfully to path: {AudioPath}", _audioFileName);
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error: {ex.Message}", 0);
            progressTimer.Stop();
            _logger.LogError(ex, "Failed to extract audio.");
        }
    }

    private async void TranscribeButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_audioFileName))
        {
            UpdateStatus("Please extract audio first!", 0);
            _logger.LogWarning("Attempted to transcribe audio without extracted audio file.");
            return;
        }

        _textFileName = Path.ChangeExtension(_audioFileName, ".txt");
        transcriptTextBox.Text = string.Empty;
        UpdateStatus("Preparing transcription...", 3);
        _logger.LogInformation("Preparing to transcribe audio file: {AudioFile}", _audioFileName);

        try
        {
            if (!File.Exists(_textFileName))
            {
                UpdateStatus("Transcribing audio...", GetRandomProgress(8, 21));
                progressTimer.Start();

                var transcription = await _aiService.TranscribeAudioAsync(_audioFileName);

                UpdateStatus("Normalizing transcription...");
                transcription = await _aiService.GetCompletionForPromptAsync(transcription,
                    _prompts.PromptForAudioTranscriptionTextNormalization);

                await File.WriteAllTextAsync(_textFileName, transcription);
            }

            var text = await File.ReadAllTextAsync(_textFileName);
            PopulateTextBox(text);

            UpdateStatus("Audio transcribed", 100);
            progressTimer.Stop();
            _logger.LogInformation("Audio transcribed successfully. Transcription saved to: {TextFile}", _textFileName);
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error: {ex.Message}", 0);
            progressTimer.Stop();
            _logger.LogError(ex, "Failed to transcribe audio.");
        }
    }

    private async void GetSummaryButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_textFileName))
        {
            UpdateStatus("Please transcribe audio first!", 0);
            _logger.LogWarning("Attempted to summarize transcription without a transcribed text file.");
            return;
        }

        _summaryFileName = Path.ChangeExtension(_textFileName, ".sum");
        transcriptTextBox.Text = string.Empty;
        UpdateStatus("Analyzing transcription...", GetRandomProgress(11, 34));
        progressTimer.Start();
        _logger.LogInformation("Preparing to summarize transcription file: {TextFile}", _textFileName);

        try
        {
            var transcription = await File.ReadAllTextAsync(_textFileName);
            var summary = await _aiService.GetCompletionForPromptAsync(transcription, _prompts.PromptForSummary);

            await File.WriteAllTextAsync(_summaryFileName, summary);
            PopulateTextBox(summary);

            UpdateStatus("Transcription summarized", 100);
            _logger.LogInformation("Transcription summarized successfully. Summary saved to: {SummaryFile}",
                _summaryFileName);
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error: {ex.Message}", 0);
            _logger.LogError(ex, "Failed to summarize transcription.");
        }
    }

    private void PopulateTextBox(string text) =>
        transcriptTextBox.Text = string.Join(Environment.NewLine + Environment.NewLine,
            text.Split(new string[] { "\n\n" }, StringSplitOptions.None));

    private void ProgressTimer_Tick(object sender, EventArgs e)
    {
        if (progressBar.Value < 90)
            progressBar.Value += new Random().Next(1, 6);
        else
            progressTimer.Stop();
    }

    private void UpdateStatus(string message, int progressValue = -1)
    {
        statusLabel.Text = message;

        if (progressValue >= 0)
            progressBar.Value = progressValue;
    }

    private static int GetRandomProgress(int min, int max) =>
        new Random().Next(min, max);
}