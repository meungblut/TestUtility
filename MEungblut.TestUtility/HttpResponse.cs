namespace MEungblut.TestUtility
{
    using System.Collections.Generic;

    public class HttpResponse
    {
        public HttpResponse(int resposeCode, string body, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, string contentType)
        {
            this.ContentType = contentType;
            this.ResponseCode = resposeCode;
            this.Body = body;
            this.Headers = headers;
        }

        public int ResponseCode { get; private set; }

        public string Body { get; private set; }

        public string ContentType { get; set; }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; private set; }
    }
}