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

        // I once wrote code to set the focus to the second button if it's visible, and it did work,
        //     but there wasnt much point as arrow keys didnt work to navigate between the buttons like in WPF.
        // Opened += (sender, e) =>
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
