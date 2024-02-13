using System;
using Avalonia.Controls;
using yyLib;

namespace yyTodoMail
{
    public static class MessageBox
    {
        // It's not considered a good practice to show an owner-less message box especially in the code-behind of the app class,
        //     but this is a small project and I have more important things to work on (meaning I'm just lazy).

        public static MessageBoxResult Show (Window? owner, string messageBoxText, string? caption = null, MessageBoxButton button = MessageBoxButton.OK, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            // todo
            yySimpleLogger.Default.TryWriteMessage ("MessageBox.Show: " + messageBoxText);
            return MessageBoxResult.None;
        }

        public static MessageBoxResult ShowException (Window? owner, Exception exception) => Show (owner, exception.ToString ().TrimEnd (), "Exception");
    }
}
