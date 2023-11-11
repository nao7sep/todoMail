using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using yyGptLib;

namespace yyTodoMailSenderWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        public MainWindow () => InitializeComponent ();

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateTextOfSenderAndRecipient ();
                UpdateIsEnabledOfSendAndTranslate ();
                UpdateIsEnabledOfSendTranslated ();
                SetInitialFocus ();
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void Subject_TextChanged (object sender, TextChangedEventArgs e)
        {
            try
            {
                UpdateIsEnabledOfSendAndTranslate ();
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void Body_TextChanged (object sender, TextChangedEventArgs e)
        {
            try
            {
                UpdateIsEnabledOfSendAndTranslate ();
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void Send_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run (() =>
                {
                    WindowAlt.Dispatcher.Invoke (() =>
                    {
                        Subject.IsEnabled = false;
                        Body.IsEnabled = false;

                        Send.IsEnabled = false;
                        Translate.IsEnabled = false;

                        SendTranslated.IsEnabled = false;
                    });

                    SendMessage (Subject, Body);

                    WindowAlt.Dispatcher.Invoke (() =>
                    {
                        Subject.Clear ();
                        Body.Clear ();

                        Subject.IsEnabled = true;
                        Body.IsEnabled = true;

                        Send.IsEnabled = true;
                        Translate.IsEnabled = true;

                        TranslatedSubject.Clear ();
                        TranslatedBody.Clear ();

                        // Stays down.
                        // SendTranslated.IsEnabled = true;
                    });
                });
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void Translate_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run (() =>
                {
                    WindowAlt.Dispatcher.Invoke (() =>
                    {
                        Subject.IsEnabled = false;
                        Body.IsEnabled = false;

                        Send.IsEnabled = false;
                        Translate.IsEnabled = false;

                        TranslatedSubject.Clear ();
                        TranslatedBody.Clear ();

                        SendTranslated.IsEnabled = false;
                    });

                    TranslateAlt (Subject, TranslatedSubject);
                    TranslateAlt (Body, TranslatedBody);

                    // If the button could be pressed, at least one thing was translated.

                    WindowAlt.Dispatcher.Invoke (() =>
                    {
                        Subject.IsEnabled = true;
                        Body.IsEnabled = true;

                        Send.IsEnabled = true;
                        Translate.IsEnabled = true;

                        SendTranslated.IsEnabled = true;
                    });
                });
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void SendTranslated_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run (() =>
                {
                    WindowAlt.Dispatcher.Invoke (() =>
                    {
                        Subject.IsEnabled = false;
                        Body.IsEnabled = false;

                        Send.IsEnabled = false;
                        Translate.IsEnabled = false;

                        SendTranslated.IsEnabled = false;
                    });

                    SendMessage (TranslatedSubject, TranslatedBody);

                    WindowAlt.Dispatcher.Invoke (() =>
                    {
                        Subject.Clear ();
                        Body.Clear ();

                        Subject.IsEnabled = true;
                        Body.IsEnabled = true;

                        Send.IsEnabled = true;
                        Translate.IsEnabled = true;

                        TranslatedSubject.Clear ();
                        TranslatedBody.Clear ();

                        // Stays down.
                        // SendTranslated.IsEnabled = true;
                    });
                });
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void Close_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                Close ();
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (IsEditing)
                {
                    MessageBoxResult xResult = MessageBox.Show (this, "Are you sure you want to close this window?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                    if (xResult == MessageBoxResult.No)
                        e.Cancel = true;
                }
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }
    }
}
