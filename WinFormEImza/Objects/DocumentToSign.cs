using System.Collections.Generic;

namespace WinFormEImza.Objects
{
    public class DocumentToSign
    {
        public string source { get; set; }
        public string sourceName { get; set; }
        public string targetUrl { get; set; }
        public string sourceType { get; set; }
        public string format { get; set; }
    }

    public class DocumentRequestResponse
    {
        public string id { get; set; }
        public List<DocumentToSign> resources { get; set; }
        public string responseUrl { get; set; }
    }

    public class FileUploadResponse
    {
        public bool error { get; set; }
        public string msg { get; set; }
    }
}
