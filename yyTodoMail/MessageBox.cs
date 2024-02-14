using System;
using Avalonia.Controls;
using yyTodoMail.ViewModels;
using yyTodoMail.Views;

namespace yyTodoMail
{
    public static class MessageBox
    {
        // It's not considered a good practice to show an owner-less message box especially in the code-behind of the app class,
        //     but this is a small project and I have more important things to work on (meaning I'm just lazy).

        public static void Show (Window? owner, string caption, string message, bool isSecondButtonVisible)
        {
            MessageBoxWindow xWindow = new ()
            {
                DataContext = new MessageBoxWindowViewModel
                {
                    Caption = caption,
                    Message = message,
                    IsSecondButtonVisible = isSecondButtonVisible
                }
            };

            if (owner != null)
            {
                xWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                xWindow.ShowDialog (owner); // Returns a task that completes when the window is closed.
                // For more information, refer to the Closing event's implementation in MainWindow.axaml.cs.
            }

            else
            {
                xWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                xWindow.Show ();
            }
        }

        public static void ShowException (Window? owner, Exception exception) => Show (owner, "Exception", exception.ToString ().TrimEnd (), isSecondButtonVisible: false);
    }
}
