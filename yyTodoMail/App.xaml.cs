using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using yyGptLib;
using yyLib;
using yyMailLib;

namespace yyTodoMail
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App: Application
    {
        private static readonly Lazy <yyMailContactModel?> _sender = new (() =>
        {
            string xSenderInfoFilePath = yyApplicationDirectory.MapPath ("Sender.json");

            if (File.Exists (xSenderInfoFilePath) == false)
            {
                var xSender = new yyMailContactModel
                {
                    Name = "NAME",
                    Address = "ADDRESS"
                };

                File.WriteAllText (xSenderInfoFilePath, JsonSerializer.Serialize (xSender, yyJson.DefaultSerializationOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailContactModel> (File.ReadAllText (xSenderInfoFilePath, Encoding.UTF8), yyJson.DefaultDeserializationOptions);
        });

        public static yyMailContactModel? Sender => _sender.Value;

        private static readonly Lazy <string?> _senderString = new (() =>
        {
            if (Sender == null)
                return null;

            if (string.IsNullOrWhiteSpace (Sender.Name) == false)
                return $"{Sender.Name} <{Sender.Address}>";

            else return Sender.Address;
        });

        public static string? SenderString => _senderString.Value;

        private static readonly Lazy <yyMailContactModel?> _recipient = new (() =>
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

                File.WriteAllText (xRecipientInfoFilePath, JsonSerializer.Serialize (xRecipient, yyJson.DefaultSerializationOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailContactModel> (File.ReadAllText (xRecipientInfoFilePath, Encoding.UTF8), yyJson.DefaultDeserializationOptions);
        });

        public static yyMailContactModel? Recipient => _recipient.Value;

        private static readonly Lazy <string?> _recipientString = new (() =>
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

        private static readonly Lazy <yyGptChatConnectionInfoModel?> _gptChatConnectionInfo = new (() =>
        {
            string xGptChatConnectionInfoFilePath = yyApplicationDirectory.MapPath ("GptChatConnection.json");

            if (File.Exists (xGptChatConnectionInfoFilePath) == false)
            {
                var xGptChatConnectionInfo = new yyGptChatConnectionInfoModel
                {
                    ApiKey = "API_KEY",
                    Endpoint = yyGptChat.DefaultEndpoint
                };

                File.WriteAllText (xGptChatConnectionInfoFilePath, JsonSerializer.Serialize (xGptChatConnectionInfo, yyJson.DefaultSerializationOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyGptChatConnectionInfoModel> (File.ReadAllText (xGptChatConnectionInfoFilePath, Encoding.UTF8), yyJson.DefaultDeserializationOptions);
        });

        public static yyGptChatConnectionInfoModel? GptChatConnectionInfo => _gptChatConnectionInfo.Value;

        private static readonly Lazy <yyGptChatConversation> _conversation = new (() =>
        {
            yyGptChatConversation xConversation = new (GptChatConnectionInfo!);
            xConversation.Request.Model = "gpt-4";
            xConversation.Request.AddMessage (yyGptChatMessageRole.System, "You are a helpful assistant.");
            return xConversation;
        });

        public static yyGptChatConversation Conversation => _conversation.Value;

        // -----------------------------------------------------------------------------

        // Things related to mail sending:

        private static readonly Lazy <yyMailConnectionInfoModel?> _mailConnectionInfo = new (() =>
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

                File.WriteAllText (xMailConnectionInfoFilePath, JsonSerializer.Serialize (xMailConnectionInfo, yyJson.DefaultSerializationOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailConnectionInfoModel> (File.ReadAllText (xMailConnectionInfoFilePath, Encoding.UTF8), yyJson.DefaultDeserializationOptions);
        });

        public static yyMailConnectionInfoModel? MailConnectionInfo => _mailConnectionInfo.Value;

        private void Application_Startup (object sender, StartupEventArgs e)
        {
            try
            {
                // If we directly check Sender and the others, the program will exit for each newly generated file,
                // forcing us to restart it a few times until the configuration files are all generated.

                var xSender = Sender;
                var xRecipient = Recipient;
                var xGptChatConnectionInfo = GptChatConnectionInfo;
                var xMailConnectionInfo = MailConnectionInfo;

                if (xSender == null || xRecipient == null || xGptChatConnectionInfo == null || xMailConnectionInfo == null)
                {
                    MessageBox.Show ("Please check the JSON configuration files located in the same directory as this application.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown ();
                    return;
                }
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
            }
        }

        private static void Cleanup () => Conversation.Dispose (); // May be unnecessary, but harmless.

        private void Application_Exit (object sender, ExitEventArgs e) // App closing.
        {
            try
            {
                Cleanup ();
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
            }
        }

        private void Application_SessionEnding (object sender, SessionEndingCancelEventArgs e) // User logging off from Windows.
        {
            try
            {
                Cleanup ();
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
            }
        }
    }
}
