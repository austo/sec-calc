using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SecuritiesPositionCalculator
{
    public class Market
    {
        //private readonly ReaderWriterLockSlim
        //    _cacheLock = new ReaderWriterLockSlim();
        public readonly Dictionary<string, double>
            Securities = new Dictionary<string, double>();

        private int _consoleStartRow;
        private int _priceStartColumn;

        public double GetPrice(string secId)
        {
            Util.MarketLock.EnterReadLock();
            try
            {
                return Securities[secId];
            }
            finally
            {
                Util.MarketLock.ExitReadLock();
            }
        }

        public Market(TradeOrder order)
        {
            foreach (var trade in order.Trades
                .Where(trade => !Securities.ContainsKey(trade.SecurityId)))
            {
                Securities.Add(trade.SecurityId, trade.TradePrice);
            }
        }

        public void Start()
        {
            _consoleStartRow = Console.CursorTop + 1;
            var random = new Random(DateTime.Now.Millisecond);
            var secNames = Securities.Keys.ToArray();
            _priceStartColumn =
                secNames.OrderByDescending(s => s.Length).First().Length + 4;
            try
            {
                while (true)
                {
                    Util.MarketLock.EnterUpgradeableReadLock();
                    Fluctuate(random, secNames);
                    Util.MarketLock.ExitUpgradeableReadLock();
                    Thread.Sleep(random.Next() % 1000); // sleep for somewhere under a second
                }
            }
            catch (ThreadAbortException abEx)
            {
                Console.WriteLine(
                    string.Format("\nDue to {0}, the market has closed.",
                    abEx.ExceptionState));
            }
        }

        public void Close(PositionReport report, TradeOrder order)
        {
            Util.MarketLock.EnterUpgradeableReadLock();
            for (int i = 0, n = report.Count; i < n; i++)
            {
                // Get commodity quantity grouped by TradingBook name
                int index = i;
                double qty =
                    order.Trades.
                        Where(t => t.SecurityId.ToLower() ==
                            report[index].SecurityId.ToLower() &&
                                   t.TradingBook.Name.ToLower() ==
                                   report[index].TradingBook.Name.ToLower())
                                   .Sum(t => t.Quantity);

                Util.MarketLock.EnterWriteLock();
                report[i].MarketPrice =
                    Math.Round(Securities[report[i].SecurityId], 2);
                report[i].MarketValue =
                    Math.Round(report[i].MarketPrice * qty, 2);
                Util.MarketLock.ExitWriteLock();
            }
            Util.MarketLock.ExitUpgradeableReadLock();
        }

        private void Fluctuate(Random random, IList<string> secNames)
        {
            for (int i = 0, n = Securities.Count; i < n; i++)
            {
                var secName = secNames[i];
                var price = Securities[secName];
                var flux = price * ((random.NextDouble() % Cfg.Settings.Volitility) / 100);
                Util.MarketLock.EnterWriteLock();
                price += random.Next() % 2 == 0 ? flux : flux * -1;
                Securities[secName] = price;
                Util.MarketLock.ExitWriteLock();

                Console.SetCursorPosition(0, _consoleStartRow + i);
                Console.Write(string.Format("{0}:", secName));
                Console.CursorLeft = _priceStartColumn;
                Console.WriteLine(string.Format("{0:C}", price));

                if (random.Next() % 4 == 0) // simulate acyclic pause
                {
                    Thread.Sleep(random.Next() % 100);
                }
            }
        }
    }
}
