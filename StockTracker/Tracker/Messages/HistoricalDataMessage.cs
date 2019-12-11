using System.Text;

namespace StockTracker.Messages
{
	public class HistoricalDataMessage : IBMessage 
	{
		public int RequestId { get; set; }

		public string Date { get; set; }

		public double Open { get; set; }

		public double High { get; set; }

		public double Low { get; set; }

		public double Close { get; set; }

		public int Volume { get; set; }

		public int Count { get; set; }

		public double Wap { get; set; }

		public bool HasGaps { get; set; }

		public HistoricalDataMessage(int reqId, string date, double open, double high, double low, double close, int volume, int count, double WAP, bool hasGaps)
		{
			Type = MessageType.HistoricalData;
			RequestId = reqId;
			Date = date;
			Open = open;
			High = high;
			Low = low;
			Close = close;
			Volume = volume;
			Count = count;
			Wap = WAP;
			HasGaps = hasGaps;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"HD - RequestId: {RequestId} Date: {Date}, Count: {Count}, HasGaps: {HasGaps}\n");
			sb.Append($"HD - RequestId: {RequestId} Open: {Open}, High: {High}, Low: {Low}, Close: {Close}, Volume: {Volume}, WAP: {Wap}");
			return sb.ToString();
		}
	}
}
