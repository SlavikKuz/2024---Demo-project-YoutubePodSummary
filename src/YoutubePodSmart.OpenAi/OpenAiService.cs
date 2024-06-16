using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using YoutubePodSmart.Common.Contracts;

namespace YoutubePodSmart.OpenAi;

public class OpenAiService : IAiService
{
    private readonly OpenAIClient _aiClient;

    private readonly string _audioModel;
    private readonly string _completionModel;

    public OpenAiService(string apiKey, string audioModel, string completionModel)
    {
        if(string.IsNullOrEmpty(apiKey))
            throw new ArgumentNullException(nameof(apiKey));

        _aiClient = new OpenAIClient(apiKey);

        _audioModel = audioModel;
        _completionModel = completionModel;
    }

    public async Task<string> TranscribeAudioAsync(string audioFilePath)
    {
        AudioTranscription transcription = await _aiClient
            .GetAudioClient(_audioModel)
            .TranscribeAudioAsync(audioFilePath);

        return transcription.Text;
    }

    public async Task<string> GetCompletionForPromptAsync(string prompt, string text)
    {
        ChatCompletion completion = await _aiClient
            .GetChatClient(_completionModel)
            .CompleteChatAsync($"{prompt}. {text}");

        return completion.Content[0].Text;
    }
}