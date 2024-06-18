namespace YoutubePodSmart.Common.Contracts;

public interface IAudioProvider
{
    Task GetAudioFromVideoAsync(string inputVideoFilePath, string outputAudioFilePath);
}
