namespace StockTracker.Messages
{
	public class HistoricalDataEndMessage : IBMessage
	{
		public string StartDate { get; set; }

		public int RequestId { get; set; }

		public string EndDate { get; set; }

		public HistoricalDataEndMessage(int requestId, string startDate, string endDate)
		{
			Type = MessageType.HistoricalDataEnd;
			RequestId = requestId;
			StartDate = startDate;
			EndDate = endDate;
		}

		public override string ToString()
		{
			return $"HD End - RequestId: {RequestId}, Start Date: {StartDate}, End Date: {EndDate}";
		}
	}
}
