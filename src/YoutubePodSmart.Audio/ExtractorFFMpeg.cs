using NReco.VideoConverter;
using YoutubePodSmart.Common.Contracts;

namespace YoutubePodSmart.Audio;

public class ExtractorFFMpeg : IAudioProvider
{
    public void GetAudioFromVideo(string inputVideoFilePath, string outputAudioFilePath)
    {
        var converter = new FFMpegConverter();
        converter.ConvertMedia(inputVideoFilePath, outputAudioFilePath, "mp3");
    }
}