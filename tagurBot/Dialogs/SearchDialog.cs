using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;

namespace tagurBot.Dialogs
{
    [LuisModel("d8cc0ca9-6bc6-4972-b44e-4af246ed6d8b", "bf1131f271e5477398c77428cc445257")]
    [Serializable]
    public class SearchDialog : LuisDialog<object>
    {
        [LuisIntent("SearchImages")]
        public async Task SearchImages(IDialogContext context, LuisResult result)
        {
            var entity = await ParseSearchEntityAsync(result.Query);

            if (entity != null)
            {
                string searchQuery = entity;

                await context.PostAsync("Searching for **" + entity + "**...");

                var imageIds = await SearchImagesAsync(searchQuery);
                
                await context.PostAsync(string.Join(",", imageIds.Take(5)));

                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Try searching for common tag or categories, such as 'house', 'man', or 'puppy'.");
                await context.PostAsync("Here's an example: *\"Find **boat** images.\"*");
                context.Wait(MessageReceived);
            }
        }

        private IMessageActivity CreateImageCarouselActivity(IMessageActivity activity, List<Guid> imageIds)
        {
            activity = new Activity("Here are the top images I found:");// activity.CreateReply("Here's the latest news I found:");

            activity.Type = ActivityTypes.Message;
            activity.AttachmentLayout = "Carousel";
            activity.Attachments = new List<Attachment>();

            foreach (var imageId in imageIds)
            {
                Attachment attachment = new Attachment();
                attachment.ContentType = "application/vnd.microsoft.card.hero";

                HeroCard heroCard = new HeroCard();
                heroCard.Title = imageId.ToString();
                heroCard.Subtitle = imageId.ToString();
                heroCard.Images = new List<CardImage>();

                CardImage thumbnailImage = new CardImage();
                thumbnailImage.Url = $"https://tagur.blob.core.windows.net/app-uploads/{imageId}.jpg";
                heroCard.Images.Add(thumbnailImage);

                heroCard.Buttons = new List<CardAction>();
                CardAction articleCard = new CardAction();

                articleCard.Title = "View article";
                articleCard.Type = "openUrl";

                articleCard.Value = $"https://tagur.blob.core.windows.net/app-uploads/{imageId}.jpg";
                heroCard.Buttons.Add(articleCard);

                attachment.Content = heroCard;

                activity.Attachments.Add(attachment);
            }

            return activity;
        }

        private async Task<string> ParseSearchEntityAsync(string searchQuery)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            Guid subscriptionKey = new Guid("bf1131f271e5477398c77428cc445257");
            Guid appId = new Guid("d8cc0ca9-6bc6-4972-b44e-4af246ed6d8b");
             
            var uri = $"https://api.projectoxford.ai/luis/v1/application/preview?id=d8cc0ca9-6bc6-4972-b44e-4af246ed6d8b&subscription-key=bf1131f271e5477398c77428cc445257&q={Uri.EscapeDataString(searchQuery)}";
            
            var stream = await client.GetStreamAsync(uri);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LuisResponse));
            var response = (LuisResponse)ser.ReadObject(stream);

            string searchEntity = null;

            try
            {
                searchEntity = response.topScoringIntent.actions[0].parameters[0].value[0].entity;
            }
            catch { }
            
            return searchEntity;
        }

        private async Task<List<Guid>> SearchImagesAsync(string searchQuery)
        {
            List<Guid> imageIds = new List<Guid>();

            SearchServiceClient serviceClient = new SearchServiceClient("tagur", new SearchCredentials("FD636DD031CBFB765D9371827812828F"));
            
            var sp = new SearchParameters() { };
            
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("tags");
            DocumentSearchResult response = await indexClient.Documents.SearchAsync(searchQuery.Trim() + "*", sp);
            
            imageIds = (from result in response.Results
                        select new Guid((string)result.Document["id"])).ToList();

            return imageIds;
        }
 
        [LuisIntent("None")]
        public async Task DoNothing(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Try searching for common tag or categories, such as 'house', 'man', or 'puppy'. Example: \"Find boat images.\"");
            context.Wait(MessageReceived);
        }
    }
}