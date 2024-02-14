using System.Reactive;
using ReactiveUI;

namespace yyTodoMail.ViewModels;

public class MainWindowViewModel: ViewModelBase
{
    private string? _sender;

    public string? Sender
    {
        get => _sender;
        set => this.RaiseAndSetIfChanged (ref _sender, value);
    }

    private string? _subject;

    public string? Subject
    {
        get => _subject;

        set
        {
            this.RaiseAndSetIfChanged (ref _subject, value);
            UpdateHasSubjectOrBody ();
        }
    }

    private string? _body;

    public string? Body
    {
        get => _body;

        set
        {
            this.RaiseAndSetIfChanged (ref _body, value);
            UpdateHasSubjectOrBody ();
        }
    }

    private string? _recipient;

    public string? Recipient
    {
        get => _recipient;
        set => this.RaiseAndSetIfChanged (ref _recipient, value);
    }

    private string? _translatedSubject;

    public string? TranslatedSubject
    {
        get => _translatedSubject;
        set
        {
            this.RaiseAndSetIfChanged (ref _translatedSubject, value);
            UpdateHasTranslation ();
        }
    }

    private string? _translatedBody;

    public string? TranslatedBody
    {
        get => _translatedBody;
        set
        {
            this.RaiseAndSetIfChanged (ref _translatedBody, value);
            UpdateHasTranslation ();
        }
    }

    // -----------------------------------------------------------------------------

    private bool _hasSubjectOrBody;

    public bool HasSubjectOrBody
    {
        get => _hasSubjectOrBody;
        set => this.RaiseAndSetIfChanged (ref _hasSubjectOrBody, value);
    }

    public void UpdateHasSubjectOrBody () => HasSubjectOrBody = string.IsNullOrWhiteSpace (Subject) == false || string.IsNullOrWhiteSpace (Body) == false;

    private bool _hasTranslation;

    public bool HasTranslation
    {
        get => _hasTranslation;
        set => this.RaiseAndSetIfChanged (ref _hasTranslation, value);
    }

    public void UpdateHasTranslation () => HasTranslation = string.IsNullOrWhiteSpace (TranslatedSubject) == false || string.IsNullOrWhiteSpace (TranslatedBody) == false;

    // -----------------------------------------------------------------------------

    public ReactiveCommand <Unit, Unit> SendCommand { get; }

    public ReactiveCommand <Unit, Unit> TranslateCommand { get; }

    public ReactiveCommand <Unit, Unit> SendTranslatedCommand { get; }

    public MainWindowViewModel ()
    {
        SendCommand = ReactiveCommand.Create (() =>
        {
            // todo
        });

        TranslateCommand = ReactiveCommand.Create (() =>
        {
            // todo
        });

        SendTranslatedCommand = ReactiveCommand.Create (() =>
        {
            // todo
        });
    }
}
