namespace MEungblut.TestUtility 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    using MEungblut.Websockets.Client;

    public abstract class WebSocketEventListener
    {
        private readonly CamelCaseJsonConverter converter;

        private IClientWebSocket websocket;

        private List<object> messages;

        private readonly ManualResetEvent socketConnectedEvent;

        private string webSocketLocation;

        protected WebSocketEventListener(string webSocketLocation)
        {
            this.webSocketLocation = webSocketLocation;
            this.messages = new List<object>();
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

        public TExpectedResponseType WaitForMessageOfType<TExpectedResponseType>(TimeSpan timeAfterWhichToStopWaiting, Func<TExpectedResponseType, bool> expressionToMatch) where TExpectedResponseType : class
        {
            DateTime startDate = DateTime.Now;
            while (!this.messages.OfType<TExpectedResponseType>().Any(expressionToMatch))
            {
                Thread.Sleep(100);
                if (DateTime.Now.Subtract(timeAfterWhichToStopWaiting) > startDate) throw new ExpectedResponseTypeNotReceivedWithinTimeoutException();
            }

            return this.messages.OfType<TExpectedResponseType>().First(expressionToMatch);
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
