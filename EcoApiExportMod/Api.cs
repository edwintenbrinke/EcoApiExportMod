using Newtonsoft.Json;
using System.Net;

namespace Eco.Plugins.EcoApiExportMod
{
    public class Api
    {
        // post the data to the api
        public static void Post(string url, config config_data, dynamic post_data)
        {
            using (WebClient wc = new WebClient())
            {
                // set it so the api knows that type of data is being received
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = config_data.api_access_token;
                // post to the api & transform the api array to JSON
                string HtmlResult = wc.UploadString(
                    string.Format("{0}{1}", config_data.base_api_url, url),
                    JsonConvert.SerializeObject(post_data)
                );
            }
        }
    }
}
