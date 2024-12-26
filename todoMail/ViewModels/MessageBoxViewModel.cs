using ReactiveUI;

namespace todoMail.ViewModels;

public class MessageBoxWindowViewModel: ViewModelBase
{
    private string? _caption;

    public string? Caption
    {
        get => _caption;
        set => this.RaiseAndSetIfChanged (ref _caption, value);
    }

    private string? _message;

    public string? Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged (ref _message, value);
    }

    private string? _firstButtonContent;

    public string? FirstButtonContent
    {
        get => _firstButtonContent;
        set => this.RaiseAndSetIfChanged (ref _firstButtonContent, value);
    }

    private bool _isSecondButtonVisible;

    public bool IsSecondButtonVisible
    {
        get => _isSecondButtonVisible;
        set
        {
            this.RaiseAndSetIfChanged (ref _isSecondButtonVisible, value);
            UpdateButtonsContents ();
        }
    }

    private string? _secondButtonContent;

    public string? SecondButtonContent
    {
        get => _secondButtonContent;
        set => this.RaiseAndSetIfChanged (ref _secondButtonContent, value);
    }

    public void UpdateButtonsContents ()
    {
        if (IsSecondButtonVisible == false)
        {
            FirstButtonContent = "OK";
            SecondButtonContent = null;
        }

        else
        {
            FirstButtonContent = "Yes";
            SecondButtonContent = "No";
        }
    }
}
