using System;
using System.IO;
using System.Net;

namespace MetadataReader.Models
{
    public interface IDownloadToStream
    {
        MemoryStream Download(string url);
    }

    public class DownloadToStream : IDownloadToStream
    {
        public MemoryStream Download(string url)
        {
            // A better implementation that supports limiting the bytes being read: http://stackoverflow.com/a/5605490/116685
            WebClient webClient = new WebClient();
            var stream = new MemoryStream(webClient.DownloadData(url));

            return stream;
        }
    }
}