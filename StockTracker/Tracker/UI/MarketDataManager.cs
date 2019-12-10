using IBApi;
using StockTracker.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StockTracker.UI
{
	public class MarketDataManager : DataManager
	{
		public static int MAX_SUBSCRIPTIONS_ALLOWED { get; private set; } = 15;

		private const int TICK_ID_BASE = 10000000;
		private const int HISTORICAL_ID_BASE = 30000000;

		private const int DESCRIPTION_INDEX = 0;
		private const int LAST_PRICE_INDEX = 1;
		private const int BID_PRICE_INDEX = 2;
		private const int ASK_PRICE_INDEX = 3;
		private const int WPR_5DAY_INDEX = 4;
		private const int WPR_1DAY_INDEX = 5;

		private Dictionary<string, int> ContractDescToRow = new Dictionary<string, int>();
		private Dictionary<int, int> RequestIdToRow = new Dictionary<int, int>();
		private Dictionary<int, string> RequestIdToContractDesc = new Dictionary<int, string>();

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
		
		public MarketDataManager(IBClient client, DataGridView dataGrid) : base(client, dataGrid) {}

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
			string whatToShow = "TRADES";
			int useRTH = 0;
			AddHistoricalDataRequest(contract, contractDesc, endTime, duration, barSize, whatToShow, useRTH, 1);

			return true;
		}

		private void AddHistoricalDataRequest(Contract contract, string contractDesc, string endDateTime, string durationString, string barSizeSetting, string whatToShow, int useRTH, int dateFormat)
		{
			int requestId = HISTORICAL_ID_BASE + CurrentTicker++;
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
			if (!HistoricalReqIdToContractDesc.ContainsKey(requestId))
			{
				HistoricalReqIdToContractDesc.Add(requestId, contractDesc);
			}
			if (!ContractDescToWPR.ContainsKey(contractDesc))
			{
				ContractDescToWPR.Add(contractDesc, new WPRCalculator());
			}
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
			}
			RequestIdToContractDesc.Clear();
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
			RequestIdToContractDesc.Add(requestId, contractDesc);
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
								double fiveDayLow = -1;
								foreach (var item in historicalData)
								{
									fiveDayHigh = Math.Max(fiveDayHigh, item.High);
									fiveDayLow = Math.Min(fiveDayLow, item.Low);
								}
								calc.FiveDayHigh = fiveDayHigh;
								calc.FiveDayLow = fiveDayLow;
							}
						}
					}
					break;
			}
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
						int rowIndex = RequestIdToRow[dataMessage.RequestId];
						switch (dataMessage.Field) // TickType
						{
							case 1:  // BID
							case 66: // delayed BID
								{
									grid[BID_PRICE_INDEX, rowIndex].Value = priceMessage.Price;
									break;
								}
							case 2:  // ASK
							case 67: // delayed ASK
								{
									grid[ASK_PRICE_INDEX, rowIndex].Value = priceMessage.Price;
									break;
								}
							case 4:  // LAST
							case 68: // delayed LAST
								{
									grid[LAST_PRICE_INDEX, rowIndex].Value = priceMessage.Price;
									break;
								}
							case 9:  // CLOSE
							case 75: // delayed CLOSE
								{
									if (RequestIdToContractDesc.ContainsKey(priceMessage.RequestId))
									{
										string contractDesc = RequestIdToContractDesc[priceMessage.RequestId];
										if (ContractDescToWPR.ContainsKey(contractDesc))
										{
											WPRCalculator calc = ContractDescToWPR[contractDesc];
											calc.Close = priceMessage.Price;
										}
									}
									break;
								}
							case 6:  // HIGH
							case 72: // delayed HIGH
								{
									if (RequestIdToContractDesc.ContainsKey(priceMessage.RequestId))
									{
										string contractDesc = RequestIdToContractDesc[priceMessage.RequestId];
										if (ContractDescToWPR.ContainsKey(contractDesc))
										{
											WPRCalculator calc = ContractDescToWPR[contractDesc];
											calc.LatestHigh = priceMessage.Price;
										}
									}
									break;
								}
							case 7:  // LOW
							case 73: // delayed LOW
								{
									if (RequestIdToContractDesc.ContainsKey(priceMessage.RequestId))
									{
										string contractDesc = RequestIdToContractDesc[priceMessage.RequestId];
										if (ContractDescToWPR.ContainsKey(contractDesc))
										{
											WPRCalculator calc = ContractDescToWPR[contractDesc];
											calc.LatestLow = priceMessage.Price;
										}
									}
									break;
								}
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
					CalcWPRDelegate calcFunc;
					int columnIndex;
					if (calcMsg.WPRType == WPRType.OneDay)
					{
						calcFunc = calc.Get1DayWPR;
						columnIndex = WPR_1DAY_INDEX;
					}
					else
					{
						calcFunc = calc.Get5DayWPR;
						columnIndex = WPR_5DAY_INDEX;
					}
					double wpr = calcFunc();
					if (wpr > 0)
					{
						continue;
					}
					if (ContractDescToRow.ContainsKey(contractDesc))
					{
						int rowIndex = ContractDescToRow[contractDesc];
						grid[columnIndex, rowIndex].Value = wpr;
					}
				}
			}
		}
	}

	delegate double CalcWPRDelegate();
}
