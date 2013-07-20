using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SecuritiesPositionCalculator;

namespace SecuritiesPositionCalculator
{
    public enum Action
    {
        Sell,
        Buy
    }

    public class Trade
    {
        public DateTime Date;
        public string Action;
        public int Quantity;
        public double TradePrice;
        public string SecurityId;
        public TradingBook TradingBook;
    }
}
