using System;
using System.Globalization;
using System.IO;
using MimeKit;
using yyLib;

namespace todoMail
{
    public static class MailStorage
    {
        private static readonly Lazy <string> _mailStorageDirectoryPath = new (() => yyAppDirectory.MapPath ("MailStorage"));

        public static string MailStorageDirectoryPath => _mailStorageDirectoryPath.Value;

        public static void Store (MimeMessage message)
        {
            if (Directory.Exists (MailStorageDirectoryPath) == false)
                Directory.CreateDirectory (MailStorageDirectoryPath);

            string xFilePath = Path.Join (MailStorageDirectoryPath, message.Date.ToUniversalTime ().ToString ("'Message-'yyyyMMdd'T'HHmmss'Z.eml'", CultureInfo.InvariantCulture));
            message.WriteTo (xFilePath);
        }
    }
}
