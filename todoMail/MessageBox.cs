using System;
using Avalonia.Controls;
using Avalonia.Threading;
using todoMail.ViewModels;
using todoMail.Views;

namespace todoMail
{
    public static class MessageBox
    {
        // It's not considered a good practice to show an owner-less message box especially in the code-behind of the app class,
        //     but this is a small project and I have more important things to work on (meaning I'm just lazy).

        public static void Show (Window? owner, string caption, string message, bool isSecondButtonVisible)
        {
            // Checks if the code is running in the UI thread.
            // If it is and we dont need a result, we can fire and forget the window; the parent window will anyway be inoperative.
            // If we need a result, we can use "await" in an asynchronous command or event handler and use the result when it comes out.
            // If it's running in a thread other than the UI thread, the following code should work.

            if (Dispatcher.UIThread.CheckAccess ())
            {
                // Originally, the code to generate the window and the data context was before the call to CheckAccess.
                // Then, I got an exception with the message "Call from invalid thread" when I Task.Run-ed some code, in which a message box was shown.
                // Generation of a window highly likely interacts with UI-related things as it requires to refer to styles, resources, etc.
                // View models, on the other hand, MAY be safer to generate in a non-UI thread,
                //     but we dont need to take the risk considering view models in Avalonia UI inherit from ReactiveObject, whose implementation may be changed in the future.
                // https://github.com/reactiveui/ReactiveUI/blob/main/src/ReactiveUI/ReactiveObject/ReactiveObject.cs

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
                    // Added comment: If an exception is thrown within the task, its info will be stored silently in the task.
                    // For something more complex than this simple yes/no dialog, we should worry about exceptions.
                }

                else
                {
                    xWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    xWindow.Show ();
                }
            }

            else
            {
                Dispatcher.UIThread.InvokeAsync (() =>
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
                        xWindow.ShowDialog (owner);
                    }

                    else
                    {
                        xWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        xWindow.Show ();
                    }
                });
            }
        }

        public static void ShowException (Window? owner, Exception exception) => Show (owner, "Exception", exception.ToString ().TrimEnd (), isSecondButtonVisible: false);
    }
}
