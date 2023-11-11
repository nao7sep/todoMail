using System;
using System.Windows;

namespace yyTodoMailSenderWpf
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
                if (Sender == null || Recipient == null || GptChatConnectionInfo == null || MailConnectionInfo == null)
                {
                    MessageBox.Show ("Please check the JSON configuration files located in the same directory as this application.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown ();
                    return;
                }
            }

            catch (Exception xException)
            {
                Logger.LogException (xException);
            }
        }

        private static void Cleanup () => Conversation.Dispose (); // May be unnecessary, but harmless.

        private void Application_Exit (object sender, ExitEventArgs e) // App closing.
        {
            try
            {
                Cleanup ();
            }

            catch (Exception xException)
            {
                Logger.LogException (xException);
            }
        }

        private void Application_SessionEnding (object sender, SessionEndingCancelEventArgs e) // User logging off from Windows.
        {
            try
            {
                Cleanup ();
            }

            catch (Exception xException)
            {
                Logger.LogException (xException);
            }
        }
    }
}
