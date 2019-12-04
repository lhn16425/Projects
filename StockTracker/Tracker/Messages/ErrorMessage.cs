namespace StockTracker
{
	public class ErrorMessage : IBMessage 
	{
		public ErrorMessage(int requestId, int errorCode, string message)
		{
			Type = MessageType.Error;
			Message = message;
			RequestId = requestId;
			Code = errorCode;
		}

		public ErrorMessage(string message) : this(-1, -1, message) { }

		public string Message { get; set; }

		public int Code { get; set; }

		public int RequestId { get; set; }

		public override string ToString()
		{
			return string.Format("Error: Request={0}, Code={1} - {2}", RequestId, Code, Message);
		}
	}
}
