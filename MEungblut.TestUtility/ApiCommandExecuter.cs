﻿namespace MEungblut.TestUtility
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

        public async void SendCommand(ApiMessage argumentsToSend)
        {
            await this.SendRequestToApi(argumentsToSend);
        }

        protected async Task SendRequestToApi(ApiMessage apiMessage)
        {
            var urlToSendRequestTo = new Uri(this.uri + apiMessage.UrlFragmentToSendTo);

            var client = new HttpClient();

            if (apiMessage.HttpVerb == HttpVerb.Put)
                await client.PutAsync(urlToSendRequestTo, apiMessage.BodyAsStringContent());
            else
                await client.PostAsync(urlToSendRequestTo, apiMessage.BodyAsStringContent());
        }
    }
}
