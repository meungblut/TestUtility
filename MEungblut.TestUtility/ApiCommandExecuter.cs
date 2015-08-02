namespace MEungblut.TestUtility
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ApiCommandExecuter
    {
        private readonly string uri;

        public ApiCommandExecuter(string apiLocation)
        {
            this.uri = apiLocation;
        }

        public async Task<HttpResponseMessage> SendCommand(ApiMessage argumentsToSend)
        {
            return await this.SendRequestToApi(argumentsToSend);
        }

        protected async Task<HttpResponseMessage> SendRequestToApi(ApiMessage apiMessage)
        {
            var urlToSendRequestTo = new Uri(this.uri + apiMessage.UrlFragmentToSendTo);

            var client = new HttpClient();

            if (apiMessage.HttpVerb == HttpVerb.Put)
                return await client.PutAsync(urlToSendRequestTo, apiMessage.BodyAsStringContent());

            return await client.PostAsync(urlToSendRequestTo, apiMessage.BodyAsStringContent());
        } 
    }
}
