namespace YoutubePodSmart.Common.Contracts;

public interface IAiService
{
    public Task<string> TranscribeAudioAsync(string audioFilePath);

    public Task<string> GetCompletionForPromptAsync(string prompt, string text);
}