using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;
using yyGptLib;
using yyLib;

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

    private bool _isWindowEnabled = true;

    public bool IsWindowEnabled
    {
        get => _isWindowEnabled;
        set => this.RaiseAndSetIfChanged (ref _isWindowEnabled, value);
    }

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
        SendCommand = ReactiveCommand.CreateFromTask (async () =>
        {
            try
            {
                IsWindowEnabled = false;

                var xTask = Task.Run (() =>
                {
                    var xSubjectAndBody = Shared.GetSubjectAndBody (Subject, Body, null, null); // Sends only Subject and Body.
                    return Shared.SendMessage (xSubjectAndBody.Subject, xSubjectAndBody.Body);
                });

                await xTask;

                if (xTask.Result)
                {
                    Subject = null;
                    Body = null;
                    TranslatedSubject = null;
                    TranslatedBody = null;

                    // todo: Set focus to Body.
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                MessageBox.ShowException (null, xException);
            }

            finally
            {
                IsWindowEnabled = true;
            }
        });

        TranslateCommand = ReactiveCommand.CreateFromTask (async () =>
        {
            try
            {
                IsWindowEnabled = false;
                TranslatedSubject = null;
                TranslatedBody = null;

                bool TranslateString (bool isSubject, string str)
                {
                    var xConversation = isSubject ? Session.ConversationForSubject : Session.ConversationForBody;

                    try
                    {
                        xConversation.Request.Stream = true;
                        xConversation.Request.AddMessage (yyGptChatMessageRole.User, $"Please translate the following text into {Session.Recipient!.PreferredLanguages! [0]} and return only the translated text:{Environment.NewLine}{Environment.NewLine}{str}");
                        xConversation.SendAsync ().Wait ();

                        while (true)
                        {
                            var xResult = xConversation.TryReadAndParseChunkAsync ().Result;

                            if (xResult.IsSuccess)
                            {
                                if (xResult.PartialMessage == null)
                                    break; // End of stream or "data: [DONE]" is detected.

                                Dispatcher.UIThread.InvokeAsync (() =>
                                {
                                    if (isSubject)
                                        TranslatedSubject += xResult.PartialMessage;

                                    else TranslatedBody += xResult.PartialMessage;
                                });
                            }

                            else
                            {
                                if (xResult.RawContent != null)
                                    yySimpleLogger.Default.TryWrite ("Translation RawContent", xResult.RawContent.GetVisibleString ()); // Just in case.

                                if (xResult.PartialMessage != null)
                                    throw new yyApplicationException ($"Translation Error: {xResult.PartialMessage.GetVisibleString ()}");

                                else throw new yyApplicationException ($"Translation Exception: {xResult.Exception}"); // Not exactly an user friendly message, though.
                            }
                        }

                        return true;
                    }

                    catch (Exception xException)
                    {
                        yySimpleLogger.Default.TryWriteException (xException);
                        MessageBox.ShowException (null, xException);
                        return false;
                    }

                    finally
                    {
                        while (xConversation.Request.Messages!.Count >= 2)
                            xConversation.Request.RemoveLastMessage ();
                    }
                }

                var xTasks = Enumerable.Range (0, 2).Select (x => Task.Run (() =>
                {
                    if (x == 0)
                    {
                        string xSubject = (Subject ?? string.Empty).Trim ();

                        if (string.IsNullOrWhiteSpace (xSubject) == false)
                            return TranslateString (true, xSubject);

                        else return true;
                    }

                    else
                    {
                        string xBody = (Body ?? string.Empty).Trim ();

                        if (string.IsNullOrWhiteSpace (xBody) == false)
                            return TranslateString (false, xBody);

                        else return true;
                    }
                })).
                ToArray (); // If we dont finalize this here, 2 sets of the same tasks will be created.

                await Task.WhenAll (xTasks);

                if (xTasks.All (x => x.Result))
                {
                    // We often need to modify the body and re-translate it.
                    // todo: Set focus to Body.
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                MessageBox.ShowException (null, xException);
            }

            finally
            {
                IsWindowEnabled = true;
            }
        });

        SendTranslatedCommand = ReactiveCommand.CreateFromTask (async () =>
        {
            try
            {
                IsWindowEnabled = false;

                var xTask = Task.Run (() =>
                {
                    var xSubjectAndBody = Shared.GetSubjectAndBody (Subject, Body, TranslatedSubject, TranslatedBody); // Sends everything.
                    return Shared.SendMessage (xSubjectAndBody.Subject, xSubjectAndBody.Body);
                });

                await xTask;

                if (xTask.Result)
                {
                    Subject = null;
                    Body = null;
                    TranslatedSubject = null;
                    TranslatedBody = null;

                    // todo: Set focus to Body.
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                MessageBox.ShowException (null, xException);
            }

            finally
            {
                IsWindowEnabled = true;
            }
        });
    }
}
