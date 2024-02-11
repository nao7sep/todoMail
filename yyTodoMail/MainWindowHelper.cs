using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Smtp;
using MimeKit;
using yyGptLib;
using yyLib;
using yyMailLib;

namespace yyTodoMail
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

        private bool SendMessage (TextBox subjectControl, TextBox bodyControl)
        {
            try
            {
                // Initialized to avoid compiler warning.
                string xSubject = string.Empty;
                StringBuilder xBody = new ();

                WindowAlt.Dispatcher.Invoke (() =>
                {
                    xSubject = subjectControl.Text.Trim ();
                    xBody.Append (yyString.TrimLines (bodyControl.Text));

                    // Added code.
                    // Should be more useful now.

                    if (subjectControl == TranslatedSubject)
                    {
                        string xBorderline = new ('-', 4);

                        if (string.IsNullOrWhiteSpace (Subject.Text) == false)
                        {
                            if (xBody.Length > 0)
                            {
                                xBody.AppendLine ();
                                xBody.AppendLine (xBorderline);
                            }

                            xBody.Append (Subject.Text.Trim ());
                        }

                        if (string.IsNullOrWhiteSpace (Body.Text) == false)
                        {
                            if (xBody.Length > 0)
                            {
                                xBody.AppendLine ();
                                xBody.AppendLine (xBorderline);
                            }

                            xBody.Append (yyString.TrimLines (Body.Text));
                        }
                    }
                });

                // Everything bad that occurs during the sending should be logged.

                using MimeMessage xMessage = new ();
                xMessage.From.Add (new MailboxAddress (Session.Sender!.Name, Session.Sender!.Address));
                xMessage.To.Add (new MailboxAddress (Session.Recipient!.Name, Session.Recipient!.Address));
                xMessage.Subject = $"[TODO] {xSubject}";
                xMessage.Body = new TextPart ("plain") { Text = xBody.ToString () };

                using SmtpClient xClient = new ();
                xClient.ConnectAsync (Session.MailConnectionInfo!).Wait ();
                xClient.AuthenticateAsync (Session.MailConnectionInfo!).Wait ();
                xClient.SendAsync (xMessage).Wait ();

                MailStorage.Store (xMessage);

                return true;
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);

                // Caught exceptions, even if they are re-thrown, will not affect the upper Task.
                // Errors in this class must be reported to the user by this class.
                MessageBox.Show ($"Exception: {xException}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }

        private void SetInitialFocus () => Subject.Focus ();

        private static bool TranslateAlt (TextBox sourceControl, TextBox targetControl)
        {
            try
            {
                // Initialized to avoid compiler warning.
                string xOriginalText = string.Empty;

                sourceControl.Dispatcher.Invoke (() => xOriginalText = sourceControl.Text);

                if (string.IsNullOrWhiteSpace (xOriginalText))
                    return true; // Not an error.

                Session.Conversation.Request.Stream = true;
                Session.Conversation.Request.AddMessage (yyGptChatMessageRole.User, $"Please translate the following text into {Session.Recipient!.PreferredLanguages! [0]} and return only the translated text:{Environment.NewLine}{Environment.NewLine}{xOriginalText}");
                Session.Conversation.SendAsync ().Wait ();

                while (true)
                {
                    var xResult = Session.Conversation.TryReadAndParseChunkAsync ().Result;

                    if (xResult.IsSuccess)
                    {
                        if (xResult.PartialMessage == null)
                            break; // End of stream or "data: [DONE]" is detected.

                        targetControl.Dispatcher.Invoke (() => targetControl.Text += xResult.PartialMessage);
                    }

                    else
                    {
                        // Logging what can be.
                        // "Translation Exception" isnt really a user friendly term, but it should be OK.

                        if (xResult.RawContent != null)
                            yySimpleLogger.Default.TryWrite ("Translation RawContent", xResult.RawContent.GetVisibleString ());

                        if (xResult.PartialMessage != null)
                        {
                            yySimpleLogger.Default.TryWrite ("Translation Error", xResult.PartialMessage.GetVisibleString ());
                            MessageBox.Show ($"Error: {xResult.PartialMessage.GetVisibleString ()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // Cant access 'owner' here.
                        }

                        else
                        {
                            yySimpleLogger.Default.TryWrite ("Translation Exception", xResult.Exception!.ToString ());
                            MessageBox.Show ($"Exception: {xResult.Exception}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        return false;
                    }
                }

                return true;
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                MessageBox.Show ($"Exception: {xException}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            finally
            {
                // Keeping the head empty.

                while (Session.Conversation.Request.Messages!.Count > 1)
                    Session.Conversation.Request.RemoveLastMessage ();
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
            Sender.Text = Session.SenderString;
            Recipient.Text = Session.RecipientString;
        }

        private void InitializeWindow ()
        {
            string? xTitle = Shared.AppSpecificConfig ["Title"];

            if (string.IsNullOrWhiteSpace (xTitle) == false)
                Title = xTitle;

            if (bool.TryParse (Shared.AppSpecificConfig ["IsWindowMaximized"], out bool xIsMaximized) && xIsMaximized)
                WindowState = WindowState.Maximized;

            string? xFontFamily = Shared.AppSpecificConfig ["FontFamily"];

            if (string.IsNullOrWhiteSpace (xFontFamily) == false)
                FontFamily = new (xFontFamily);

            if (double.TryParse (Shared.AppSpecificConfig ["ContentFontSize"], out double xContentFontSize))
            {
                Body.FontSize = xContentFontSize;
                TranslatedBody.FontSize = xContentFontSize;
            }
        }
    }
}
