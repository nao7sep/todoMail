using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using yyGptLib;
using yyLib;
using yyMailLib;

namespace yyTodoMailSenderWpf
{
    public partial class App: Application
    {
        // Things related to the sender and recipient:

        public static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        private static Lazy <yyMailContactModel?> _sender = new Lazy <yyMailContactModel?> (() =>
        {
            string xSenderInfoFilePath = yyApplicationDirectory.MapPath ("Sender.json");

            if (File.Exists (xSenderInfoFilePath) == false)
            {
                var xSender = new yyMailContactModel
                {
                    Name = "NAME",
                    Address = "ADDRESS"
                };

                File.WriteAllText (xSenderInfoFilePath, JsonSerializer.Serialize (xSender, JsonSerializerOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailContactModel> (File.ReadAllText (xSenderInfoFilePath, Encoding.UTF8), JsonSerializerOptions);
        });

        public static yyMailContactModel? Sender => _sender.Value;

        private static Lazy <string?> _senderString = new Lazy <string?> (() =>
        {
            if (Sender == null)
                return null;

            if (string.IsNullOrWhiteSpace (Sender.Name) == false)
                return $"{Sender.Name} <{Sender.Address}>";

            else return Sender.Address;
        });

        public static string? SenderString => _senderString.Value;

        private static Lazy <yyMailContactModel?> _recipient = new Lazy <yyMailContactModel?> (() =>
        {
            string xRecipientInfoFilePath = yyApplicationDirectory.MapPath ("Recipient.json");

            if (File.Exists (xRecipientInfoFilePath) == false)
            {
                var xRecipient = new yyMailContactModel
                {
                    Name = "NAME",
                    Address = "ADDRESS",
                    PreferredLanguages = new [] { "English" }
                };

                File.WriteAllText (xRecipientInfoFilePath, JsonSerializer.Serialize (xRecipient, JsonSerializerOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailContactModel> (File.ReadAllText (xRecipientInfoFilePath, Encoding.UTF8), JsonSerializerOptions);
        });

        public static yyMailContactModel? Recipient => _recipient.Value;

        private static Lazy <string?> _recipientString = new Lazy <string?> (() =>
        {
            if (Recipient == null)
                return null;

            if (string.IsNullOrWhiteSpace (Recipient.Name) == false)
                return $"{Recipient.Name} <{Recipient.Address}>";

            else return Recipient.Address;
        });

        public static string? RecipientString => _recipientString.Value;

        // -----------------------------------------------------------------------------

        // Things related to translation:

        private static Lazy <yyGptChatConnectionInfoModel?> _gptChatConnectionInfo = new Lazy <yyGptChatConnectionInfoModel?> (() =>
        {
            string xGptChatConnectionInfoFilePath = yyApplicationDirectory.MapPath ("GptChatConnection.json");

            if (File.Exists (xGptChatConnectionInfoFilePath) == false)
            {
                var xGptChatConnectionInfo = new yyGptChatConnectionInfoModel
                {
                    ApiKey = "API_KEY",
                    Endpoint = yyGptChatDefaultValues.Endpoint
                };

                File.WriteAllText (xGptChatConnectionInfoFilePath, JsonSerializer.Serialize (xGptChatConnectionInfo, JsonSerializerOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyGptChatConnectionInfoModel> (File.ReadAllText (xGptChatConnectionInfoFilePath, Encoding.UTF8), JsonSerializerOptions);
        });

        public static yyGptChatConnectionInfoModel? GptChatConnectionInfo => _gptChatConnectionInfo.Value;

        private static Lazy <yyGptChatConversation> _conversation = new Lazy <yyGptChatConversation> (() =>
        {
            yyGptChatConversation xConversation = new (GptChatConnectionInfo!);
            xConversation.Request.Model = "gpt-4";
            xConversation.Request.AddMessage (yyGptChatMessageRole.System, "You are a helpful assistant.");
            return xConversation;
        });

        public static yyGptChatConversation Conversation => _conversation.Value;

        // -----------------------------------------------------------------------------

        // Things related to mail sending:

        private static Lazy <yyMailConnectionInfoModel?> _mailConnectionInfo = new Lazy <yyMailConnectionInfoModel?> (() =>
        {
            string xMailConnectionInfoFilePath = yyApplicationDirectory.MapPath ("MailConnection.json");

            if (File.Exists (xMailConnectionInfoFilePath) == false)
            {
                var xMailConnectionInfo = new yyMailConnectionInfoModel
                {
                    Host = "HOST",
                    Port = 587,
                    UserName = "USERNAME",
                    Password = "PASSWORD"
                };

                File.WriteAllText (xMailConnectionInfoFilePath, JsonSerializer.Serialize (xMailConnectionInfo, JsonSerializerOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailConnectionInfoModel> (File.ReadAllText (xMailConnectionInfoFilePath, Encoding.UTF8), JsonSerializerOptions);
        });

        public static yyMailConnectionInfoModel? MailConnectionInfo => _mailConnectionInfo.Value;
    }
}
