using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegExTester
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void btnMatch_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(rtbRegEx.Text) ||
				string.IsNullOrEmpty(rtbInput.Text))
			{
				return;
			}
			rtbOutput.Clear();
			Match match = Regex.Match(rtbInput.Text, rtbRegEx.Text, RegexOptions.IgnoreCase);
			if (match.Success)
			{
				for (int i = 0; i < match.Groups.Count; ++i)
				{
					rtbOutput.AppendText($"Group {i}: {match.Groups[i].Value}\n");
					if (match.Groups[i].Captures.Count > 1)
					{
						foreach (Capture capture in match.Groups[i].Captures)
						{
							rtbOutput.AppendText($"\t{capture.Value.ToString()}\n");
						}
					}
				}
			}
			else
			{
				rtbOutput.Text = "No match!";
			}
		}
	}
}
