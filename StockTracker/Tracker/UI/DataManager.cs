using System.Windows.Forms;

namespace StockTracker.UI
{
	public abstract class DataManager
	{
		protected Control DataGrid;
		protected IBClient Client;
		protected int CurrentTicker = 1;

		public DataManager(IBClient client, Control dataGrid)
		{
			Client = client;
			DataGrid = dataGrid;
		}
		
		public abstract void NotifyError(int requestId);
		
		public abstract void Clear();

		public abstract void UpdateUI(IBMessage message);
	}
}
