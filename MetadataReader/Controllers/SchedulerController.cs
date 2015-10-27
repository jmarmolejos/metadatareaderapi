using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Hangfire;
using MetadataReader.BackgroundJobs;
using MetadataReader.Models;

namespace MetadataReader.Controllers
{
    public class SchedulerController : ApiController
    {
        private IMetadataContext _context;
        private IBackgroundJobClient _backgroundJobClient;

        public SchedulerController(IMetadataContext context, IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _backgroundJobClient = backgroundJobClient;
        }

        public SchedulerController() 
            : this(new MetadataContext(), new BackgroundJobClient())
        {
            
        }

        // GET: api/Scheduler
        public IEnumerable<AssetApiModel> Get()
        {
            return _context.ScheduledImages.ToList().Select(im => AssetApiModel(im));
        }
        
        // GET: api/Scheduler/5
        public AssetApiModel Get(int id)
        {
            var imageMetadata = _context.ScheduledImages.Find(id);
            return AssetApiModel(imageMetadata);
        }

        // POST: api/Scheduler
        public void Post([FromBody]AssetApiModel assetApiModel)
        {
            // Should save asset info
            var imageMetadata = new ScheduledImage()
            {
                FileName = assetApiModel.FileName,
                DownloadUrl = assetApiModel.Url
            };
            _context.ScheduledImages.Add(imageMetadata);

            _context.SaveChanges();
            
            // Should start download and encode queue
            var helper = new JobsHelper(_context, new DownloadToStream(), new CustomMetadataReader());
            _backgroundJobClient.Enqueue(() => helper.DownloadAndReadMetadata(imageMetadata.Id));
            
            // Should return saved asset info ?
        }

        

        // PUT: api/Scheduler/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Scheduler/5
        public void Delete(int id)
        {
        }

        private AssetApiModel AssetApiModel(ScheduledImage im)
        {
            return new AssetApiModel()
            {
                FileName = im.FileName,
                Url = im.DownloadUrl
            };
        }

    }
}
