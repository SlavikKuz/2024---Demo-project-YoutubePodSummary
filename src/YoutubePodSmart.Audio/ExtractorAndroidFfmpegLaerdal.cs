//using Xabe.FFmpeg;
//using Xabe.FFmpeg.Downloader;
//using YoutubePodSmart.Common.Contracts;

//public class ExtractorAndroidFfmpegLaerdal : IAudioProvider
//{
//    public ExtractorAndroidFfmpegLaerdal()
//    {
//        string ffmpegPath = GetFfmpegPath().GetAwaiter().GetResult();
//        FFmpeg.SetExecutablesPath(ffmpegPath);

//        if (DeviceInfo.Platform == DevicePlatform.Android)
//        {
//            CheckAndSetExecutable(ffmpegPath, "ffmpeg");
//            CheckAndSetExecutable(ffmpegPath, "ffprobe");
//        }
//    }

//    private async Task<string> GetFfmpegPath()
//    {
//        string basePath = FileSystem.AppDataDirectory;
//        string ffmpegPath = Path.Combine(basePath, "ffmpeg");

//        // Ensure ffmpeg directory exists
//        if (!Directory.Exists(ffmpegPath))
//        {
//            Directory.CreateDirectory(ffmpegPath);
//        }

//        // Copy ffmpeg binaries if they don't exist in the app data directory
//        await CopyBinaryIfNotExists("ffmpeg");
//        await CopyBinaryIfNotExists("ffprobe");

//        return ffmpegPath;
//    }

//    private async Task CopyBinaryIfNotExists(string fileName)
//    {
//        string destinationPath = Path.Combine(FileSystem.AppDataDirectory, "ffmpeg", fileName);

//        if (!File.Exists(destinationPath))
//        {
//            try
//            {
//                await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Android, FileSystem.AppDataDirectory);

//                Console.WriteLine($"Downloaded {fileName} to {destinationPath}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error downloading {fileName} to {destinationPath}: {ex.Message}");
//                throw;
//            }
//        }
//        else
//        {
//            Console.WriteLine($"{fileName} already exists at {destinationPath}");
//        }
//    }

//    private void CheckAndSetExecutable(string directory, string fileName)
//    {
//        string path = Path.Combine(directory, fileName);
//        if (File.Exists(path))
//        {
//            var fileInfo = new FileInfo(path);
//            fileInfo.Attributes = FileAttributes.Normal;
//            using (var process = new System.Diagnostics.Process())
//            {
//                process.StartInfo.FileName = "/system/bin/sh";
//                process.StartInfo.Arguments = $"-c \"chmod +x {path}\"";
//                process.StartInfo.UseShellExecute = false;
//                process.StartInfo.CreateNoWindow = true;
//                process.Start();
//                process.WaitForExit();
//                Console.WriteLine($"Set {path} as executable");
//            }
//        }
//        else
//        {
//            Console.WriteLine($"Executable not found: {path}");
//            throw new FileNotFoundException($"Executable not found: {path}");
//        }
//    }

//    public async Task GetAudioFromVideoAsync(string inputVideoFilePath, string outputAudioFilePath)
//    {
//        if (!File.Exists(inputVideoFilePath))
//        {
//            throw new FileNotFoundException($"Input file not found: {inputVideoFilePath}");
//        }

//        try
//        {
//            string ffmpegPath = await GetFfmpegPath();
//            FFmpeg.SetExecutablesPath(ffmpegPath);

//            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Android, FFmpeg.ExecutablesPath);

//            var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(inputVideoFilePath, outputAudioFilePath);
//            await conversion.Start();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"An error occurred during the conversion: {ex.Message}");
//            throw;
//        }
//    }
//}