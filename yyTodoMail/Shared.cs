using System;
using Microsoft.Extensions.Configuration;
using yyLib;

namespace yyTodoMail
{
    public static class Shared
    {
        // GetSection never returns null.

        // IConfiguration.GetSection(String) Method (Microsoft.Extensions.Configuration) | Microsoft Learn
        // https://learn.microsoft.com/ja-jp/dotnet/api/microsoft.extensions.configuration.iconfiguration.getsection

        private static readonly Lazy <IConfigurationSection> _appSpecificConfig = new (() => yyAppSettings.Config.GetSection ("AppSpecific"));

        public static IConfigurationSection AppSpecificConfig => _appSpecificConfig.Value;

        private static readonly Lazy <string> _themeVariant = new (() =>
        {
            string? xThemeVariant = AppSpecificConfig ["ThemeVariant"];

            if (string.Equals (xThemeVariant, "Light", StringComparison.OrdinalIgnoreCase))
                return "Light";

            if (string.Equals (xThemeVariant, "Dark", StringComparison.OrdinalIgnoreCase))
                return "Dark";

            return "Default";
        });

        public static string ThemeVariant => _themeVariant.Value;
    }
}
