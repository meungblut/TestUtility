namespace MEungblut.TestUtility
{
    using System;

    public class HttpRequest
    {
        public HttpRequest(string body, Uri uri)
        {
            this.Body = body;
            this.Url = uri;
        }

        public string Body { get; set; }

        public Uri Url { get; set; }
    }
}