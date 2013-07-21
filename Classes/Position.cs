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

        public Position(Trade trade)
        {
            SecurityId = trade.SecurityId;
            TradingBook = trade.TradingBook;
            SecurityType = SecurityType.Option;
            ProfitLoss = 0;
        }
    }
}