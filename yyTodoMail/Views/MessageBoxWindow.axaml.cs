using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using yyLib;
using yyTodoMail.ViewModels;

namespace yyTodoMail.Views;

public partial class MessageBoxWindow: Window
{
    public MessageBoxWindow ()
    {
        InitializeComponent ();

        Opened += (sender, e) =>
        {
            try
            {
                if (DataContext is MessageBoxWindowViewModel xViewModel)
                {
                    // I dont want to close message boxes by pressing the enter key accidentally,
                    //     but when I'm closing a window, I sometimes want to do so only by the keyboard.

                    if (xViewModel.IsSecondButtonVisible)
                        SecondButton.Focus ();
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                MessageBox.ShowException (null, xException);
            }
        };
    }

    private void FirstButtonClicked (object? sender, RoutedEventArgs e)
    {
        try
        {
            // If the window is closed by the window's close button, the result should be null.
            Close (1);
        }

        catch (Exception xException)
        {
            yySimpleLogger.Default.TryWriteException (xException);
            MessageBox.ShowException (null, xException);
        }
    }

    private void SecondButtonClicked (object? sender, RoutedEventArgs e)
    {
        try
        {
            Close (2);
        }

        catch (Exception xException)
        {
            yySimpleLogger.Default.TryWriteException (xException);
            MessageBox.ShowException (null, xException);
        }
    }
}
