# YoutubePodSmart

YoutubePodSmart is a .NET MAUI application designed to download videos from YouTube, extract audio, transcribe the audio, and summarize the transcription. It supports cross-platform operation on both Windows and Android.

## Features

- Download videos from YouTube
- Extract audio from videos
- Transcribe audio using AI
- Summarize the transcriptions
- Option to keep or delete the downloaded video, extracted audio, and transcriptions

## Technology Stack

- **.NET MAUI**: Cross-platform framework for creating native mobile and desktop apps with C# and XAML.
- **FFMpegCore**: A .NET wrapper for FFmpeg used to handle audio extraction.
- **OpenAI**: Used for audio transcription and summarization.
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Logging**: Microsoft.Extensions.Logging
- **Configuration**: Microsoft.Extensions.Configuration

## Project Structure

- **MainViewModel.cs**: The main logic for handling video download, audio extraction, transcription, and summarization.
- **MainPage.xaml**: The UI layout for the main page of the application.
- **VideoInfo.cs**: Model class for holding video-related information.

## How It Works

### Short Algorithm

1. **Download Video**: Fetch the video from YouTube using the provided URL.
2. **Extract Audio**: Extract the audio from the downloaded video file.
3. **Transcribe Audio**: Use an AI service to transcribe the extracted audio to text.
4. **Summarize Transcription**: Summarize the transcription using the AI service.
5. **Cleanup**: Optionally delete the video, audio, and transcription files based on user preferences.

### Configuration

The application uses an `appsettings.json` file to store configuration settings such as folder paths and API keys.

### Usage

1. **Set Up Configuration**: Ensure the `appsettings.json` file is correctly set up with necessary API keys and folder paths.
2. **Download and Extract Audio**: Enter the YouTube video URL and run the application to download and extract audio.
3. **Transcribe and Summarize**: The application will transcribe the audio and provide a summary of the transcription.
4. **Review Logs**: Check the application logs for detailed steps and any errors encountered.

## Installation

1. Clone the repository: `git clone https://github.com/yourusername/YoutubePodSmart.git`
2. Navigate to the project directory: `cd YoutubePodSmart`
3. Restore dependencies: `dotnet restore`
4. Add necessary FFmpeg binaries to the `ffmpeg` folder.
5. Build the project: `dotnet build`

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- **FFMpegCore** for providing a .NET wrapper for FFmpeg.
- **OpenAI** for the transcription and summarization services.
- **Microsoft** for the .NET MAUI framework.