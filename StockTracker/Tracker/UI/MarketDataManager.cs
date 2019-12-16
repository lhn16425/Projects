using IBApi;
using StockTracker.Messages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace StockTracker.UI
{
	public class MarketDataManager : DataManager
	{
		public static int MAX_SUBSCRIPTIONS_ALLOWED { get; private set; } = 15;

		private const int TICK_ID_BASE = 10000000;
		private const int HISTORICAL_ID_BASE = 30000000;
		private const int RT_BARS_ID_BASE = 40000000;

		private const int DESCRIPTION_INDEX = 0;
		private const int LAST_PRICE_INDEX = 1;
		private const int BID_PRICE_INDEX = 2;
		private const int ASK_PRICE_INDEX = 3;
		private const int WPR_5DAY_INDEX = 4;
		private const int WPR_1DAY_INDEX = 5;

		private Dictionary<string, int> ContractDescToRow = new Dictionary<string, int>();
		private Dictionary<int, int> RequestIdToRow = new Dictionary<int, int>();
		private Dictionary<int, string> RealTimeReqIdToContractDesc = new Dictionary<int, string>();

		private Dictionary<int, string> HistoricalReqIdToContractDesc = new Dictionary<int, string>();
		private Dictionary<int, List<HistoricalDataMessage>> HistoricalData = new Dictionary<int, List<HistoricalDataMessage>>();

		private Dictionary<string, WPRCalculator> ContractDescToWPR = new Dictionary<string, WPRCalculator>();

		private class CompareHistoricalData : IComparer<HistoricalDataMessage>
		{
			public int Compare(HistoricalDataMessage x, HistoricalDataMessage y)
			{
				return x.Date.CompareTo(y.Date);
			}
		}

		private CompareHistoricalData HistoricalDataComparer = new CompareHistoricalData();

		private SoundPlayer SoundPlayer = new SoundPlayer(@"C:\Windows\media\Alarm01.wav");

		public MarketDataManager(IBClient client, DataGridView dataGrid) : base(client, dataGrid) { }

		public bool AddRequest(Contract contract, string genericTickList, bool getDelayedData = false)
		{
			if (RequestIdToRow.Count >= MAX_SUBSCRIPTIONS_ALLOWED)
			{
				return false;
			}
			string contractDesc = Utils.Utils.ContractToString(contract);
			if (ContractDescToRow.ContainsKey(contractDesc) &&
				RequestIdToRow.ContainsValue(ContractDescToRow[contractDesc]))
			{
				return true; // already have an active subscription for this contract
			}
			int requestId = TICK_ID_BASE + CurrentTicker;
			AddDataRow(requestId, contractDesc);
			if (getDelayedData)
			{
				Client.ClientSocket.reqMarketDataType(3);
			}
			Client.ClientSocket.reqMktData(requestId, contract, genericTickList, false, new List<TagValue>());

			string endTime = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
			string duration = "5 D";
			string barSize = "1 day";
			string whatToShow = (contract.SecType == "CASH") ? "MIDPOINT" : "TRADES";
			int useRTH = 0;
			AddHistoricalDataRequest(contract, contractDesc, endTime, duration, barSize, whatToShow, useRTH, 1);

			AddRealTimeRequest(contract, contractDesc, whatToShow, true);

			ContractDescToWPR.Add(contractDesc, new WPRCalculator());

			return true;
		}

		public void AddRealTimeRequest(Contract contract, string contractDesc, string whatToShow, bool useRTH)
		{
			int requestId = RT_BARS_ID_BASE + CurrentTicker++;
			Client.ClientSocket.reqRealTimeBars(requestId, contract, 5, whatToShow, useRTH, null);
			RealTimeReqIdToContractDesc.Add(requestId, contractDesc);
		}

		private void AddHistoricalDataRequest(Contract contract, string contractDesc, string endDateTime, string durationString, string barSizeSetting, string whatToShow, int useRTH, int dateFormat)
		{
			int requestId = HISTORICAL_ID_BASE + CurrentTicker;
			Client.ClientSocket.reqHistoricalData
				(
					requestId,
					contract,
					endDateTime,
					durationString,
					barSizeSetting,
					whatToShow,
					useRTH,
					1,
					new List<TagValue>()
				);
			HistoricalReqIdToContractDesc.Add(requestId, contractDesc);
		}

		public override void NotifyError(int requestId)
		{
		}

		public override void Clear()
		{
			((DataGridView)DataGrid).Rows.Clear();
			ContractDescToRow.Clear();
			CurrentTicker = 1;
		}

		public void StopActiveRequests(bool clearTable)
		{
			for (int i = 1; i < CurrentTicker; i++)
			{
				Client.ClientSocket.cancelMktData(i + TICK_ID_BASE);
				Client.ClientSocket.cancelRealTimeBars(i + RT_BARS_ID_BASE);
			}
			RealTimeReqIdToContractDesc.Clear();
			RequestIdToRow.Clear();
			HistoricalData.Clear();
			HistoricalReqIdToContractDesc.Clear();
			ContractDescToWPR.Clear();
			if (clearTable)
			{
				Clear();
			}
		}

		private void AddDataRow(int requestId, string contractDesc)
		{
			DataGridView grid = (DataGridView)DataGrid;
			int rowIndex;
			if (ContractDescToRow.ContainsKey(contractDesc))
			{
				rowIndex = ContractDescToRow[contractDesc];
			}
			else
			{
				rowIndex = requestId - TICK_ID_BASE - 1;
				grid.Rows.Add(rowIndex, 0);
				grid[DESCRIPTION_INDEX, rowIndex].Value = contractDesc;
				ContractDescToRow.Add(contractDesc, rowIndex);
			}
			RequestIdToRow.Add(requestId, rowIndex);
		}

		public void UpdateRealTimeData(RealTimeBarMessage message)
		{
			if (RealTimeReqIdToContractDesc.ContainsKey(message.RequestId))
			{
				string contractDesc = RealTimeReqIdToContractDesc[message.RequestId];
				if (ContractDescToWPR.ContainsKey(contractDesc))
				{
					WPRCalculator calc = ContractDescToWPR[contractDesc];
					calc.Close = message.Close;
					calc.LatestHigh = message.High;
					calc.LatestLow = message.Low;
				}
			}
		}

		public void UpdateHistoricalData(IBMessage message)
		{
			switch (message.Type)
			{
				case MessageType.HistoricalData:
					HistoricalDataMessage hdMsg = (HistoricalDataMessage)message;
					if (!HistoricalData.ContainsKey(hdMsg.RequestId))
					{
						HistoricalData.Add(hdMsg.RequestId, new List<HistoricalDataMessage>());
					}
					HistoricalData[hdMsg.RequestId].Add(hdMsg);
					break;
				case MessageType.HistoricalDataEnd:
					HistoricalDataEndMessage endMsg = (HistoricalDataEndMessage)message;
					int requestId = endMsg.RequestId;
					if (!HistoricalData.ContainsKey(requestId))
					{
						throw new Exception("Received end message with no historical data");
					}
					HistoricalData[endMsg.RequestId].Sort(HistoricalDataComparer);
					if (HistoricalReqIdToContractDesc.ContainsKey(requestId))
					{
						string contractDesc = HistoricalReqIdToContractDesc[requestId];
						if (ContractDescToWPR.ContainsKey(contractDesc))
						{
							WPRCalculator calc = ContractDescToWPR[contractDesc];
							if (HistoricalData.ContainsKey(requestId) &&
								(HistoricalData[requestId].Count > 0))
							{
								List<HistoricalDataMessage> historicalData = HistoricalData[requestId];
								HistoricalDataMessage last = historicalData.Last();
								calc.OneDayHigh = last.High;
								calc.OneDayLow = last.Low;
								calc.Close = last.Close;
								double fiveDayHigh = -1;
								double fiveDayLow = double.MaxValue;
								foreach (var item in historicalData)
								{
									fiveDayHigh = Math.Max(fiveDayHigh, item.High);
									fiveDayLow = Math.Min(fiveDayLow, item.Low);
								}
								calc.FiveDayHigh = fiveDayHigh;
								calc.FiveDayLow = (fiveDayLow == double.MaxValue) ? -1 : fiveDayLow;
							}
						}
					}
					break;
			}
		}

		private string GetFormattedPrice(TickPriceMessage message)
		{
			string price;
			switch (message.Field)
			{
				case TickType.BID:
				case TickType.DELAYED_BID:
				case TickType.ASK:
				case TickType.DELAYED_ASK:
					if (message.Price < 1.0)
					{
						price = string.Format("{0:F3}", message.Price);
					}
					else
					{
						price = string.Format("{0:F2}", message.Price);
					}
					break;
				case TickType.LAST:
				case TickType.DELAYED_LAST:
				default:
					price = string.Format("{0:F3}", message.Price);
					break;
			}
			return price;
		}

		public override void UpdateUI(IBMessage message)
		{
			DataGridView grid = (DataGridView)DataGrid;
			if (message is MarketDataMessage)
			{
				MarketDataMessage dataMessage = (MarketDataMessage)message;
				if (RequestIdToRow.ContainsKey(dataMessage.RequestId))
				{
					if (message is TickPriceMessage)
					{
						TickPriceMessage priceMessage = (TickPriceMessage)message;
						int columnIndex = -1;
						switch (dataMessage.Field) // TickType
						{
							case TickType.BID:
							case TickType.DELAYED_BID:
								columnIndex = BID_PRICE_INDEX;
								break;
							case TickType.ASK:
							case TickType.DELAYED_ASK:
								columnIndex = ASK_PRICE_INDEX;
								break;
							case TickType.LAST:
							case TickType.DELAYED_LAST:
								columnIndex = LAST_PRICE_INDEX;
								break;
							default:
								return;
						}
						int rowIndex = RequestIdToRow[dataMessage.RequestId];
						grid[columnIndex, rowIndex].Value = GetFormattedPrice(priceMessage);
						double previousPrice = Convert.ToDouble(grid[columnIndex, rowIndex].Value);
						if (previousPrice > priceMessage.Price)
						{
							grid[columnIndex, rowIndex].Style.ForeColor = Color.Red;
						}
						else if (previousPrice > priceMessage.Price)
						{
							grid[columnIndex, rowIndex].Style.ForeColor = Color.Green;
						}
						else
						{
							grid[columnIndex, rowIndex].Style.ForeColor = Color.Black;
						}
					}
				}
			}
			else if (message is CalculateWPRMessage)
			{
				CalculateWPRMessage calcMsg = (CalculateWPRMessage)message;
				foreach (var item in ContractDescToWPR)
				{
					string contractDesc = item.Key;
					WPRCalculator calc = item.Value;
					if (calcMsg.WPRType == WPRType.Both)
					{
						CalculateBothWPRs(contractDesc, calc, grid);
					}
					else
					{
						CalculateSingleWPR(contractDesc, calc, calcMsg.WPRType, grid);
					}
				}
			}
		}

		private void CalculateBothWPRs(string contractDesc, WPRCalculator calc, DataGridView grid)
		{
			double oneDayWPR = calc.Get1DayWPR();
			double fiveDayWPR = calc.Get5DayWPR();
			if (ContractDescToRow.ContainsKey(contractDesc))
			{
				int rowIndex = ContractDescToRow[contractDesc];
				if (oneDayWPR < 0)
				{
					grid[WPR_1DAY_INDEX, rowIndex].Value = string.Format("{0:F2}", oneDayWPR);
				}
				if (fiveDayWPR < 0)
				{
					grid[WPR_5DAY_INDEX, rowIndex].Value = string.Format("{0:F2}", fiveDayWPR);
				}
				if ((oneDayWPR <= -80) && (fiveDayWPR <= -80))
				{
					SoundPlayer.Play();
					grid[WPR_1DAY_INDEX, rowIndex].Style.BackColor = Color.Red;
					grid[WPR_5DAY_INDEX, rowIndex].Style.BackColor = Color.Red;
				}
				else
				{
					grid[WPR_1DAY_INDEX, rowIndex].Style.BackColor = Color.White;
					grid[WPR_5DAY_INDEX, rowIndex].Style.BackColor = Color.White;
				}
			}
		}

		private void CalculateSingleWPR(string contractDesc, WPRCalculator calc, WPRType type, DataGridView grid)
		{
			CalcWPRDelegate calcFunc;
			int columnIndex;
			double otherLatestWPR;
			if (type == WPRType.OneDay)
			{
				otherLatestWPR = calc.Latest5DayWPR;
				calcFunc = calc.Get1DayWPR;
				columnIndex = WPR_1DAY_INDEX;
			}
			else
			{
				otherLatestWPR = calc.Latest1DayWPR;
				calcFunc = calc.Get5DayWPR;
				columnIndex = WPR_5DAY_INDEX;
			}
			double wpr = calcFunc();

			if (ContractDescToRow.ContainsKey(contractDesc))
			{
				if (wpr < 0)
				{
					int rowIndex = ContractDescToRow[contractDesc];
					grid[columnIndex, rowIndex].Value = string.Format("{0:F2}", wpr);
				}
				if ((wpr <= -80) && (otherLatestWPR <= -80))
				{
					SoundPlayer.Play();
				}
			}
		}

		delegate double CalcWPRDelegate();
	}
}
