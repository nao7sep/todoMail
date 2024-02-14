using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using yyLib;
using yyTodoMail.ViewModels;
using yyTodoMail.Views;

namespace yyTodoMail;

public partial class App: Application
{
    public override void Initialize ()
    {
        AvaloniaXamlLoader.Load (this);
    }

    public override void OnFrameworkInitializationCompleted ()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel ()
            };

            desktop.Startup += (sender, e) =>
            {
                try
                {
                    // If we directly check Sender and the others, the program will exit for each newly generated file,
                    //     forcing us to restart it a few times until the configuration files are all generated.

                    var xSender = Session.Sender;
                    var xRecipient = Session.Recipient;
                    var xGptChatConnectionInfo = Session.GptChatConnectionInfo;
                    var xMailConnectionInfo = Session.MailConnectionInfo;

                    if (xSender == null || xRecipient == null || xGptChatConnectionInfo == null || xMailConnectionInfo == null)
                    {
                        MessageBox.Show (null, "Configuration Error", "Please check the JSON configuration files located in the same directory as this application.", isSecondButtonVisible: false);
                        // desktop.Shutdown (1); // We need the UI thread to be running to show the message box.
                        desktop.MainWindow = null; // Prevents the main window from appearing.
                    }
                }

                catch (Exception xException)
                {
                    yySimpleLogger.Default.TryWriteException (xException);
                    MessageBox.ShowException (null, xException);
                }
            };

            desktop.Exit += (sender, e) =>
            {
                try
                {
                    Session.Cleanup ();
                }

                catch (Exception xException)
                {
                    yySimpleLogger.Default.TryWriteException (xException);
                    MessageBox.ShowException (null, xException);
                }
            };
        }

        base.OnFrameworkInitializationCompleted ();
    }
}
