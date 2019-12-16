using System;

namespace StockTracker.Messages
{
	public class RealTimeBarMessage : HistoricalDataMessage
	{
		public long LongVolume { get; set; }

		public long Timestamp { get; set; }

		public RealTimeBarMessage(int reqId, long date, double open, double high, double low, double close, long volume, double WAP, int count)
			: base(reqId, UnixTimestampToDateTime(date).ToString("yyyyMMdd hh:mm:ss"), open, high, low, close, -1, count, WAP, false)
		{
			Type = MessageType.RealTimeBars;
			Timestamp = date;
			LongVolume = volume;
		}

		static DateTime UnixTimestampToDateTime(long unixTimestamp)
		{
			DateTime unixBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return unixBaseTime.AddSeconds(unixTimestamp);
		}

		public override string ToString()
		{
			return $"RT - RequestId: {RequestId}, Date: {Date}, High: {High}, Low: {Low}, Close: {Close}";
		}
	}
}
