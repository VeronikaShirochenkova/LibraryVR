using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;

namespace chatGPT
{
    public class OpenAIController : MonoBehaviour
    {
        public TMP_Text textBox;
        
        private List<ChatMessage> messages;

        private OpenAIAPI api;
        private APIAuthentication key = new APIAuthentication("");
        
        void Start()
        {
            api = new OpenAIAPI(key);
            StartConversation();
        }

        private void StartConversation()
        {
            ChatMessage startMessage = new ChatMessage(ChatMessageRole.System, "You'll get a piece of text. You need to retell it as briefly as possible. Your answer should not contain anything other than a paraphrase.");
            messages = new List<ChatMessage> { startMessage };
        }

        public async void GetResponse(string text)
        {
            if (text.Length < 1) return;

            ChatMessage userMessage = new ChatMessage(ChatMessageRole.User, text);


            if (userMessage.Content.Length > 600)
            {
                // Limit messages to 600 characters
                userMessage.Content = userMessage.Content.Substring(0, 600);
            }
            
            messages.Add(userMessage);
            
            // send the entire chat to OpenAI to get the next message
            var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.3,
                MaxTokens = 150,
                Messages = messages
            });
            
            // Get the response message
            ChatMessage responseMessage = new ChatMessage(chatResult.Choices[0].Message.Role, chatResult.Choices[0].Message.Content);

            // Add the response to the list of messages
            messages.Add(responseMessage);
            
            // Update the text field with response
            textBox.text = responseMessage.Content;

        }
    }
}
