using System;
using System.IO;
using System.Net;

namespace MetadataReader.Models
{
    public class DownloadToStream
    {
        private string _url;

        public DownloadToStream(string fileUrl)
        {
            _url = fileUrl;
        }

        public MemoryStream Download()
        {
            // A better implementation that supports limiting the bytes being read: http://stackoverflow.com/a/5605490/116685
            WebClient webClient = new WebClient();
            var stream = new MemoryStream(webClient.DownloadData(_url));

            return stream;
        }
    }
}