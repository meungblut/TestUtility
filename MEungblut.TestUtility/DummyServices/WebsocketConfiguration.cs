namespace MEungblut.TestUtility.DummyServices
{
    using MEungblut.Websockets;

    public class WebsocketConfiguration : IWebsocketConfiguration
    {
        public int Port { get; private set; }

        public string Server { get; private set; }

        public WebsocketConfiguration(int port, string server)
        {
            this.Port = port;
            this.Server = server;
        }
    }
}
