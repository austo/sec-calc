using System;

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
