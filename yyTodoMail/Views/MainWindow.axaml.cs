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

        Closing += (sender, e) =>
        {
            try
            {
                if (DataContext is MainWindowViewModel xViewModel)
                {
                    if (xViewModel.HasSubjectOrBody || xViewModel.HasTranslation)
                    {
                        if (MessageBox.Show (this, "Are you sure you want to close the window?", "Confirmation",
                                MessageBoxButton.OKCancel, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                            e.Cancel = true;
                    }
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                MessageBox.ShowException (this, xException);
            }
        };
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
