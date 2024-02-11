using System;
using System.Windows;
using yyLib;

namespace yyTodoMail
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App: Application
    {
        private void Application_Startup (object sender, StartupEventArgs e)
        {
            try
            {
                // If we directly check Sender and the others, the program will exit for each newly generated file,
                // forcing us to restart it a few times until the configuration files are all generated.

                var xSender = Session.Sender;
                var xRecipient = Session.Recipient;
                var xGptChatConnectionInfo = Session.GptChatConnectionInfo;
                var xMailConnectionInfo = Session.MailConnectionInfo;

                if (xSender == null || xRecipient == null || xGptChatConnectionInfo == null || xMailConnectionInfo == null)
                {
                    MessageBox.Show ("Please check the JSON configuration files located in the same directory as this application.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown ();
                    return;
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Shared.ShowExceptionMessageBox (null, xException);
            }
        }

        private void Application_Exit (object sender, ExitEventArgs e) // App closing.
        {
            try
            {
                Session.Cleanup ();
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Shared.ShowExceptionMessageBox (null, xException);
            }
        }

        private void Application_SessionEnding (object sender, SessionEndingCancelEventArgs e) // User logging off from Windows.
        {
            try
            {
                Session.Cleanup ();
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Shared.ShowExceptionMessageBox (null, xException);
            }
        }
    }
}
