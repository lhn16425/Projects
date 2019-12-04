using IBApi;
using StockTracker.Messages;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StockTracker.UI
{
	public class MarketDataManager : DataManager
	{
		private const int MAX_SUBSCRIPTIONS_ALLOWED = 15;

		private const int TICK_ID_BASE = 10000000;

		private const int DESCRIPTION_INDEX = 0;
		private const int LAST_PRICE_INDEX = 1;
		private const int BID_PRICE_INDEX = 2;
		private const int ASK_PRICE_INDEX = 3;

		private Dictionary<string, int> ContractDescToRow = new Dictionary<string, int>();
		private Dictionary<int, int> RequestIdToRow = new Dictionary<int, int>();
		
		public MarketDataManager(IBClient client, DataGridView dataGrid) : base(client, dataGrid)
		{
		}

		public void AddRequest(Contract contract, string genericTickList, bool getDelayedData = false)
		{
			if (RequestIdToRow.Count >= MAX_SUBSCRIPTIONS_ALLOWED)
			{
				return;
			}
			string contractDesc = Utils.Utils.ContractToString(contract);
			if (ContractDescToRow.ContainsKey(contractDesc) &&
				RequestIdToRow.ContainsValue(ContractDescToRow[contractDesc]))
			{
				return; // already have an active subscription for this contract
			}
			int nextReqId = TICK_ID_BASE + CurrentTicker++;
			AddDataRow(nextReqId, contractDesc);
			if (getDelayedData)
			{
				Client.ClientSocket.reqMarketDataType(3);
			}
			Client.ClientSocket.reqMktData(nextReqId, contract, genericTickList, false, new List<TagValue>());

			if (!DataGrid.Visible)
			{
				DataGrid.Visible = true;
			}
		}

		public override void NotifyError(int requestId)
		{
			//ActiveRequests.RemoveAt(GetIndex(requestId));
			//CurrentTicker--;
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
			RequestIdToRow.Clear();
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

		public override void UpdateUI(IBMessage message)
		{
			MarketDataMessage dataMessage = (MarketDataMessage)message;
			if (RequestIdToRow.ContainsKey(dataMessage.RequestId))
			{
				DataGridView grid = (DataGridView)DataGrid;
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
					}
				}
			}
		}
	}
}
