using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using YoutubePodSmart.Common.Contracts;
using YoutubePodSmart.Maui.AudioExtractor;
using YoutubePodSmart.Maui.ViewModels;

namespace Tests;

[TestFixture]
public class MainViewModelTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<MainViewModel>> _mockLogger;
    private Mock<IAiService> _mockAiService;
    private Mock<IAudioProvider> _mockAudioExtractor;
    private MainViewModel _viewModel;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<MainViewModel>>();
        _mockAiService = new Mock<IAiService>();
        _mockAudioExtractor = new Mock<IAudioProvider>();

        _viewModel = new MainViewModel(
            _mockConfiguration.Object,
            _mockLogger.Object
        );
    }

    [Test]
    public void VideoUrl_PropertyChanged()
    {
        // Arrange
        bool propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(_viewModel.VideoInfo.VideoUrl))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.VideoInfo.VideoUrl = "https://example.com/video.mp4";

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    [Test]
    public void KeepVideo_DefaultTrue()
    {
        // Assert
        Assert.IsTrue(_viewModel.KeepVideo);
    }

    [Test]
    public async Task ProcessFileAsync_SetsIsBusyTrue()
    {
        // Arrange
        string videoUrl = "https://example.com/video.mp4";

        // Act
        var processTask = _viewModel.ProcessFileAsync(videoUrl);

        // Assert
        Assert.IsTrue(_viewModel.IsBusy);
        await processTask;
    }

    [Test]
    public async Task ProcessFileAsync_UpdatesViewText()
    {
        // Arrange
        string videoUrl = "https://example.com/video.mp4";
        _mockAiService.Setup(x => x.TranscribeAudioAsync(It.IsAny<string>())).ReturnsAsync("Transcription");
        _mockAiService.Setup(x => x.GetCompletionForPromptAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Summary");
        _mockAudioExtractor.Setup(x => x.GetAudioFromVideoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.ProcessFileAsync(videoUrl);

        // Assert
        StringAssert.Contains("Starting process...", _viewModel.ViewText);
        StringAssert.Contains("Downloading video...", _viewModel.ViewText);
        StringAssert.Contains("Extracting audio...", _viewModel.ViewText);
        StringAssert.Contains("Transcribing audio...", _viewModel.ViewText);
        StringAssert.Contains("Summarizing transcription...", _viewModel.ViewText);
    }

    [Test]
    public void CleanupFiles_RemovesVideoFile_WhenKeepVideoIsFalse()
    {
        // Arrange
        _viewModel.KeepVideo = false;
        _viewModel.VideoInfo.VideoFileName = "video.mp4";
        File.Create(_viewModel.VideoInfo.VideoFileName).Dispose();

        // Act
        _viewModel.CleanupFiles();

        // Assert
        Assert.IsFalse(File.Exists(_viewModel.VideoInfo.VideoFileName));
    }

    [Test]
    public void CleanupFiles_RemovesAudioFile_WhenKeepAudioIsFalse()
    {
        // Arrange
        _viewModel.KeepAudio = false;
        _viewModel.VideoInfo.AudioFileName = "audio.mp3";
        File.Create(_viewModel.VideoInfo.AudioFileName).Dispose();

        // Act
        _viewModel.CleanupFiles();

        // Assert
        Assert.IsFalse(File.Exists(_viewModel.VideoInfo.AudioFileName));
    }

    [Test]
    public void CleanupFiles_RemovesTranscriptionFile_WhenKeepTranscriptionIsFalse()
    {
        // Arrange
        _viewModel.KeepTranscription = false;
        _viewModel.VideoInfo.TextFileName = "transcription.txt";
        File.Create(_viewModel.VideoInfo.TextFileName).Dispose();

        // Act
        _viewModel.CleanupFiles();

        // Assert
        Assert.IsFalse(File.Exists(_viewModel.VideoInfo.TextFileName));
    }
}