namespace RegExTester
{
	partial class Form1
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
			this.label1 = new System.Windows.Forms.Label();
			this.rtbRegEx = new System.Windows.Forms.RichTextBox();
			this.rtbInput = new System.Windows.Forms.RichTextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnMatch = new System.Windows.Forms.Button();
			this.rtbOutput = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "RegEx:";
			// 
			// rtbRegEx
			// 
			this.rtbRegEx.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtbRegEx.Location = new System.Drawing.Point(60, 6);
			this.rtbRegEx.Multiline = false;
			this.rtbRegEx.Name = "rtbRegEx";
			this.rtbRegEx.Size = new System.Drawing.Size(728, 96);
			this.rtbRegEx.TabIndex = 1;
			this.rtbRegEx.Text = "";
			// 
			// rtbInput
			// 
			this.rtbInput.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtbInput.Location = new System.Drawing.Point(60, 117);
			this.rtbInput.Name = "rtbInput";
			this.rtbInput.Size = new System.Drawing.Size(728, 96);
			this.rtbInput.TabIndex = 3;
			this.rtbInput.Text = "";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 120);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Input:";
			// 
			// btnMatch
			// 
			this.btnMatch.Location = new System.Drawing.Point(7, 224);
			this.btnMatch.Name = "btnMatch";
			this.btnMatch.Size = new System.Drawing.Size(47, 23);
			this.btnMatch.TabIndex = 4;
			this.btnMatch.Text = "Match";
			this.btnMatch.UseVisualStyleBackColor = true;
			this.btnMatch.Click += new System.EventHandler(this.btnMatch_Click);
			// 
			// rtbOutput
			// 
			this.rtbOutput.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtbOutput.Location = new System.Drawing.Point(60, 226);
			this.rtbOutput.Name = "rtbOutput";
			this.rtbOutput.ReadOnly = true;
			this.rtbOutput.Size = new System.Drawing.Size(728, 393);
			this.rtbOutput.TabIndex = 5;
			this.rtbOutput.Text = "";
			// 
			// Form1
			// 
			this.AcceptButton = this.btnMatch;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 633);
			this.Controls.Add(this.rtbOutput);
			this.Controls.Add(this.btnMatch);
			this.Controls.Add(this.rtbInput);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.rtbRegEx);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "RegEx Tester";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox rtbRegEx;
		private System.Windows.Forms.RichTextBox rtbInput;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnMatch;
		private System.Windows.Forms.RichTextBox rtbOutput;
	}
}

