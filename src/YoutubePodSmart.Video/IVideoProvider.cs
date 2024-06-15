namespace YoutubePodSmart.Video;

public interface IVideoProvider
{
    Task<string> GetVideoFileNameAsync(string path);
    Task<string> GetVideoAsync(string videoPath, IProgress<double>? progress = null);
}