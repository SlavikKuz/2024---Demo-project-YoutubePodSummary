using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YoutubePodSmart.Common.Contracts;
using YoutubePodSmart.Common.SettingsModels;
using YoutubePodSmart.Maui.AudioExtractor;
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
    private bool _keepVideo;
    private bool _keepAudio;
    private bool _keepTranscription;
    private bool _isBusy;
    private string _viewText;

    public VideoInfo VideoInfo
    {
        get => _videoInfo;
        set
        {
            _videoInfo = value;
            OnPropertyChanged();
        }
    }

    public bool KeepVideo
    {
        get => _keepVideo;
        set
        {
            _keepVideo = value;
            OnPropertyChanged();
        }
    }

    public bool KeepAudio
    {
        get => _keepAudio;
        set
        {
            _keepAudio = value;
            OnPropertyChanged();
        }
    }

    public bool KeepTranscription
    {
        get => _keepTranscription;
        set
        {
            _keepTranscription = value;
            OnPropertyChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public string ViewText
    {
        get => _viewText;
        set
        {
            _viewText = value;
            OnPropertyChanged();
        }
    }

    public ICommand GetVideoCommand { get; }

    public MainViewModel()
    {
        // Initialize commands to do nothing to prevent null reference exceptions
        GetVideoCommand = new Command(() => { });
        _logger?.LogInformation("Parameterless constructor called.");
    }

    public MainViewModel(IConfiguration configuration, ILogger<MainViewModel> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var apiKey = _configuration["OpenAiSettings:ApiKey"];
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

        // Set default values for checkboxes
        _keepVideo = true;
        _keepAudio = true;
        _keepTranscription = true;

        GetVideoCommand = new Command(async () => await ProcessFileAsync(VideoInfo.VideoUrl));
    }

    public async Task ProcessFileAsync(string videoFileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(videoFileUrl))
                throw new ArgumentException("Video URL is required");

            IsBusy = true;
            ViewText = "Starting process...\n";

            ViewText += "Downloading video...\n";
            await GetVideoAsync(videoFileUrl);

            ViewText += "Extracting audio...\n";
            await GetAudioAsync();

            ViewText += "Transcribing audio...\n";
            await TranscribeAudioAsync();

            ViewText += "Summarizing transcription...\n";
            await SummarizeTranscriptionAsync();

            CleanupFiles();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: {Message}", ex.Message);
            await Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "OK")!;
            ViewText += $"Error: {ex.Message}\n";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string GetPlatformSpecificPath(string folderName)
    {
        string basePath;

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            basePath = FileSystem.AppDataDirectory;
        }
        else if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
        else
        {
            throw new PlatformNotSupportedException("Platform not supported");
        }

        return Path.Combine(basePath, folderName);
    }

    private async Task GetVideoAsync(string videoFileUrl)
    {
        IVideoProvider youtube = new Youtube(videoFileUrl);
        var folderPath = GetPlatformSpecificPath(_configuration["Settings:FolderPath"]);

        EnsureDirectoryExists(folderPath);

        VideoInfo.VideoFileName = await Task.Run(() => { return youtube.GetVideoFileNameAsync(folderPath); });

        if (File.Exists(VideoInfo.VideoFileName))
            return;

        await Task.Run(() => youtube.GetVideoAsync(VideoInfo.VideoFileName));
        _logger.LogInformation("Video downloaded successfully to path: {VideoPath}", VideoInfo.VideoFileName);
        ViewText += $"Video downloaded to: {VideoInfo.VideoFileName}\n";
    }

    public async Task GetAudioAsync()
    {
        VideoInfo.AudioFileName = Path.ChangeExtension(VideoInfo.VideoFileName, ".mp3");

        if (File.Exists(VideoInfo.AudioFileName))
            return;

        await new ExtractorWindowsFfMpeg().GetAudioFromVideoAsync(VideoInfo.VideoFileName, VideoInfo.AudioFileName);

        //await new ExtractorAndroidFfMpegCore().GetAudioFromVideoAsync(VideoInfo.VideoFileName, VideoInfo.AudioFileName);
        _logger.LogInformation("Audio extracted successfully to path: {AudioPath}", VideoInfo.AudioFileName);
        ViewText += $"Audio extracted to: {VideoInfo.AudioFileName}\n";
    }

    public async Task TranscribeAudioAsync()
    {
        VideoInfo.TextFileName = Path.ChangeExtension(VideoInfo.AudioFileName, ".txt");

        if (File.Exists(VideoInfo.TextFileName))
            return;

        var transcription = await Task.Run(async () => await _aiService.TranscribeAudioAsync(VideoInfo.AudioFileName));
        transcription = await Task.Run(async () => await _aiService.GetCompletionForPromptAsync(transcription,
            _prompts.PromptForAudioTranscriptionTextNormalization));

        await File.WriteAllTextAsync(VideoInfo.TextFileName, transcription);
        _logger.LogInformation("Audio transcribed successfully. Transcription saved to: {TextFile}",
            VideoInfo.TextFileName);
        ViewText += $"Transcription saved to: {VideoInfo.TextFileName}\n";
    }

    public async Task SummarizeTranscriptionAsync()
    {
        VideoInfo.SummaryFileName = Path.ChangeExtension(VideoInfo.TextFileName, ".sum");

        var transcription = await Task.Run(async () => await File.ReadAllTextAsync(VideoInfo.TextFileName));
        var summary = await Task.Run(async () =>
            await _aiService.GetCompletionForPromptAsync(transcription, _prompts.PromptForSummary));

        await File.WriteAllTextAsync(VideoInfo.SummaryFileName, summary);
        _logger.LogInformation("Transcription summarized successfully. Summary saved to: {SummaryFile}",
            VideoInfo.SummaryFileName);

        ViewText = summary;
    }

    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public void CleanupFiles()
    {
        if (!KeepVideo && File.Exists(VideoInfo.VideoFileName))
        {
            File.Delete(VideoInfo.VideoFileName);
            _logger.LogInformation("Video file deleted: {VideoFileName}", VideoInfo.VideoFileName);
        }

        if (!KeepAudio && File.Exists(VideoInfo.AudioFileName))
        {
            File.Delete(VideoInfo.AudioFileName);
            _logger.LogInformation("Audio file deleted: {AudioFileName}", VideoInfo.AudioFileName);
        }

        if (!KeepTranscription && File.Exists(VideoInfo.TextFileName))
        {
            File.Delete(VideoInfo.TextFileName);
            _logger.LogInformation("Transcription file deleted: {TextFileName}", VideoInfo.TextFileName);
        }

        if (!KeepTranscription && File.Exists(VideoInfo.SummaryFileName))
        {
            File.Delete(VideoInfo.SummaryFileName);
            _logger.LogInformation("Summary file deleted: {SummaryFileName}", VideoInfo.SummaryFileName);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}