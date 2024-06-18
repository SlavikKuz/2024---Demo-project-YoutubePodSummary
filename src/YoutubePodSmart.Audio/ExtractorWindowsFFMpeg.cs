using NReco.VideoConverter;
using YoutubePodSmart.Common.Contracts;

namespace YoutubePodSmart.Maui.AudioExtractor;

public class ExtractorWindowsFfMpeg : IAudioProvider
{
    public async Task GetAudioFromVideoAsync(string inputVideoFilePath, string outputAudioFilePath)
    {
        var converter = new FFMpegConverter();
        await Task.Run(() => converter.ConvertMedia(inputVideoFilePath, outputAudioFilePath, "mp3"));
    }
}