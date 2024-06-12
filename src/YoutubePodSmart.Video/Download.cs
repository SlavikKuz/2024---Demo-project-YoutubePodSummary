using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubePodSmart.Video;

public class Download
{
    public async Task<string> GetYoutubeVideo(string url, string path, IProgress<double>? progress = null)
    {
        var youtube = new YoutubeClient();
        var videoId = YoutubeExplode.Videos.VideoId.Parse(url);
        var video = await youtube.Videos.GetAsync(videoId);
        var videoTitle = video.Title;

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeFileName = string.Join("_", videoTitle.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
            .Trim();

        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        var videoPath = Path.Combine(path, $"{safeFileName}.mp4");

        await youtube.Videos.Streams.DownloadAsync(streamInfo, videoPath, progress);

        return videoPath;
    }
}