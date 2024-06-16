using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using YoutubePodSmart.Common.Contracts;

namespace YoutubePodSmart.Video;

public class Youtube : IVideoProvider
{
    private readonly YoutubeClient _client;
    private readonly VideoId _videoId;

    public Youtube(string url)
    {
        _client = new YoutubeClient();
        _videoId = VideoId.Parse(url);
    }

    public async Task<string> GetVideoFileNameAsync(string path)
    {
        var video = await _client.Videos.GetAsync(_videoId);
        var videoTitle = video.Title;

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeFileName = string.Join("_", videoTitle.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
            .Trim();

        return Path.Combine(path, $"{safeFileName}.mp4");
    }


    public async Task<string> GetVideoAsync(string videoPath, IProgress<double>? progress = null)
    {
        var streamManifest = await _client.Videos.Streams.GetManifestAsync(_videoId);
        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        await _client.Videos.Streams.DownloadAsync(streamInfo, videoPath, progress);

        return videoPath;
    }
}