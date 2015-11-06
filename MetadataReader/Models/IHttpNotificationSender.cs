using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MetadataReader.Models
{
    public interface IHttpNotificationSender
    {
        Task PostHttpNotificationAsync(ScheduledImage scheduledImage);
    }

    public class HttpNotificationSender : IHttpNotificationSender
    {
        public async Task PostHttpNotificationAsync(ScheduledImage scheduledImage)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.PostAsJsonAsync(scheduledImage.SuccessNotificationUrl, new
            {
                FileName = scheduledImage.FileName,
                Metadata = scheduledImage.MetadataTags.Select(c => new { TagName = c.Name, Value = c.MetadataValue })
            });

            if (!result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                throw new Exception($"Request was not successful. Got error response. \n {resultContent}");
            }
        }
    }
}