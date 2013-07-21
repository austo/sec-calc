using System.Xml.Serialization;

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
        [XmlIgnoreAttribute]
        public int Quantity;

        public Position()
        {
        }

        public Position(Trade trade)
        {
            SecurityId = trade.SecurityId;
            TradingBook = trade.TradingBook;
            SecurityType = SecurityType.Option;
            Quantity = trade.Quantity;
        }
    }
}
