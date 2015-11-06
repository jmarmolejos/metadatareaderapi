using System.Linq;
using System.Threading.Tasks;

namespace MetadataReader.Models
{
    public interface IPostNotificationSender
    {
        Task SendCompleteNotification(int imageId);
    }

    public class PostNotificationSender : IPostNotificationSender
    {
        private IHttpNotificationSender _httpNotificationSender;

        private IMetadataRepository _repository;

        public PostNotificationSender(IHttpNotificationSender httpNotificationSender, IMetadataRepository repository)
        {
            _httpNotificationSender = httpNotificationSender;
            _repository = repository;
        }

        public async Task SendCompleteNotification(int imageId)
        {
            var image = _repository.GetScheduledImage(imageId);
            await _httpNotificationSender.PostHttpNotificationAsync(image);
        }
    }
}