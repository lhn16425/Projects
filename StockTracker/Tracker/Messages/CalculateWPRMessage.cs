namespace StockTracker.Messages
{
	public enum WPRType
	{
		OneDay,
		FiveDay
	};

	class CalculateWPRMessage : IBMessage
	{
		public CalculateWPRMessage(WPRType wprType)
		{
			Type = MessageType.CalculateWPR;
			WPRType = wprType;
		}

		public WPRType WPRType { get; set; }
	}
}