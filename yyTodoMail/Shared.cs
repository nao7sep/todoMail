using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using yyLib;

namespace yyTodoMail
{
    public static class Shared
    {
        public static void ShowExceptionMessageBox (Window? owner, Exception exception) =>
            MessageBox.Show (owner, exception.ToString (), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);

        // GetSection never returns null.

        // IConfiguration.GetSection(String) Method (Microsoft.Extensions.Configuration) | Microsoft Learn
        // https://learn.microsoft.com/ja-jp/dotnet/api/microsoft.extensions.configuration.iconfiguration.getsection

        private static Lazy <IConfigurationSection> _appSpecificConfig = new (() => yyAppSettings.Config.GetSection ("AppSpecific"));

        public static IConfigurationSection AppSpecificConfig => _appSpecificConfig.Value;
    }
}
