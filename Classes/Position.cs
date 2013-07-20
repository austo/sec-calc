using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecuritiesPositionCalculator
{
    public enum SecurityType
    {
        Stock,
        Bond,
        Option,
        Future,
        Hedge
    }

    public class Position
    {
        public string SecurityId;
        public SecurityType SecurityType;
        public TradingBook TradingBook;
        public double MarketPrice;
        public double MarketValue;
        public double ProfitLoss;
    }
}
