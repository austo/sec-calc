using System;
using System.IO;
using System.Xml.Serialization;

namespace SecuritiesPositionCalculator
{
    [XmlRoot("TradeOrder",
        Namespace = Cfg.Settings.Namespace, IsNullable = false)]
    public class TradeOrder
    {
        [XmlArrayAttribute("Trades")]
        public Trade[] Trades;


        protected static void
            XUnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine(string.Format("Unknown node \"{0}\" at line {1}:\t\"{2}\".",
                e.Name, e.LineNumber, e.Text));
        }

        protected static void
            XUnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            var attr = e.Attr;
            Console.WriteLine(string.Format("Unknown attribute \"{0}\" with value \"{1}\" at {2}.",
                attr.Name, attr.Value, e.LineNumber));
        }

        public static TradeOrder ReadFromFile(string fileName)
        {
            XmlSerializer xSer = new XmlSerializer(typeof(TradeOrder));
            xSer.UnknownNode += XUnknownNode;
            xSer.UnknownAttribute += XUnknownAttribute;
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                return (TradeOrder)xSer.Deserialize(fs);
            }
        }

        public void Execute(Market market, PositionReport report)
        {
            // Compare market price to trade price, buy or sell accordingly
            Util.MarketLock.EnterUpgradeableReadLock();
            for (int i = 0, n = Trades.Length; i < n; i++)
            {
                if (Trades[i].Executed) { continue; }
                Action action;
                if (!Enum.TryParse(Trades[i].Action.Trim(), true, out action))
                {
                    continue;
                }

                var position = report.For(Trades[i].SecurityId, Trades[i].TradingBook.Name);
                switch (action)
                {
                    case Action.Sell:
                        if (market.Securities[Trades[i].SecurityId] >= Trades[i].TradePrice)
                        {
                            Util.MarketLock.EnterWriteLock();
                            position.ProfitLoss +=
                                market.Securities[Trades[i].SecurityId] * Trades[i].Quantity;
                            Trades[i].Executed = true;
                            report.Update(position);
                            Util.MarketLock.ExitWriteLock();
                        }
                        break;

                    case Action.Buy:
                        if (market.Securities[Trades[i].SecurityId] <= Trades[i].TradePrice)
                        {
                            Util.MarketLock.EnterWriteLock();
                            position.ProfitLoss -=
                                market.Securities[Trades[i].SecurityId] * Trades[i].Quantity;
                            Trades[i].Executed = true;
                            report.Update(position);
                            Util.MarketLock.ExitWriteLock();
                        }
                        break;
                }
            }
            Util.MarketLock.ExitUpgradeableReadLock();
        }
    }
}
