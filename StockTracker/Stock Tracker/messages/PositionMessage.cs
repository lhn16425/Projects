﻿/* Copyright (C) 2013 Interactive Brokers LLC. All rights reserved.  This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;

namespace StockTracker.messages
{
    public class PositionMessage : IBMessage 
    {
        private string account;
        private Contract contract;
        private double position;
        private double averageCost;
        
        public PositionMessage(string account, Contract contract, double pos, double avgCost)
        {
            Type = MessageType.Position;
            Account = account;
            Contract = contract;
            Position = pos;
            AverageCost = avgCost;
        }

        public string Account
        {
            get { return account; }
            set { account = value; }
        }

        public Contract Contract
        {
            get { return contract; }
            set { contract = value; }
        }

        public double Position
        {
            get { return position; }
            set { position = value; }
        }
        
        public double AverageCost
        {
            get { return averageCost; }
            set { averageCost = value; }
        }
    }
}
