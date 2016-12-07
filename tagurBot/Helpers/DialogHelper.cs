using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tagurBot.Helpers
{
    public static class DialogHelper
    {
        public static Activity CreateImageCarouselActivity(Activity activity, List<ImageInformation> images, string searchQuery)
        {
            Activity replyToConversation = activity.CreateReply($"Here are the top {images.Count} images I found for **{searchQuery}**:");

            replyToConversation.Type = ActivityTypes.Message;
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            foreach (var image in images)
            {
                Attachment attachment = new Attachment();
                attachment.ContentType = "application/vnd.microsoft.card.hero";

                HeroCard heroCard = new HeroCard();
                heroCard.Title = "**" + image.Caption + "**";
                heroCard.Subtitle = " ";
                heroCard.Text = "TAGS: *" + string.Join(", ", image.Tags) + "*"; 
                heroCard.Images = new List<CardImage>();
                 
                

                CardImage thumbnailImage = new CardImage();
                thumbnailImage.Url = $"https://[YOUR AZURE BLOB STORAGE LOCATION]/app-uploads/{image.Id}.jpg";
                heroCard.Images.Add(thumbnailImage);
                heroCard.Tap = new CardAction()
                {
                    Type = "openUrl",
                    Value = $"https://[YOUR AZURE BLOB STORAGE LOCATION]/app-uploads/{image.Id}.jpg",
                };
                 
                attachment.Content = heroCard;

                replyToConversation.Attachments.Add(attachment);
            }

            return replyToConversation;
        }
        
    }
}