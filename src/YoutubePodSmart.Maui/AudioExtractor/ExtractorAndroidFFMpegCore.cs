using FFMpegCore;
using FFMpegCore.Enums;
using YoutubePodSmart.Common.Contracts;

namespace YoutubePodSmart.Maui.AudioExtractor;

public class ExtractorAndroidFfMpegCore : IAudioProvider
{
    public ExtractorAndroidFfMpegCore()
    {
        var ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
        GlobalFFOptions.Configure(options => options.BinaryFolder = ffmpegPath);

        // Ensure FFmpeg binaries are executable (for non-Windows platforms)
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            SetExecutable(Path.Combine(ffmpegPath, "ffmpeg"));
            SetExecutable(Path.Combine(ffmpegPath, "ffprobe"));
        }
    }

    private void SetExecutable(string path)
    {
        var fileInfo = new FileInfo(path);
        fileInfo.Attributes = FileAttributes.Normal;
        // For Unix-based systems (like Android), set executable permission
        System.Diagnostics.Process.Start("chmod", $"+x {path}");
    }

    public async Task GetAudioFromVideoAsync(string inputVideoFilePath, string outputAudioFilePath)
    {
        await FFMpegArguments
            .FromFileInput(inputVideoFilePath)
            .OutputToFile(outputAudioFilePath, true, options => options
                .WithVideoCodec(VideoCodec.LibX264)
                .WithConstantRateFactor(21)
                .WithAudioCodec(AudioCodec.Aac)
                .WithVideoBitrate(1500))
            .ProcessAsynchronously();
    }
}