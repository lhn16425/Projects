﻿using System;

namespace StockTracker
{
	// https://www.investopedia.com/terms/w/williamsr.asp
	class WPRCalculator
	{
		public double FiveDayHigh { set; get; } = -1;
		public double FiveDayLow { set; get; } = -1;
		public double OneDayHigh { set; get; } = -1;
		public double OneDayLow { set; get; } = -1;
		private readonly object locker = new object();
		private double close = -1;
		public double Close
		{
			set { lock (locker) { close = value; } }
			get { return close; }
		}
		public double LatestHigh { set; get; } = -1;
		public double LatestLow { set; get; } = -1;
		public double Latest1DayWPR { get; private set; }
		public double Latest5DayWPR { get; private set; }
		public WPRCalculator() {}
		private double CalcWPR(double historicalHigh, double historicalLow)
		{
			if ((historicalHigh <= 0) ||
				(historicalLow <= 0) ||
				(Close <= 0) ||
				(LatestHigh <= 0) ||
				(LatestLow <= 0))
			{
				return 1; // valid WPR value moves between 0 and -100
			}
			double highestHigh = Math.Max(historicalHigh, LatestHigh);
			double lowestLow = Math.Min(historicalLow, LatestLow);
			return ((highestHigh - Close) / (highestHigh - lowestLow)) * -100;
		}
		public double Get5DayWPR()
		{
			Latest5DayWPR = CalcWPR(FiveDayHigh, FiveDayLow);
			return Latest5DayWPR;
		}
		public double Get1DayWPR()
		{
			Latest1DayWPR = CalcWPR(OneDayHigh, OneDayLow);
			return Latest1DayWPR;
		}
	}
}
