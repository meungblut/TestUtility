namespace MEungblut.TestUtility.DummyServices
{
    using System.Collections.Generic;

    using MEungblut.Websockets;
    using MEungblut.Websockets.ExternalPublishing.Protocol;
    using MEungblut.Websockets.SuperWebSocket;

    public class DummyWebsocketServer
    {
        private static IWebSocketManager superWebsocketManager;

        private readonly Dictionary<string, IWebSocketManager> managers;

        public DummyWebsocketServer()
        {
            this.managers = new Dictionary<string, IWebSocketManager>();
        }

        public void AddWebSocketManager(string id, string server, int port)
        {
            this.managers.Add(id, new SuperWebsocketManager(new WebsocketConfiguration(port, server)));
        }

        public DummyWebsocketServer(string server, int port)
        {
            superWebsocketManager = new SuperWebsocketManager(new WebsocketConfiguration(port, server));
        }

        public static void SendMessage(string message)
        {
            superWebsocketManager.BroadcastToAllClients(message);
        }

        public static void SendMessage(IDomainEvent domainEvent)
        {
            var serialiser = new WebSocketDataSerialisation();
            superWebsocketManager.BroadcastToAllClients(serialiser.GetString(domainEvent));
        }

        public void SendMessage(string webSocketId, string message)
        {
            this.managers[webSocketId].BroadcastToAllClients(message);
        }

        public void SendMessage(string webSocketId, IDomainEvent domainEvent)
        {
            var serialiser = new WebSocketDataSerialisation();
            this.managers[webSocketId].BroadcastToAllClients(serialiser.GetString(domainEvent));
        }
    }
}
