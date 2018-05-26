using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new EchoDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                IConversationUpdateActivity update = message;
                var client = new ConnectorClient(new System.Uri(update.ServiceUrl), new MicrosoftAppCredentials());
                if (update.MembersAdded != null && update.MembersAdded.Any())
                {
                    foreach (var newMember in update.MembersAdded)
                    {
                        if (newMember.Id != message.Recipient.Id)
                        {

                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: "https://www.ubs.com/magazines/taiwaninvest/tc/events/_jcr_content/mainpar/gridcontrol_746400058/col1/textimage_208195111/image.780.med.63292059.img/eye-banner.jpg"));

                            List<CardAction> cardButtons = new List<CardAction>();

                            CardAction plButton = new CardAction()
                            {
                                Value = $"https://www.ubs.com/magazines/taiwaninvest/tc/events.html",
                                Type = "openUrl",
                                Title = "Detail Page"
                            };

                            cardButtons.Add(plButton);

                            HeroCard plCard = new HeroCard()
                            {
                                Title = $"Are you interested in this seminar?",
                                Subtitle = $"【瑞銀智慧財富管理講座】名額有限，快來一探究竟",
                                Images = cardImages,
                                Buttons = cardButtons
                            };

                            var replyMessage = message.CreateReply();
                            Attachment plAttachment = plCard.ToAttachment();
                            replyMessage.Attachments = new List<Attachment> { plAttachment };
                            client.Conversations.ReplyToActivityAsync(replyMessage);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}