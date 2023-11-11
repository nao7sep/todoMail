using System;
using System.Globalization;
using System.IO;
using MimeKit;
using yyLib;

namespace yyTodoMailSenderWpf
{
    public static class MailStorage
    {
        private readonly static Lazy <string> _mailStorageDirectoryPath = new (() => yyApplicationDirectory.MapPath ("MailStorage"));

        public static string MailStorageDirectoryPath => _mailStorageDirectoryPath.Value;

        public static void Store (MimeMessage message)
        {
            if (Directory.Exists (MailStorageDirectoryPath) == false)
                Directory.CreateDirectory (MailStorageDirectoryPath);

            string xFilePath = Path.Join (MailStorageDirectoryPath, message.Date.ToUniversalTime ().ToString ("yyyyMMdd'T'HHmmss'Z.eml'", CultureInfo.InvariantCulture));
            message.WriteTo (xFilePath);
        }
    }
}
