using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetadataReader.Models;

namespace MetadataReader.Controllers
{
    public class SchedulerController : ApiController
    {
        // GET: api/Scheduler
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Scheduler/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Scheduler
        public void Post([FromBody]AssetApiModel assetApiModel)
        {
        }

        // PUT: api/Scheduler/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Scheduler/5
        public void Delete(int id)
        {
        }
    }
}
