using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SecuritiesPositionCalculator;

namespace SecuritiesPositionCalculator
{
    public class Trade
    {
        public DateTime Date;
        public Action Action;
        public int Quantity;
        public double TradePrice;
        public string SecurityId;
        public TradingBook TradingBook;
    }
}
