using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YoutubePodSmart.Maui.Models;

public class VideoInfo : INotifyPropertyChanged
{
    private string _videoUrl;
    private string _videoFileName;
    private string _audioFileName;
    private string _textFileName;
    private string _summaryFileName;

    public string VideoUrl
    {
        get => _videoUrl;
        set
        {
            _videoUrl = value;
            OnPropertyChanged();
        }
    }

    public string VideoFileName
    {
        get => _videoFileName;
        set
        {
            _videoFileName = value;
            OnPropertyChanged();
        }
    }

    public string AudioFileName
    {
        get => _audioFileName;
        set
        {
            _audioFileName = value;
            OnPropertyChanged();
        }
    }

    public string TextFileName
    {
        get => _textFileName;
        set
        {
            _textFileName = value;
            OnPropertyChanged();
        }
    }

    public string SummaryFileName
    {
        get => _summaryFileName;
        set
        {
            _summaryFileName = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}