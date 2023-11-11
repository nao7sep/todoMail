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
                SetInitialFocus ();
            }

            catch (Exception xException)
            {
                SimpleLogger.LogException (xException);
            }
        }

        private void Subject_TextChanged (object sender, System.Windows.Controls.TextChangedEventArgs e)
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

        private void Body_TextChanged (object sender, System.Windows.Controls.TextChangedEventArgs e)
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
