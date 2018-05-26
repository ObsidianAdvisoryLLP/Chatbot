using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using AdaptiveCards;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            //  var legal_case = "Ten years ago, my husband and I started a company and its business developed pretty well. However, since 2013, we grew conflict and seperated since then. Beforehead, the expense for me or the children are paied by my hubsband and genrally not affordable for my own. Howerer, after our seperation, this case was cut. For now I can still handle the scenario with my deposit, yet not for the future. So I want him to still hold the expense responsibility";

            var activity = (Activity)await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            else if (message.Text == "Yes")
            {
                await context.PostAsync($"We can do the registration now.");
                await context.PostAsync($"What is your name?");
                context.Wait(MessageReceivedAsync);
            }
            else if (message.Text == "Charles")
            {
                await context.PostAsync($"Charles! What is your phone number?");
                context.Wait(MessageReceivedAsync);
            }
            else if (message.Text == "61234567")
            {
                await context.PostAsync($"What is your email?");
                context.Wait(MessageReceivedAsync);
            }
            /*
            else if(message.Text == "Address Proof uploaded"){
                await context.PostAsync($"Received. Now maybe you would like to upload the legal case document :)  (you can upload multiple images and I will choose the last one, please type 'Document uploaded' when you finish)");
                context.Wait(MessageReceivedAsync);
            }
            */
            else if (message.Text == "charles@gmail.com" || message.Text == "No")
            {
               
                /*
                await context.PostAsync($"OK, we perceived it is a Civil Cases. Let me find you the best lawyer.");
                await context.PostAsync($"Here he is: Mr. Lam. His available time is 4pm-5pm in 25/F Queensway Government Offices. Are you available in this timeslot? If yes, I can help you to make an appointment now.  ");

                var replyMessage = context.MakeMessage();

                Attachment attachment = new Attachment
                {
                    Name = "lam.jpg",
                    ContentType = "image/jpg",
                    ContentUrl = "https://www.pakutaso.com/shared/img/thumb/N112_sorededou_TP_V.jpg"
                };

                replyMessage.Attachments = new List<Attachment> { attachment };

                await context.PostAsync(replyMessage);
                */

                Activity replyToConversation = activity.CreateReply("What financial intrustment interests you the most");
                replyToConversation.Attachments = new List<Attachment>();

                AdaptiveCard card = new AdaptiveCard();

                card.Body.Add(new ChoiceSet()
                {
                    Id = "snooze",
                    Style = ChoiceInputStyle.Compact,
                    Choices = new List<Choice>()
                    {
                        new Choice() { Title = "travel", Value = "5", IsSelected = true },
                        new Choice() { Title = "golf", Value = "15" },
                        new Choice() { Title = "food", Value = "30" },
                        new Choice() { Title = "wine", Value = "40" },
                        new Choice() { Title = "hotel", Value = "50" }
                    }
                });

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };

                replyToConversation.Attachments.Add(attachment);

                var connector = new ConnectorClient(new System.Uri(message.ServiceUrl), new MicrosoftAppCredentials());

                var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);

                //await context.PostAsync(reply.Id);

                context.Wait(MessageReceivedAsync);
                

            } else if(message.Text == "travel" || message.Text == "golf" || message.Text == "food" || message.Text == "wine" || message.Text == "hotel")
            {
                await context.PostAsync($"News you might be interested in {message.Text}");
                await context.PostAsync("Your active feedback is highly appreciated. More exciting financial news coming soon. Stay tuned!");

            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}