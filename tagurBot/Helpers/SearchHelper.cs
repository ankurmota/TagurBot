using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;

namespace tagurBot.Helpers
{
    public static class SearchHelper
    {        
        public static async Task<Activity> CreateResponseAsync(Activity activity)
        {
            Activity response = null;

            var entity = await ParseSearchEntityAsync(activity.Text);

            if (entity != null)
            {
                string searchQuery = entity;
                
                var images = await SearchImagesAsync(searchQuery);

                response = Helpers.DialogHelper.CreateImageCarouselActivity(activity, images, searchQuery);                
            }
            else
            {
                if (Common.IsInitialized)
                {
                    response = activity.CreateReply("Try searching for common tag or categories, such as 'house', 'man', or 'puppy'. Here's an example: *\"Find **boat** images\"*.");
                }
                else
                {
                    response = activity.CreateReply("Glad you stopped by! To get started, search for common tag or categories, such as 'house', 'man', or 'puppy' by typing something like *\"Find **boat** images\"*.");
                }
            }

            Common.IsInitialized = true;

            return response;
        }
        
        private static async Task<string> ParseSearchEntityAsync(string searchQuery)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            Guid subscriptionKey = new Guid("[YOUR CS LUIS SUBSCRIPTION KEY]");
            Guid appId = new Guid("[YOUR CS LUIS APP ID]");

            var uri = $"https://api.projectoxford.ai/luis/v1/application/preview?id={appId}&subscription-key={subscriptionKey}&q={Uri.EscapeDataString(searchQuery)}";

            var stream = await client.GetStreamAsync(uri);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LuisResponse));
            var response = (LuisResponse)ser.ReadObject(stream);

            string searchEntity = null;

            try
            {
                searchEntity = response.topScoringIntent.actions[0].parameters[0].value.Last().entity;
            }
            catch { }

            return searchEntity;
        }

        private static async Task<List<ImageInformation>> SearchImagesAsync(string searchQuery)
        {
            List<ImageInformation> images = new List<ImageInformation>();

            SearchServiceClient serviceClient = new SearchServiceClient("[YOUR AZURE SEARCH SERVICE NAME]", new SearchCredentials("[YOUR AZURE SEARCH API KEY]"));

            var sp = new SearchParameters() { Top = 10 };

            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("tags");
            DocumentSearchResult response = await indexClient.Documents.SearchAsync(searchQuery.Trim() + "*", sp);

            images = (from result in response.Results
                      select new ImageInformation()
                      {
                          Id = new Guid((string)result.Document["id"]),
                          Caption = (result.Document["caption"] + "").ToFirstCharUpper(),
                          Tags = ((string[])result.Document["tags"]).ToList()

                      }).ToList();

            return images;
        }        
        
    }
}