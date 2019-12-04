namespace StockTracker.Messages
{
	public class ConnectionStatusMessage : IBMessage
	{
		public bool Connected { get; private set; }

		public ConnectionStatusMessage(bool connected)
		{
			Type = MessageType.ConnectionStatus;
			Connected = connected;
		}
	}
}
