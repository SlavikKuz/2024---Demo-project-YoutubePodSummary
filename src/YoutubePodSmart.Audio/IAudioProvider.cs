namespace YoutubePodSmart.Audio;

public interface IAudioProvider
{
    void GetAudioFromVideo(string inputVideoFilePath, string outputAudioFilePath);
}
