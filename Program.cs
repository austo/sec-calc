using System;
using System.Linq;
using System.Threading;

namespace SecuritiesPositionCalculator
{
    public class Program
    {
        private static TradeOrder _tradeOrder;
        private static PositionReport _positionReport;

        static void Main(string[] args)
        {
            if (args.Count() != 1) // We have some options...
            {
                if (!Util.ParseArgs(args))
                {
                    return; // ...and they're wrong
                }
            }

            ReadTrades();

            // Our "market" will run in a background thread.
            // The motivation for this to have a more meaningful datum for ProfitLoss. 
            var market = new Market(_tradeOrder);
            var marketStart = new ThreadStart(market.Start);
            var marketThread = new Thread(marketStart) { IsBackground = true };

            Console.Beep(); // Too tempting
            Console.WriteLine("The market has started.\n" +
                              "To see a report of your positions, press Enter.\n");
            
            marketThread.Start();

            Console.ReadLine();
            WritePositions();
        }

        private static void ReadTrades()
        {
            _tradeOrder = TradeOrder.ReadFromFile(Cfg.Settings.TradesFile);
        }

        private static void WritePositions()
        {
            // Fight with the market thread's ReaderWriterLock to get prices,
            // then build PositionReport and write to file and screen.
            _positionReport = new PositionReport("Positions", _tradeOrder);
            _positionReport.Write(Cfg.Settings.PositionsFile);
        }
    }
}
