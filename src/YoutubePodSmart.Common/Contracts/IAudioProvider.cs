namespace YoutubePodSmart.Common.Contracts;

public interface IAudioProvider
{
    void GetAudioFromVideo(string inputVideoFilePath, string outputAudioFilePath);
}
