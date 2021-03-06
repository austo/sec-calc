﻿using System;
using System.Linq;
using System.Threading;

namespace SecuritiesPositionCalculator
{
    public static class Simulation
    {
        private static TradeOrder _tradeOrder;
        private static PositionReport _positionReport;
        private static Market _market;

        private static void Main(string[] args)
        {
            if (args.Count() != 1) // We have some arguments...
            {
                if (!Util.ParseArgs(args))
                {
                    return; // ...and they're invalid.
                }
            }

            try
            {
                ReadTrades();

                // Our "market" will run in a background thread.
                // The motivation for this to have "meaningful" Profit/Loss data. 
                _market = new Market(_tradeOrder);
                var marketStart = new ThreadStart(_market.Start);
                var marketThread = new Thread(marketStart) { IsBackground = true };

                var tradeStart = new ThreadStart(ExecuteTrades);
                var traderThread = new Thread(tradeStart) { IsBackground = true };

                Console.Beep(); // Too tempting
                Console.WriteLine("The market has started.\n" +
                                  "To see a report of your positions, press Enter.\n");

                marketThread.Start();
                traderThread.Start();

                Console.ReadLine();

                // TODO: there may be a better order for this
                _market.Close(_positionReport, _tradeOrder);

                traderThread.Abort("Exiting the market...");
                traderThread.Join();

                marketThread.Abort(Cfg.ClosingReason);
                marketThread.Join();

                WritePositions();

                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    string.Format("Unfortunately, the market has crashed. " +
                                  "Excuse:\n{0}", ex.Message));
                Console.ReadKey();
            }
        }

        private static void ReadTrades()
        {
            _tradeOrder = TradeOrder.ReadFromFile(Cfg.Settings.TradesFile);
            _positionReport = new PositionReport("Positions", _tradeOrder);
        }

        private static void WritePositions()
        {
            // Get prices, write PositionReport to file and screen.
            _positionReport.WriteToScreen();
            _positionReport.WriteToFile(Cfg.Settings.PositionsFile);
        }

        private static void ExecuteTrades()
        {
            var random = new Random(DateTime.Now.Millisecond);
            try
            {
                while (true)
                {
                    _tradeOrder.Execute(_market, _positionReport);

                    Thread.Sleep(random.Next() % Cfg.Settings.MaxTradeInterval);
                }
            }
            catch (ThreadAbortException abEx)
            {
                Console.WriteLine(
                    string.Format("\n{0}", abEx.ExceptionState));
            }
        }
    }
}