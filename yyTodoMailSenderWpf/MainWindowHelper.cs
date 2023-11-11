using System;
using System.Windows;
using System.Windows.Controls;
using yyGptLib;

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

        private bool IsEditing => string.IsNullOrWhiteSpace (Subject.Text) == false || string.IsNullOrWhiteSpace (Body.Text) == false;

        private void SetInitialFocus () => Subject.Focus ();

        private void TranslateAlt (TextBox sourceControl, TextBox targetControl)
        {
            try
            {
                // Initialized to avoid compiler warning.
                string xOriginalText = string.Empty;

                // ChatGPT says:

                    // Dispatcher.Invoke in WPF synchronously executes code on the UI thread from a background thread.
                    // It blocks the calling thread until the UI thread has processed and completed the code within the delegate.
                    // This ensures that operations within Invoke are completed before the background thread proceeds.
                    // Use this method judiciously to prevent deadlocks and maintain application responsiveness.
                    // For non-blocking calls, consider using Dispatcher.BeginInvoke, which executes asynchronously.

                // The reason why the compiler warns me about xOriginalText being used without initialization is unclear.
                // Maybe the IDE cant (always) track operations on variables in other threads.

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

        private void UpdateTextOfSenderAndRecipient ()
        {
            Sender.Text = App.SenderString;
            Recipient.Text = App.RecipientString;
        }
    }
}
