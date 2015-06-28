namespace MEungblut.TestUtility
{
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpCommunicator
    {
        private readonly HttpClient client;
        private HttpContent content;

        public HttpCommunicator()
        {
            this.client = new HttpClient();
        }

        public async Task<HttpResponse> Post(HttpRequest message)
        {
            this.content = new StringContent(message.Body, Encoding.UTF8, "application/json");
            var responseMessage = await this.client.PostAsync(message.Url, this.content);

            return await ConvertResponseToHttpResponse(responseMessage);
        }

        public async Task<HttpResponse> Get(HttpRequest message)
        {
            var responseMessage = await this.client.GetAsync(message.Url);

            return await ConvertResponseToHttpResponse(responseMessage);
        }

        private static async Task<HttpResponse> ConvertResponseToHttpResponse(HttpResponseMessage responseMessage)
        {
            var headers = from x in responseMessage.Headers
                select x;

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            return new HttpResponse((int) responseMessage.StatusCode, responseBody, headers, responseMessage.Content.Headers.ContentType.MediaType);
        }
    }
}