using NReco.VideoConverter;

namespace YoutubePodSmart.Audio;

public class Extract
{
    public void GetAudioFromVideo(string inputVideoFilePath, string outputAudioFilePath, IProgress<double> progress)
    {
        var converter = new FFMpegConverter();
        converter.ConvertProgress += (sender, args) => progress?.Report(args.Processed.Ticks / (double)args.TotalDuration.Ticks);
        converter.ConvertMedia(inputVideoFilePath, outputAudioFilePath, "mp3");
    }
}