using IBApi;
using StockTracker.Messages;
using StockTracker.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace StockTracker
{
	public partial class StockTracker : Form
	{
		delegate void MessageHandler(IBMessage message);

		private MarketDataManager MDManager;

		public bool Connected { get; set; }
		private EReaderMonitorSignal MessageMonitor = new EReaderMonitorSignal();
		private IBClient Client;

		private const int MAX_LOG_SIZE = 200;
		private const int REDUCED_LOG_SIZE = 100;
		private List<string> LogContent = new List<string>(MAX_LOG_SIZE);

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

		void Client_ConnectionClosed()
		{
			Connected = false;
			HandleMessage(new ConnectionStatusMessage(false));
		}

		void Client_TickPrice(int tickerId, int field, double price, int canAutoExecute)
		{
			HandleMessage(new LogMessage(string.Format("Tick Price - RequestId: {0}, Type: {1}, Price: {2}", tickerId, TickType.getField(field), price)));
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

		private void HandleTickMessage(MarketDataMessage tickMessage)
		{
			MDManager.UpdateUI(tickMessage);
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
						if (error.RequestId <= 0)
						{
							Log(string.Format("Code={0} - {1}", error.Code, error.Message));
						}
						else
						{
							Log(string.Format("RequestId={0}, Code={1} - {2}", error.RequestId, error.Code, error.Message));
						}
						break;
					}
				case MessageType.Log:
					{
						LogMessage log = (LogMessage)message;
						Log(log.Message);
						break;
					}
				//case MessageType.TickOptionComputation:
				case MessageType.TickPrice:
				case MessageType.TickSize:
					{
						HandleTickMessage((MarketDataMessage)message);
						break;
					}
				//case MessageType.MarketDepth:
				//case MessageType.MarketDepthL2:
				//	{
				//		deepBookManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.HistoricalData:
				//case MessageType.HistoricalDataEnd:
				//	{
				//		historicalDataManager.UpdateUI(message);
				//		break;
				//	}
				//case MessageType.RealTimeBars:
				//	{
				//		realTimeBarManager.UpdateUI(message);
				//		break;
				//	}
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
				Connected = false;
				Client.ClientSocket.eDisconnect();
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (Connected)
			{
				Contract contract = GetMDContract();
				MDManager.AddRequest(contract, string.Empty, (cbTradingEnvironment.SelectedItem.ToString() == "SIMULATED"));
			}
		}

		private Contract GetMDContract()
		{
			return new Contract
			{
				Symbol = tbSymbol.Text.ToUpper().Trim(),
				SecType = cbSecType.SelectedItem.ToString(),
				Currency = tbCurrency.Text.ToUpper().Trim(),
				Exchange = tbExchange.Text.ToUpper().Trim()
			};
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			MDManager.StopActiveRequests(true);
		}

		private void btnClearLog_Click(object sender, EventArgs e)
		{
			tbLog.Clear();
		}
	}
}
