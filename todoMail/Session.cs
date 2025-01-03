﻿using System;
using System.IO;
using System.Text;
using System.Text.Json;
using yyLib;

namespace todoMail
{
    public static class Session
    {
        private static readonly Lazy <yyMailContact?> _sender = new (() =>
        {
            string xSenderInfoFilePath = yyAppDirectory.MapPath ("Sender.json");

            if (File.Exists (xSenderInfoFilePath) == false)
            {
                var xSender = new yyMailContact
                {
                    Name = "NAME",
                    Address = "ADDRESS"
                };

                File.WriteAllText (xSenderInfoFilePath, JsonSerializer.Serialize (xSender, yyJson.DefaultSerializationOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailContact> (File.ReadAllText (xSenderInfoFilePath, Encoding.UTF8), yyJson.DefaultDeserializationOptions);
        });

        public static yyMailContact? Sender => _sender.Value;

        private static readonly Lazy <string?> _senderString = new (() =>
        {
            if (Sender == null)
                return null;

            if (string.IsNullOrWhiteSpace (Sender.Name) == false)
                return $"{Sender.Name} <{Sender.Address}>";

            else return Sender.Address;
        });

        public static string? SenderString => _senderString.Value;

        private static readonly Lazy <yyMailContact?> _recipient = new (() =>
        {
            string xRecipientInfoFilePath = yyAppDirectory.MapPath ("Recipient.json");

            if (File.Exists (xRecipientInfoFilePath) == false)
            {
                var xRecipient = new yyMailContact
                {
                    Name = "NAME",
                    Address = "ADDRESS",
                    PreferredLanguages = ["English"]
                };

                File.WriteAllText (xRecipientInfoFilePath, JsonSerializer.Serialize (xRecipient, yyJson.DefaultSerializationOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailContact> (File.ReadAllText (xRecipientInfoFilePath, Encoding.UTF8), yyJson.DefaultDeserializationOptions);
        });

        public static yyMailContact? Recipient => _recipient.Value;

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

        private static readonly Lazy <yyGptChatConnectionInfo?> _gptChatConnectionInfo = new (() =>
        {
            string xGptChatConnectionInfoFilePath = yyAppDirectory.MapPath ("GptChatConnection.json");

            if (File.Exists (xGptChatConnectionInfoFilePath) == false)
            {
                var xGptChatConnectionInfo = new yyGptChatConnectionInfo
                {
                    // We cant provide a default API key.
                    // A dummy value is initially contained in the newly generated JSON file.
                    // The user may: 1) Replace it with a valid key,
                    //     or 2) Remove the corresponding entry from the file to use the one that MAY be in the .yyUserSecrets.json file IF it exists and contains the key and a value.
                    ApiKey = "API_KEY",

                    // Rather than a dummy value, the value in the .yyUserSecrets.json file (IF it's available) or the embedded-in-code default value is initially output.
                    // The user may: 1) Replace the one in GptChatConnection.json, or 2) Remove the key to use whatever yyGptChat.Default* may return.

                    Organization = yyGpt.DefaultOrganization,
                    Project = yyGpt.DefaultProject,
                    Endpoint = yyGptChat.DefaultEndpoint,
                    Timeout = yyGpt.DefaultTimeout

                    // The code above is merely initialization of the JSON file.
                    // The loading and usage of alternative values are done about 10 lines later.
                };

                File.WriteAllText (xGptChatConnectionInfoFilePath, JsonSerializer.Serialize (xGptChatConnectionInfo, yyJson.DefaultSerializationOptions), Encoding.UTF8);

                return null;
            }

            var xConnectionInfo = JsonSerializer.Deserialize <yyGptChatConnectionInfo> (File.ReadAllText (xGptChatConnectionInfoFilePath, Encoding.UTF8), yyJson.DefaultDeserializationOptions);

            if (xConnectionInfo != null)
            {
                if (string.IsNullOrWhiteSpace (xConnectionInfo.ApiKey))
                    xConnectionInfo.ApiKey = yyGpt.DefaultApiKey;

                if (string.IsNullOrWhiteSpace (xConnectionInfo.Organization))
                    xConnectionInfo.Organization = yyGpt.DefaultOrganization;

                if (string.IsNullOrWhiteSpace (xConnectionInfo.Project))
                    xConnectionInfo.Project = yyGpt.DefaultProject;

                if (string.IsNullOrWhiteSpace (xConnectionInfo.Endpoint))
                    xConnectionInfo.Endpoint = yyGptChat.DefaultEndpoint;

                if (xConnectionInfo.Timeout == null)
                    xConnectionInfo.Timeout = yyGpt.DefaultTimeout;
            }

            return xConnectionInfo;
        });

        public static yyGptChatConnectionInfo? GptChatConnectionInfo => _gptChatConnectionInfo.Value;

        private static readonly Lazy <yyGptChatConversation> _conversationForSubject = new (() =>
        {
            yyGptChatConversation xConversation = new (GptChatConnectionInfo!);

            // The choice of model is not exactly connection-related and is exclusively stored in appsettings.json.
            // If the file contains a valid value, it's used.
            // If not, as usual, the .yyUserSecrets.json file is first checked and, if it doesnt exist or doesnt contain the corresponding value, the embedded-in-code default value is used.
            xConversation.Request.Model = Shared.AppSpecificConfig ["OpenAiChatModel"].WhiteSpaceToNull () ?? yyGptChat.DefaultModel;

            xConversation.Request.AddMessage (yyGptChatMessageRole.System, "You are a helpful assistant.");

            return xConversation;
        });

        public static yyGptChatConversation ConversationForSubject => _conversationForSubject.Value;

        // Lazy coding.
        // The app will use 2 threads to translate the subject and body at the same time.

        private static readonly Lazy <yyGptChatConversation> _conversationForBody = new (() =>
        {
            yyGptChatConversation xConversation = new (GptChatConnectionInfo!);

            xConversation.Request.Model = Shared.AppSpecificConfig ["OpenAiChatModel"].WhiteSpaceToNull () ?? yyGptChat.DefaultModel;

            xConversation.Request.AddMessage (yyGptChatMessageRole.System, "You are a helpful assistant.");

            return xConversation;
        });

        public static yyGptChatConversation ConversationForBody => _conversationForBody.Value;

        // -----------------------------------------------------------------------------

        // Things related to mail sending:

        private static readonly Lazy <yyMailConnectionInfo?> _mailConnectionInfo = new (() =>
        {
            string xMailConnectionInfoFilePath = yyAppDirectory.MapPath ("MailConnection.json");

            if (File.Exists (xMailConnectionInfoFilePath) == false)
            {
                var xMailConnectionInfo = new yyMailConnectionInfo
                {
                    Host = "HOST",
                    Port = 587,
                    UserName = "USERNAME",
                    Password = "PASSWORD"
                };

                File.WriteAllText (xMailConnectionInfoFilePath, JsonSerializer.Serialize (xMailConnectionInfo, yyJson.DefaultSerializationOptions), Encoding.UTF8);

                return null;
            }

            return JsonSerializer.Deserialize <yyMailConnectionInfo> (File.ReadAllText (xMailConnectionInfoFilePath, Encoding.UTF8), yyJson.DefaultDeserializationOptions);
        });

        public static yyMailConnectionInfo? MailConnectionInfo => _mailConnectionInfo.Value;

        public static void Cleanup () // May be unnecessary, but harmless.
        {
            if (_conversationForSubject.IsValueCreated)
                _conversationForSubject.Value.Dispose ();

            if (_conversationForBody.IsValueCreated)
                _conversationForBody.Value.Dispose ();
        }
    }
}
