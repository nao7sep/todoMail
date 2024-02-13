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
    }
}
