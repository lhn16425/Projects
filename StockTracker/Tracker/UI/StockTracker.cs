using IBApi;
using StockTracker.Messages;
using StockTracker.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace StockTracker
{
	public partial class StockTracker : Form
	{
		delegate void MessageHandler(IBMessage message);

		private MarketDataManager MDManager;

		//private Timer OneDayWPRTimer;
		//private Timer FiveDayWPRTimer;
		private Timer WPRTimer;

		public bool Connected { get; set; }
		private EReaderMonitorSignal MessageMonitor = new EReaderMonitorSignal();
		private IBClient Client;

		private const int MAX_LOG_SIZE = 200;
		private const int REDUCED_LOG_SIZE = 100;
		private List<string> LogContent = new List<string>(MAX_LOG_SIZE);

		private Dictionary<string, Tuple<string, string, string>> MasterList = new Dictionary<string, Tuple<string, string, string>>();

		public StockTracker()
		{
			InitializeComponent();
			cbTradingEnvironment.SelectedItem = "SIMULATED";
			cbSecType.SelectedItem = "STK";

			Connected = false;
			Client = new IBClient(MessageMonitor);
			MDManager = new MarketDataManager(Client, dgvMarketData);

			Client.NextValidId += Client_NextValidId;
			Client.ConnectionClosed += Client_ConnectionClosed;
			Client.Error += Client_Error;
			Client.TickPrice += Client_TickPrice;
			Client.RealtimeBar += (reqId, time, open, high, low, close, volume, WAP, count) => HandleMessage(new RealTimeBarMessage(reqId, time, open, high, low, close, volume, WAP, count));
			Client.HistoricalData += (reqId, date, open, high, low, close, volume, count, WAP, hasGaps) =>
				HandleMessage(new HistoricalDataMessage(reqId, date, open, high, low, close, volume, count, WAP, hasGaps));
			Client.HistoricalDataEnd += (reqId, startDate, endDate) => HandleMessage(new HistoricalDataEndMessage(reqId, startDate, endDate));

			LoadMasterList();
		}

		void LoadMasterList()
		{
			try
			{
				using (StreamReader sr = new StreamReader("MasterList.csv"))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						line = line.Trim();
						string[] parts = line.Split(',');
						if ((parts.Length >= 4) && !MasterList.ContainsKey(parts[0]))
						{
							MasterList.Add(parts[0], new Tuple<string, string, string>(parts[1], parts[2], parts[3]));
						}
					}
				}
#if DEBUG
				MasterList.Add("EUR", new Tuple<string, string, string>("CASH", "USD", "IDEALPRO"));
#endif
				tbSymbol.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
				tbSymbol.AutoCompleteSource = AutoCompleteSource.CustomSource;
				string[] keys = new string[MasterList.Keys.Count];
				MasterList.Keys.CopyTo(keys, 0);
				tbSymbol.AutoCompleteCustomSource.AddRange(keys);
			}
			catch (Exception)
			{
				throw;
			}
		}

		void Log(string msg)
		{
			//tbLog.AppendText(string.Format("{0}\n", msg));
			if (!msg.EndsWith("\n"))
			{
				msg += "\n";
			}

			if (LogContent.Count >= MAX_LOG_SIZE)
			{
				LogContent.RemoveRange(0, MAX_LOG_SIZE - REDUCED_LOG_SIZE);
				tbLog.Lines = LogContent.ToArray();
			}

			LogContent.Add(msg);
			tbLog.AppendText(msg);
		}

		void Client_NextValidId(int orderId)
		{
			Connected = true;
			HandleMessage(new ConnectionStatusMessage(true));
		}

		void CalculateWPR(object wprType)
		{
			WPRType type = (WPRType)wprType;
			HandleMessage(new CalculateWPRMessage(type));
		}

		void Client_ConnectionClosed()
		{
			Connected = false;
			HandleMessage(new ConnectionStatusMessage(false));
		}

		void Client_TickPrice(int tickerId, int field, double price, int canAutoExecute)
		{
			//HandleMessage(new LogMessage($"Tick Price - RequestId: {tickerId}, Type: {TickType.getField(field)}, Price: {price}"));

			//if ((field == TickType.LOW) ||
			//	(field == TickType.DELAYED_LOW) ||
			//	(field == TickType.HIGH) ||
			//	(field == TickType.DELAYED_HIGH) ||
			//	(field == TickType.CLOSE) ||
			//	(field == TickType.DELAYED_CLOSE))
			//{
			//	HandleMessage(new LogMessage($"Tick Price - RequestId: {tickerId}, Type: {TickType.getField(field)}, Price: {price}"));
			//}
			HandleMessage(new TickPriceMessage(tickerId, field, price, canAutoExecute));
		}

		void Client_Error(int id, int errorCode, string message, Exception ex)
		{
			if (ex != null)
			{
				HandleMessage(new ErrorMessage(ex.ToString()));
				return;
			}

			if (id == 0 || errorCode == 0)
			{
				HandleMessage(new ErrorMessage(message));
				return;
			}

			HandleMessage(new ErrorMessage(id, errorCode, message));
		}

		public void HandleMessage(IBMessage message)
		{
			if (this.InvokeRequired)
			{
				MessageHandler callback = new MessageHandler(HandleMessage);
				this.Invoke(callback, new object[] { message });
			}
			else
			{
				UpdateUI(message);
			}
		}

		private string GetString(int value, string name)
		{
			return (value <= 0) ? string.Empty : $"{name}={value}";
		}

		private void UpdateUI(IBMessage message)
		{
			switch (message.Type)
			{
				case MessageType.ConnectionStatus:
					{
						ConnectionStatusMessage status = (ConnectionStatusMessage)message;
						if (status.Connected)
						{
							tssLabel.Text = "Status: Connected - Client ID = " + Client.ClientId;
							btnConnect.Text = "Disconnect";

							//OneDayWPRTimer = new Timer(new TimerCallback(CalculateWPR), WPRType.OneDay, 0, 60 * 1000);
							//FiveDayWPRTimer = new Timer(new TimerCallback(CalculateWPR), WPRType.FiveDay, 0, 60 * 1000);
							WPRTimer = new Timer(new TimerCallback(CalculateWPR), WPRType.Both, 0, 60 * 1000);
						}
						else
						{
							tssLabel.Text = "Status: Disconnected";
							btnConnect.Text = "Connect";
						}
						break;
					}
				case MessageType.Error:
					{
						ErrorMessage error = (ErrorMessage)message;
						string requestId = GetString(error.RequestId, "RequestId");
						string code = GetString(error.Code, "Code");
						Log($"{(string.IsNullOrEmpty(requestId) ? string.Empty : requestId + ", ")}" +
							$"{(string.IsNullOrEmpty(code) ? string.Empty : code + " - ")}" +
							$"{error.Message}");
						break;
					}
				case MessageType.Log:
					{
						LogMessage log = (LogMessage)message;
						Log(log.Message);
						break;
					}
				case MessageType.CalculateWPR:
					{
						Log(string.Format("{0:MM/dd/yyyy HH:mm:ss} - Calculate WPR", DateTime.Now));
						MDManager.UpdateUI(message);
						break;
					}
				case MessageType.TickPrice:
					{
						MDManager.UpdateUI(message);
						break;
					}
				case MessageType.HistoricalDataEnd:
					{
						HistoricalDataEndMessage historicalDataEndMessage = (HistoricalDataEndMessage)message;
						HandleMessage(new LogMessage(historicalDataEndMessage.ToString()));
						MDManager.UpdateHistoricalData(message);
						break;
					}
				case MessageType.HistoricalData:
					{
						HistoricalDataMessage historicalDataMessage = (HistoricalDataMessage)message;
						HandleMessage(new LogMessage(historicalDataMessage.ToString()));
						MDManager.UpdateHistoricalData(message);
						break;
					}
				case MessageType.RealTimeBars:
					{
						RealTimeBarMessage rtbMessage = (RealTimeBarMessage)message;
						HandleMessage(new LogMessage(rtbMessage.ToString()));
						MDManager.UpdateRealTimeData(rtbMessage);
						break;
					}
				//case MessageType.ScannerData:
				//case MessageType.ScannerParameters:
				//	{
				//		scannerManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.OpenOrder:
				//case MessageType.OpenOrderEnd:
				//case MessageType.OrderStatus:
				//case MessageType.ExecutionData:
				//case MessageType.CommissionsReport:
				//	{
				//		orderManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.ManagedAccounts:
				//	{
				//		orderManager.ManagedAccounts = ((ManagedAccountsMessage)message).ManagedAccounts;
				//		accountManager.ManagedAccounts = ((ManagedAccountsMessage)message).ManagedAccounts;
				//		exerciseAccount.Items.AddRange(((ManagedAccountsMessage)message).ManagedAccounts.ToArray());
				//		break;
				//	}
				//case MessageType.AccountSummaryEnd:
				//	{
				//		accSummaryRequest.Text = "Request";
				//		accountManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.AccountDownloadEnd:
				//	{
				//		break;
				//	}
				//case MessageType.AccountUpdateTime:
				//	{
				//		accUpdatesLastUpdateValue.Text = ((UpdateAccountTimeMessage)message).Timestamp;
				//		break;
				//	}
				//case MessageType.PortfolioValue:
				//	{
				//		accountManager.UpdateUI(message);
				//		if (exerciseAccount.SelectedItem != null)
				//			optionsManager.HandlePosition((UpdatePortfolioMessage)message);
				//		break;
				//	}
				//case MessageType.AccountSummary:
				//case MessageType.AccountValue:
				//case MessageType.Position:
				//case MessageType.PositionEnd:
				//	{
				//		accountManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.ContractDataEnd:
				//	{
				//		searchContractDetails.Enabled = true;
				//		contractManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.ContractData:
				//	{
				//		HandleContractDataMessage((ContractDetailsMessage)message);
				//		break;
				//	}
				//case MessageType.FundamentalData:
				//	{
				//		fundamentalsQueryButton.Enabled = true;
				//		contractManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.ReceiveFA:
				//	{
				//		advisorManager.UpdateUI((AdvisorDataMessage)message);
				//		break;
				//	}
				//case MessageType.PositionMulti:
				//case MessageType.AccountUpdateMulti:
				//case MessageType.PositionMultiEnd:
				//case MessageType.AccountUpdateMultiEnd:
				//	{
				//		acctPosMultiManager.UpdateUI(message);
				//		break;
				//	}

				//case MessageType.SecurityDefinitionOptionParameter:
				//case MessageType.SecurityDefinitionOptionParameterEnd:
				//	{
				//		optionsManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.SoftDollarTiers:
				//	{
				//		orderManager.UpdateUI(message);
				//		break;
				//	}

				default:
					{
						HandleMessage(new ErrorMessage(message.ToString()));
						break;
					}
			}
		}

		private void btnConnect_Click(object sender, EventArgs e)
		{
			if (!Connected)
			{
				try
				{
					int port = 7497;
					if (cbTradingEnvironment.SelectedItem.ToString() == "LIVE")
					{
						port = 7496;
					}
					Client.ClientId = 1;
					Client.ClientSocket.eConnect("127.0.0.1", port, Client.ClientId);

					var reader = new EReader(Client.ClientSocket, MessageMonitor);

					reader.Start();

					new Thread(() => { while (Client.ClientSocket.IsConnected()) { MessageMonitor.waitForSignal(); reader.processMsgs(); } }) { IsBackground = true }.Start();
				}
				catch (Exception)
				{
					HandleMessage(new ErrorMessage(-1, -1, "Please check your connection attributes."));
				}
			}
			else
			{
				Shutdown();
			}
		}

		private void Shutdown()
		{
			Connected = false;
			MDManager.StopActiveRequests(true);
			//OneDayWPRTimer.Dispose();
			//FiveDayWPRTimer.Dispose();
			if (WPRTimer != null)
			{
				WPRTimer.Dispose();
			}
			if (Client.ClientSocket != null)
			{
				Client.ClientSocket.eDisconnect();
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (Connected)
			{
				Contract contract = GetMDContract();
				if (!MDManager.AddRequest(contract, string.Empty, (cbTradingEnvironment.SelectedItem.ToString() == "SIMULATED")))
				{
					Log(string.Format("Error: you can only track at most {0} symbols/instruments at a time.", MarketDataManager.MAX_SUBSCRIPTIONS_ALLOWED));
				}
			}
		}

		private Contract GetMDContract()
		{
			string exchange = tbExchange.Text.ToUpper().Trim();
			return new Contract
			{
				Symbol = tbSymbol.Text.ToUpper().Trim(),
				SecType = cbSecType.SelectedItem.ToString(),
				Currency = tbCurrency.Text.ToUpper().Trim(),
				Exchange = (exchange == "NASDAQ") ? "ISLAND" : exchange // On the API side, NASDAQ is always defined as ISLAND in the exchange field
			};
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			MDManager.StopActiveRequests(true);
		}

		private void btnClearLog_Click(object sender, EventArgs e)
		{
			tbLog.Clear();
			LogContent.Clear();
		}

		private void dgvMarketData_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 6)
			{
				Log("Buy Buy Buy");
				//dgvMarketData[6, 0].Value = "Buy";
				//dgvMarketData[6, 0].
			}
		}

		private void tbSymbol_Validated(object sender, EventArgs e)
		{
			string symbol = tbSymbol.Text.Trim().ToUpper();
			if (MasterList.ContainsKey(symbol))
			{
				cbSecType.SelectedItem = MasterList[symbol].Item1;
				tbCurrency.Text = MasterList[symbol].Item2;
				tbExchange.Text = MasterList[symbol].Item3;
			}
		}

		private void tbSymbol_Click(object sender, EventArgs e)
		{
			tbSymbol.SelectAll();
		}

		private void StockTracker_FormClosing(object sender, FormClosingEventArgs e)
		{
			Shutdown();
		}
	}
}
