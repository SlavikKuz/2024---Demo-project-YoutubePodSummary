using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YoutubePodSmart.Audio;
using YoutubePodSmart.Common.Contracts;
using YoutubePodSmart.Common.SettingsModels;
using YoutubePodSmart.Maui.Models;
using YoutubePodSmart.OpenAi;
using YoutubePodSmart.Video;

namespace YoutubePodSmart.Maui.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IConfiguration _configuration;
    private readonly IAiService _aiService;
    private readonly ILogger<MainViewModel> _logger;
    private readonly PromptSettings _prompts;
    private VideoInfo _videoInfo;

    public VideoInfo VideoInfo
    {
        get => _videoInfo;
        set
        {
            _videoInfo = value;
            OnPropertyChanged();
        }
    }

    public ICommand GetVideoCommand { get; }
    public ICommand GetAudioCommand { get; }
    public ICommand TranscribeAudioCommand { get; }
    public ICommand SummarizeTranscriptionCommand { get; }

    public MainViewModel()
    {
        // Initialize commands to do nothing to prevent null reference exceptions
        GetVideoCommand = new Command(() => { });
        GetAudioCommand = new Command(() => { });
        TranscribeAudioCommand = new Command(() => { });
        SummarizeTranscriptionCommand = new Command(() => { });

        _logger?.LogInformation("Parameterless constructor called.");
    }

    public MainViewModel(IConfiguration configuration, ILogger<MainViewModel> logger)
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

        VideoInfo = new VideoInfo();

        GetVideoCommand = new Command(async () => await GetVideoAsync(VideoInfo.VideoUrl, "D:\\AppFolder"));
        GetAudioCommand = new Command(async () => await GetAudioAsync());
        TranscribeAudioCommand = new Command(async () => await TranscribeAudioAsync());
        SummarizeTranscriptionCommand = new Command(async () => await SummarizeTranscriptionAsync());
    }

    public async Task GetVideoAsync(string videoFileUrl, string folderPath)
    {
        if (string.IsNullOrEmpty(videoFileUrl))
        {
            throw new ArgumentException("Video URL is required", nameof(videoFileUrl));
        }

        IVideoProvider youtube = new Youtube(videoFileUrl);
        var videoFilePath = Path.Combine(folderPath);

        VideoInfo.VideoFileName = await youtube.GetVideoFileNameAsync(videoFilePath);
        if (File.Exists(VideoInfo.VideoFileName))
        {
            _logger.LogInformation("Video already exists at path: {VideoPath}", VideoInfo.VideoFileName);
            return;
        }

        var progressHandler = new Progress<double>(val =>
        {
            var progressValue = (int)(val * 100);
            // Update progress
        });

        await youtube.GetVideoAsync(VideoInfo.VideoFileName, progressHandler);
        _logger.LogInformation("Video downloaded successfully to path: {VideoPath}", VideoInfo.VideoFileName);
    }

    public async Task GetAudioAsync()
    {
        if (string.IsNullOrEmpty(VideoInfo.VideoFileName))
        {
            throw new InvalidOperationException("Download a video first.");
        }

        VideoInfo.AudioFileName = Path.ChangeExtension(VideoInfo.VideoFileName, ".mp3");

        if (File.Exists(VideoInfo.AudioFileName))
        {
            _logger.LogInformation("Audio already exists at path: {AudioPath}", VideoInfo.AudioFileName);
            return;
        }

        await Task.Run(() => new ExtractorFFMpeg().GetAudioFromVideo(VideoInfo.VideoFileName, VideoInfo.AudioFileName));
        _logger.LogInformation("Audio extracted successfully to path: {AudioPath}", VideoInfo.AudioFileName);
    }

    public async Task TranscribeAudioAsync()
    {
        if (string.IsNullOrEmpty(VideoInfo.AudioFileName))
        {
            throw new InvalidOperationException("Extract audio first.");
        }

        VideoInfo.TextFileName = Path.ChangeExtension(VideoInfo.AudioFileName, ".txt");

        if (File.Exists(VideoInfo.TextFileName))
        {
            return;
        }

        var transcription = await _aiService.TranscribeAudioAsync(VideoInfo.AudioFileName);
        transcription =
            await _aiService.GetCompletionForPromptAsync(transcription,
                _prompts.PromptForAudioTranscriptionTextNormalization);

        await File.WriteAllTextAsync(VideoInfo.TextFileName, transcription);
        _logger.LogInformation("Audio transcribed successfully. Transcription saved to: {TextFile}",
            VideoInfo.TextFileName);
    }

    public async Task SummarizeTranscriptionAsync()
    {
        if (string.IsNullOrEmpty(VideoInfo.TextFileName))
        {
            throw new InvalidOperationException("Transcribe audio first.");
        }

        VideoInfo.SummaryFileName = Path.ChangeExtension(VideoInfo.TextFileName, ".sum");

        var transcription = await File.ReadAllTextAsync(VideoInfo.TextFileName);
        var summary = await _aiService.GetCompletionForPromptAsync(transcription, _prompts.PromptForSummary);

        await File.WriteAllTextAsync(VideoInfo.SummaryFileName, summary);
        _logger.LogInformation("Transcription summarized successfully. Summary saved to: {SummaryFile}",
            VideoInfo.SummaryFileName);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}