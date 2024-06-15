using NReco.VideoConverter;

namespace YoutubePodSmart.Audio;

public class Extract
{
    public void GetAudioFromVideo(string inputVideoFilePath, string outputAudioFilePath)
    {
        var converter = new FFMpegConverter();
        converter.ConvertMedia(inputVideoFilePath, outputAudioFilePath, "mp3");
    }
}