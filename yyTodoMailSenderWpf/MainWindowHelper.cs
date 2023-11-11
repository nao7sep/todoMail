using System.Windows;

namespace yyTodoMailSenderWpf
{
    public partial class MainWindow: Window
    {
        // Data binding doesnt like me.
        // A lot of code that is copied from the internet or ChatGPT fails to run.
        // Maybe it's me, maybe it's the IDE.
        // Sometimes, even code that is suggested by IntelliSense and updates the window preview is rejected by the compiler and there's really nothing I can do about it.
        // For WPF, I will stick to the good old event-driven programming.

        // In alphabetical order.
        // KISS.

        private void DisableSubjectAndBody ()
        {
            Subject.IsEnabled = false;
            Body.IsEnabled = false;
        }

        private void EnableSubjectAndBody ()
        {
            Subject.IsEnabled = true;
            Body.IsEnabled = true;
        }

        private bool IsEditing => string.IsNullOrWhiteSpace (Subject.Text) == false || string.IsNullOrWhiteSpace (Body.Text) == false;

        private void SetInitialFocus () => Subject.Focus ();

        private void UpdateIsEnabledOfSendAndTranslate ()
        {
            if (string.IsNullOrWhiteSpace (Subject.Text) && string.IsNullOrWhiteSpace (Body.Text))
            {
                Send.IsEnabled = false;
                Translate.IsEnabled = false;
            }

            else
            {
                Send.IsEnabled = true;
                Translate.IsEnabled = true;
            }
        }

        private void UpdateTextOfSenderAndRecipient ()
        {
            Sender.Text = App.SenderString;
            Recipient.Text = App.RecipientString;
        }
    }
}
