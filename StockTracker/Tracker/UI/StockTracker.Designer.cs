namespace StockTracker
{
	partial class StockTracker
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ssStatus = new System.Windows.Forms.StatusStrip();
			this.tssLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.primaryExchLabel = new System.Windows.Forms.Label();
			this.symbol_label_TMD_MDT = new System.Windows.Forms.Label();
			this.btnStart = new System.Windows.Forms.Button();
			this.cbSecType = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.exchange_label_TMD_MDT = new System.Windows.Forms.Label();
			this.currency_label_TMD_MDT = new System.Windows.Forms.Label();
			this.tbSymbol = new System.Windows.Forms.TextBox();
			this.tbCurrency = new System.Windows.Forms.TextBox();
			this.tbExchange = new System.Windows.Forms.TextBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.cbTradingEnvironment = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.dgvMarketData = new System.Windows.Forms.DataGridView();
			this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Last = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BidPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AskPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tbLog = new System.Windows.Forms.TextBox();
			this.btnClearLog = new System.Windows.Forms.Button();
			this.ssStatus.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMarketData)).BeginInit();
			this.SuspendLayout();
			// 
			// ssStatus
			// 
			this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLabel});
			this.ssStatus.Location = new System.Drawing.Point(0, 829);
			this.ssStatus.Name = "ssStatus";
			this.ssStatus.Size = new System.Drawing.Size(1187, 22);
			this.ssStatus.TabIndex = 0;
			this.ssStatus.Text = "statusStrip1";
			// 
			// tssLabel
			// 
			this.tssLabel.Name = "tssLabel";
			this.tssLabel.Size = new System.Drawing.Size(117, 17);
			this.tssLabel.Text = "Status: Disconnected";
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.panel1, 0, 0);
			this.tlpMain.Controls.Add(this.dgvMarketData, 0, 1);
			this.tlpMain.Controls.Add(this.tbLog, 0, 2);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 3;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43.5F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 41.5F));
			this.tlpMain.Size = new System.Drawing.Size(1187, 829);
			this.tlpMain.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnClearLog);
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.btnConnect);
			this.panel1.Controls.Add(this.cbTradingEnvironment);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1181, 118);
			this.panel1.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnStop);
			this.groupBox2.Controls.Add(this.primaryExchLabel);
			this.groupBox2.Controls.Add(this.symbol_label_TMD_MDT);
			this.groupBox2.Controls.Add(this.btnStart);
			this.groupBox2.Controls.Add(this.cbSecType);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.exchange_label_TMD_MDT);
			this.groupBox2.Controls.Add(this.currency_label_TMD_MDT);
			this.groupBox2.Controls.Add(this.tbSymbol);
			this.groupBox2.Controls.Add(this.tbCurrency);
			this.groupBox2.Controls.Add(this.tbExchange);
			this.groupBox2.Location = new System.Drawing.Point(12, 38);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(430, 77);
			this.groupBox2.TabIndex = 56;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Market Data Subscription";
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(344, 43);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 2;
			this.btnStop.Text = "Stop All";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// primaryExchLabel
			// 
			this.primaryExchLabel.AutoSize = true;
			this.primaryExchLabel.Location = new System.Drawing.Point(8, 149);
			this.primaryExchLabel.Name = "primaryExchLabel";
			this.primaryExchLabel.Size = new System.Drawing.Size(71, 13);
			this.primaryExchLabel.TabIndex = 60;
			this.primaryExchLabel.Text = "Primary Exch.";
			// 
			// symbol_label_TMD_MDT
			// 
			this.symbol_label_TMD_MDT.AutoSize = true;
			this.symbol_label_TMD_MDT.Location = new System.Drawing.Point(6, 22);
			this.symbol_label_TMD_MDT.Name = "symbol_label_TMD_MDT";
			this.symbol_label_TMD_MDT.Size = new System.Drawing.Size(44, 13);
			this.symbol_label_TMD_MDT.TabIndex = 1;
			this.symbol_label_TMD_MDT.Text = "Symbol:";
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(344, 16);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 17;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// cbSecType
			// 
			this.cbSecType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSecType.FormattingEnabled = true;
			this.cbSecType.Items.AddRange(new object[] {
            "STK",
            "OPT",
            "FUT",
            "CASH",
            "BOND",
            "CFD",
            "FOP",
            "WAR",
            "IOPT",
            "FWD",
            "BAG",
            "IND",
            "BILL",
            "FUND",
            "FIXED",
            "SLB",
            "NEWS",
            "CMDTY",
            "BSK",
            "ICU",
            "ICS"});
			this.cbSecType.Location = new System.Drawing.Point(65, 44);
			this.cbSecType.Name = "cbSecType";
			this.cbSecType.Size = new System.Drawing.Size(100, 21);
			this.cbSecType.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "SecType:";
			// 
			// exchange_label_TMD_MDT
			// 
			this.exchange_label_TMD_MDT.AutoSize = true;
			this.exchange_label_TMD_MDT.Location = new System.Drawing.Point(174, 48);
			this.exchange_label_TMD_MDT.Name = "exchange_label_TMD_MDT";
			this.exchange_label_TMD_MDT.Size = new System.Drawing.Size(58, 13);
			this.exchange_label_TMD_MDT.TabIndex = 7;
			this.exchange_label_TMD_MDT.Text = "Exchange:";
			// 
			// currency_label_TMD_MDT
			// 
			this.currency_label_TMD_MDT.AutoSize = true;
			this.currency_label_TMD_MDT.Location = new System.Drawing.Point(174, 22);
			this.currency_label_TMD_MDT.Name = "currency_label_TMD_MDT";
			this.currency_label_TMD_MDT.Size = new System.Drawing.Size(52, 13);
			this.currency_label_TMD_MDT.TabIndex = 8;
			this.currency_label_TMD_MDT.Text = "Currency:";
			// 
			// tbSymbol
			// 
			this.tbSymbol.Location = new System.Drawing.Point(65, 18);
			this.tbSymbol.Name = "tbSymbol";
			this.tbSymbol.Size = new System.Drawing.Size(100, 20);
			this.tbSymbol.TabIndex = 0;
			this.tbSymbol.Text = "AAPL";
			// 
			// tbCurrency
			// 
			this.tbCurrency.Location = new System.Drawing.Point(238, 18);
			this.tbCurrency.Name = "tbCurrency";
			this.tbCurrency.Size = new System.Drawing.Size(100, 20);
			this.tbCurrency.TabIndex = 10;
			this.tbCurrency.Text = "USD";
			// 
			// tbExchange
			// 
			this.tbExchange.Location = new System.Drawing.Point(238, 45);
			this.tbExchange.Name = "tbExchange";
			this.tbExchange.Size = new System.Drawing.Size(100, 20);
			this.tbExchange.TabIndex = 11;
			this.tbExchange.Text = "NYSE";
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(224, 9);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(75, 23);
			this.btnConnect.TabIndex = 2;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// cbTradingEnvironment
			// 
			this.cbTradingEnvironment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTradingEnvironment.FormattingEnabled = true;
			this.cbTradingEnvironment.Items.AddRange(new object[] {
            "LIVE",
            "SIMULATED"});
			this.cbTradingEnvironment.Location = new System.Drawing.Point(123, 9);
			this.cbTradingEnvironment.Name = "cbTradingEnvironment";
			this.cbTradingEnvironment.Size = new System.Drawing.Size(95, 21);
			this.cbTradingEnvironment.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(108, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Trading Environment:";
			// 
			// dgvMarketData
			// 
			this.dgvMarketData.AllowUserToAddRows = false;
			this.dgvMarketData.AllowUserToDeleteRows = false;
			this.dgvMarketData.AllowUserToOrderColumns = true;
			this.dgvMarketData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMarketData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Description,
            this.Last,
            this.BidPrice,
            this.AskPrice});
			this.dgvMarketData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMarketData.Location = new System.Drawing.Point(3, 127);
			this.dgvMarketData.Name = "dgvMarketData";
			this.dgvMarketData.ReadOnly = true;
			this.dgvMarketData.Size = new System.Drawing.Size(1181, 354);
			this.dgvMarketData.TabIndex = 2;
			// 
			// Description
			// 
			this.Description.HeaderText = "Description";
			this.Description.Name = "Description";
			this.Description.ReadOnly = true;
			this.Description.Width = 200;
			// 
			// Last
			// 
			this.Last.HeaderText = "Last";
			this.Last.Name = "Last";
			this.Last.ReadOnly = true;
			// 
			// BidPrice
			// 
			this.BidPrice.HeaderText = "Bid";
			this.BidPrice.Name = "BidPrice";
			this.BidPrice.ReadOnly = true;
			// 
			// AskPrice
			// 
			this.AskPrice.HeaderText = "Ask";
			this.AskPrice.Name = "AskPrice";
			this.AskPrice.ReadOnly = true;
			// 
			// tbLog
			// 
			this.tbLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbLog.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbLog.Location = new System.Drawing.Point(3, 487);
			this.tbLog.Multiline = true;
			this.tbLog.Name = "tbLog";
			this.tbLog.ReadOnly = true;
			this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbLog.Size = new System.Drawing.Size(1181, 339);
			this.tbLog.TabIndex = 3;
			// 
			// btnClearLog
			// 
			this.btnClearLog.Location = new System.Drawing.Point(305, 9);
			this.btnClearLog.Name = "btnClearLog";
			this.btnClearLog.Size = new System.Drawing.Size(75, 23);
			this.btnClearLog.TabIndex = 57;
			this.btnClearLog.Text = "Clear Log";
			this.btnClearLog.UseVisualStyleBackColor = true;
			this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
			// 
			// StockTracker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1187, 851);
			this.Controls.Add(this.tlpMain);
			this.Controls.Add(this.ssStatus);
			this.Name = "StockTracker";
			this.Text = "Stock Tracker";
			this.ssStatus.ResumeLayout(false);
			this.ssStatus.PerformLayout();
			this.tlpMain.ResumeLayout(false);
			this.tlpMain.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMarketData)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip ssStatus;
		private System.Windows.Forms.ToolStripStatusLabel tssLabel;
		private System.Windows.Forms.TableLayoutPanel tlpMain;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ComboBox cbTradingEnvironment;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.DataGridView dgvMarketData;
		private System.Windows.Forms.DataGridViewTextBoxColumn Description;
		private System.Windows.Forms.DataGridViewTextBoxColumn Last;
		private System.Windows.Forms.DataGridViewTextBoxColumn BidPrice;
		private System.Windows.Forms.DataGridViewTextBoxColumn AskPrice;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Label primaryExchLabel;
		private System.Windows.Forms.Label symbol_label_TMD_MDT;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.ComboBox cbSecType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label exchange_label_TMD_MDT;
		private System.Windows.Forms.Label currency_label_TMD_MDT;
		private System.Windows.Forms.TextBox tbSymbol;
		private System.Windows.Forms.TextBox tbCurrency;
		private System.Windows.Forms.TextBox tbExchange;
		private System.Windows.Forms.TextBox tbLog;
		private System.Windows.Forms.Button btnClearLog;
	}
}

