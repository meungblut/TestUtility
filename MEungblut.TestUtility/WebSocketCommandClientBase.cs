namespace MEungblut.TestUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    using MEungblut.Websockets.Client;

    public abstract class WebSocketCommandClientBase
    {
        private readonly string uri;
        private readonly CamelCaseJsonConverter converter;
        private readonly HttpCommunicator httpCommunicator;

        private IClientWebSocket websocket;

        private List<object> messages;

        private readonly ManualResetEvent socketConnectedEvent;

        private string webSocketLocation;

        protected WebSocketCommandClientBase(string apiLocation, string webSocketLocation)
        {
            this.webSocketLocation = webSocketLocation;
            this.uri = apiLocation;
            this.messages = new List<object>();
            this.httpCommunicator = new HttpCommunicator();
            this.converter = new CamelCaseJsonConverter();
            this.socketConnectedEvent = new ManualResetEvent(false);

            this.ConnectWebsocket();
        }

        private void ConnectWebsocket()
        {
            Console.WriteLine("About to connect to main socket at " + webSocketLocation);

            this.websocket = new WebSocket4NetSocketClient();
            this.websocket.MessageReceived += this.WebsocketMessageReceived;
            this.websocket.Connect(webSocketLocation);

            Task.Factory.StartNew(this.WaitForSocketToConnect);

            var socketConnected = this.socketConnectedEvent.WaitOne(TimeSpan.FromSeconds(3));

            if (!socketConnected) throw new WebSocketCouldNotConnectException();
        }

        private void WaitForSocketToConnect()
        {
            while (this.websocket.State != WebSocketState.Open)
            {
                Thread.Sleep(10);
            }
            this.socketConnectedEvent.Set();
        }

        private async Task<string> GetWebsocketEndpoint()
        {
            HttpResponse webSocketLocationResponse = await this.httpCommunicator.Get(new HttpRequest(string.Empty, new Uri(this.uri + "/websocket/configuration")));
            return webSocketLocationResponse.Body;
        }

        void WebsocketMessageReceived(object sender, string e)
        {
            Console.WriteLine("Received websocket message: " + e);
            this.messages.Add(this.ConvertMessageToContractType(e));
        }

        public void SendMessage(string message)
        {
            this.websocket.SendMessage(message);
        }

        public void SendMessage(WebSocketSubscriptionMessage subscriptionMessage)
        {
            this.websocket.SendMessage(subscriptionMessage.GetMessageToSend());
        }

        public async Task<TExpectedResponseType> SendCommandAndWaitForResponseOfType<TExpectedResponseType>(ApiMessage argumentsToSend, TimeSpan timeAfterWhichToStopWaiting) where TExpectedResponseType : class
        {
            this.messages = new List<object>();

            await this.SendRequestToApi(argumentsToSend);

            return this.WaitForMessageOfType<TExpectedResponseType>(timeAfterWhichToStopWaiting);
        }

        public async void SendCommand(ApiMessage argumentsToSend)
        {
            this.messages = new List<object>();

            await this.SendRequestToApi(argumentsToSend);
        }

        public TExpectedResponseType WaitForMessageOfType<TExpectedResponseType>(TimeSpan timeAfterWhichToStopWaiting) where TExpectedResponseType : class
        {
            DateTime startDate = DateTime.Now;
            while (!this.messages.OfType<TExpectedResponseType>().Any())
            {
                Thread.Sleep(100);
                if (DateTime.Now.Subtract(timeAfterWhichToStopWaiting) > startDate) throw new ExpectedResponseTypeNotReceivedWithinTimeoutException();
            }

            return this.messages.OfType<TExpectedResponseType>().First();
        }

        protected async Task<HttpResponse> SendRequestToApi(ApiMessage apiMessage)
        {
            var urlToSendRequestTo = new Uri(this.uri + apiMessage.UrlFragmentToSendTo);

            var client = new HttpClient();

            if (apiMessage.HttpVerb == HttpVerb.Put)
                await client.PutAsync(urlToSendRequestTo, apiMessage.BodyAsStringContent());
            else
                await client.PostAsync(urlToSendRequestTo, apiMessage.BodyAsStringContent());

            return null;
        }

        private object ConvertMessageToContractType(string websocketMessage)
        {
            string[] splitMessage = websocketMessage.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string mediaType = splitMessage[0].Split(':')[1];
            var typeMediaTypeCorrespondsTo = this.GetTypeFor(mediaType);

            if (typeMediaTypeCorrespondsTo == typeof(UnknownType))
            {
                return new UnknownType(websocketMessage);
            }

            string body = splitMessage[1];
            return this.converter.Deserialize(body, typeMediaTypeCorrespondsTo);
        }

        protected abstract Type GetTypeFor(string mediaType);
    }
}
