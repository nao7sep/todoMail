using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Smtp;
using MimeKit;
using yyGptLib;
using yyMailLib;

namespace yyTodoMailSenderWpf
{
    public partial class MainWindow: Window
    {
        // Data binding doesnt like me.
        // A lot of code that is copied from the internet or ChatGPT fails to run.
        // Maybe it's me, maybe it's the IDE.
        // Sometimes, even code that is suggested by IntelliSense and updates the window preview is rejected by the compiler and there's really nothing I can do about it.
        // For WPF, I will stick to the good old event-driven programming.

        // In alphabetical order:

        private bool IsEditing => string.IsNullOrWhiteSpace (Subject.Text) == false || string.IsNullOrWhiteSpace (Body.Text) == false;

        private void SendMessage (TextBox subjectControl, TextBox bodyControl)
        {
            try
            {
                // Initialized to avoid compiler warning.
                string xSubject = string.Empty,
                    xBody = string.Empty;

                WindowAlt.Dispatcher.Invoke (() =>
                {
                    xSubject = subjectControl.Text;
                    xBody = bodyControl.Text;
                });

                using MimeMessage xMessageForRecipient = new MimeMessage ();
                xMessageForRecipient.From.Add (new MailboxAddress (App.Sender!.Name, App.Sender!.Address));
                xMessageForRecipient.To.Add (new MailboxAddress (App.Recipient!.Name, App.Recipient!.Address));
                xMessageForRecipient.Subject = xSubject;
                xMessageForRecipient.Body = new TextPart ("plain") { Text = xBody };

                using SmtpClient xClient = new SmtpClient ();
                xClient.ConnectAsync (App.MailConnectionInfo!).Wait ();
                xClient.AuthenticateAsync (App.MailConnectionInfo!).Wait ();

                xClient.SendAsync (xMessageForRecipient).Wait ();
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void SetInitialFocus () => Subject.Focus ();

        private void TranslateAlt (TextBox sourceControl, TextBox targetControl)
        {
            try
            {
                // Initialized to avoid compiler warning.
                string xOriginalText = string.Empty;

                sourceControl.Dispatcher.Invoke (() => xOriginalText = sourceControl.Text);

                if (string.IsNullOrWhiteSpace (xOriginalText))
                    return;

                App.Conversation.Request.Stream = true;
                App.Conversation.Request.AddMessage (yyGptChatMessageRole.User, $"Please translate the following text into {App.Recipient!.PreferredLanguages! [0]} and return only the translated text:{Environment.NewLine}{Environment.NewLine}{xOriginalText}");
                App.Conversation.SendAsync ().Wait ();

                while (true)
                {
                    var xResult = App.Conversation.TryReadAndParseChunkAsync ().Result;

                    if (xResult.IsSuccess)
                    {
                        if (xResult.PartialMessage == null)
                            break; // End of stream or "data: [DONE]" is detected.

                        targetControl.Dispatcher.Invoke (() => targetControl.Text += xResult.PartialMessage);
                    }

                    else
                    {
                        if (xResult.PartialMessage != null)
                            MessageBox.Show (this, $"Error: {xResult.PartialMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        else MessageBox.Show (this, $"Exception: {xResult.Exception}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);

                        break;
                    }
                }
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

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

        private void UpdateIsEnabledOfSendTranslated ()
        {
            if (string.IsNullOrWhiteSpace (TranslatedSubject.Text) && string.IsNullOrWhiteSpace (TranslatedBody.Text))
                SendTranslated.IsEnabled = false;

            else SendTranslated.IsEnabled = true;
        }

        private void UpdateTextOfSenderAndRecipient ()
        {
            Sender.Text = App.SenderString;
            Recipient.Text = App.RecipientString;
        }
    }
}
