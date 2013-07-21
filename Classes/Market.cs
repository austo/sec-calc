using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SecuritiesPositionCalculator
{
    public class Market
    {
        private readonly ReaderWriterLockSlim 
            _cacheLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, double>
            _securities = new Dictionary<string, double>();

        private int _consoleStartRow;
        private int _priceStartColumn;

        public double GetPrice(string secId)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return _securities[secId];
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        public Market(TradeOrder order)
        {
            foreach (var trade in order.Trades
                .Where(trade => !_securities.ContainsKey(trade.SecurityId)))
            {
                _securities.Add(trade.SecurityId, trade.TradePrice);
            }
        }

        public void Start()
        {
            _consoleStartRow = Console.CursorTop + 1;
            var random = new Random(DateTime.Now.Millisecond);
            var secNames = _securities.Keys.ToArray();
            _priceStartColumn =
                secNames.OrderByDescending(s => s.Length).First().Length + 4;
            try
            {
                while (true)
                {
                    _cacheLock.EnterUpgradeableReadLock();
                    Fluctuate(random, secNames);
                    _cacheLock.ExitUpgradeableReadLock();
                    Thread.Sleep(random.Next()%1000); // sleep for somewhere under a second
                }
            }
            catch(ThreadAbortException abEx)
            {
                Console.WriteLine(
                    string.Format("\nDue to {0}, the market has closed.",
                    abEx.ExceptionState));
            }
        }

        public void Close(PositionReport report)
        {
            _cacheLock.EnterUpgradeableReadLock();
            for (int i = 0, n = report.Count; i < n; i++)
            {
                _cacheLock.EnterWriteLock();
                report[i].MarketPrice = _securities[report[i].SecurityId];
                report[i].MarketValue = report[i].MarketPrice*report[i].Quantity;
                _cacheLock.ExitWriteLock();
            }
            _cacheLock.ExitUpgradeableReadLock();
        }

        private void Fluctuate(Random random, IList<string> secNames)
        {
            for (int i = 0, n = _securities.Count; i < n; i++)
            {
                var secName = secNames[i];
                var price = _securities[secName];
                var flux = price * ((random.NextDouble() % Cfg.Settings.Volitility) / 100);
                _cacheLock.EnterWriteLock();
                price += random.Next() % 2 == 0 ? flux : flux * -1;
                _securities[secName] = price;
                _cacheLock.ExitWriteLock();

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
