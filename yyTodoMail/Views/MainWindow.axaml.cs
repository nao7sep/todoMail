using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using yyLib;
using yyTodoMail.ViewModels;

namespace yyTodoMail.Views;

public partial class MainWindow: Window
{
    public MainWindow ()
    {
        InitializeComponent ();

        Opened += (sender, e) =>
        {
            try
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
                    BodyTextBox.FontSize = xContentFontSize;
                    TranslatedBodyTextBox.FontSize = xContentFontSize;
                }

                // -----------------------------------------------------------------------------

                SenderTextBox.Text = Session.SenderString;
                RecipientTextBox.Text = Session.RecipientString;

                // -----------------------------------------------------------------------------

                BodyTextBox.Focus ();
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                MessageBox.ShowException (this, xException);
            }
        };

        async void OnClosing (object? sender, WindowClosingEventArgs e)
        {
            try
            {
                if (DataContext is MainWindowViewModel xViewModel)
                {
                    if (xViewModel.HasSubjectOrBody || xViewModel.HasTranslation)
                    {
                        // When ShowDialog is called, it gets back to the caller immediately.
                        // We need to keep the window open so that we can wait for the task associated with the dialog to complete,
                        //     only after which we can get the actual result of the dialog.
                        // Theoretically, the app goes back to its normal state for a few milliseconds or so, waiting for new events to occur,
                        //     before the task completes, the result is examined and the window is closed if necessary.

                        e.Cancel = true;

                        MessageBoxWindow xWindow = new ()
                        {
                            DataContext = new MessageBoxWindowViewModel
                            {
                                Caption = "Confirmation",
                                Message = "Are you sure you want to close the window?",
                                IsSecondButtonVisible = true
                            },

                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };

                        if (await xWindow.ShowDialog <int> (this) == 1)
                        {
                            Closing -= OnClosing;
                            Close ();
                        }
                    }
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                MessageBox.ShowException (this, xException);
            }
        }

        Closing += OnClosing;
    }

    private void CloseButtonClicked (object? sender, RoutedEventArgs e)
    {
        try
        {
            Close ();
        }

        catch (Exception xException)
        {
            yySimpleLogger.Default.TryWriteException (xException);
            MessageBox.ShowException (this, xException);
        }
    }
}
