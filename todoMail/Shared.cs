using System;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
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

        public static (string Subject, string Body) GetSubjectAndBody (string? subject, string? body, string? translatedSubject, string? translatedBody)
        {
            string xSubject = (subject ?? string.Empty).Trim (),
                xBody = yyString.TrimRedundantLines (body ?? string.Empty)!,
                xTranslatedSubject = (translatedSubject ?? string.Empty).Trim (),
                xTranslatedBody = yyString.TrimRedundantLines (translatedBody ?? string.Empty)!;

            bool xHasTranslation = string.IsNullOrWhiteSpace (xTranslatedSubject) == false || string.IsNullOrWhiteSpace (xTranslatedBody) == false;

            StringBuilder xArnold = new ();

            if (xHasTranslation == false)
            {
                if (string.IsNullOrWhiteSpace (xBody) == false)
                    xArnold.AppendLine (xBody);
            }

            else
            {
                void AppendBorderlineIfNecessary ()
                {
                    if (xArnold.Length > 0)
                        xArnold.AppendLine ("----");
                }

                if (string.IsNullOrWhiteSpace (xTranslatedBody) == false)
                    xArnold.AppendLine (xTranslatedBody);

                if (string.IsNullOrWhiteSpace (xSubject) == false)
                {
                    AppendBorderlineIfNecessary ();
                    xArnold.AppendLine (xSubject);
                }

                if (string.IsNullOrWhiteSpace (xBody) == false)
                {
                    AppendBorderlineIfNecessary ();
                    xArnold.AppendLine (xBody);
                }
            }

            return
            (
                Subject: xHasTranslation == false ? xSubject : xTranslatedSubject,
                Body: xArnold.ToString ()
            );
        }

        public static bool SendMessage (string subject, string body)
        {
            try
            {
                using MimeMessage xMessage = new ();
                xMessage.From.Add (new MailboxAddress (Session.Sender!.Name, Session.Sender!.Address));
                xMessage.To.Add (new MailboxAddress (Session.Recipient!.Name, Session.Recipient!.Address));
                xMessage.Subject = $"[TODO] {subject}".TrimEnd ();
                xMessage.Body = new TextPart ("plain") { Text = body };

                using SmtpClient xClient = new ();
                xClient.ConnectAsync (Session.MailConnectionInfo!).Wait ();
                xClient.AuthenticateAsync (Session.MailConnectionInfo!).Wait ();
                xClient.SendAsync (xMessage).Wait ();

                MailStorage.Store (xMessage);

                return true;
            }

            catch (Exception xException)
            {
                yyLogger.Default.TryWriteException (xException);
                MessageBox.ShowException (null, xException);
                return false;
            }
        }
    }
}
