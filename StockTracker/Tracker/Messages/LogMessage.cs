namespace StockTracker.Messages
{
	class LogMessage : IBMessage
	{
		public LogMessage(string message)
		{
			Type = MessageType.Log;
			Message = message;
		}

		public string Message { get; set; }
	}
}