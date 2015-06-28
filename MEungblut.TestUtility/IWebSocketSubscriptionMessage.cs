namespace MEungblut.TestUtility
{
    public abstract class WebSocketSubscriptionMessage
    {
        public abstract string ResourceToSubscribeTo { get; protected set; }

        public string GetMessageToSend()
        {
            return string.Format("SUBSCRIBE\r\n{0}", this.ResourceToSubscribeTo);
        }
    }
}
