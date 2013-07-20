using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SecuritiesPositionCalculator
{
    public class Program
    {
        private static TradeOrder _tradeOrder;
        private static PositionReport _positionReport;

        static void Main(string[] args)
        {
            if (args.Count() != 1) // specified filenames
            {
                ParseArgs(args);
            }
            ReadTrades();
            WritePositions();
        }

        static void ParseArgs(IList<string> args)
        {
            for (int i = 0; i < args.Count(); ++i)
            {
                if (Cfg.Arguments.Flags.InFile.IsMatch(args[i]))
                {
                    Cfg.Settings.TradesFile = args[++i];
                }
                else if (Cfg.Arguments.Flags.OutFile.IsMatch(args[i]))
                {
                    Cfg.Settings.PositionsFile = args[++i];
                }
            }
        }

        private static void ReadTrades()
        {
            XmlSerializer xSer = new XmlSerializer(typeof(TradeOrder));
            xSer.UnknownNode += XUnknownNode;
            xSer.UnknownAttribute += XUnknownAttribute;
            using (FileStream fs = new FileStream(Cfg.Settings.TradesFile, FileMode.Open))
            {
                _tradeOrder = (TradeOrder)xSer.Deserialize(fs);
            }
        }

        private static void WritePositions()
        {
            _positionReport = new PositionReport { Name = "Positions" };

            // Test data
            Position p = new Position
                             {
                                 MarketPrice = 50.19,
                                 MarketValue = 50.21,
                                 ProfitLoss = .01,
                                 SecurityId = "C 9% 2014",
                                 SecurityType = SecurityType.Hedge,
                                 TradingBook = new TradingBook { Name = "SAC1" }
                             };

            _positionReport.Add(p);
            XmlSerializer x = new XmlSerializer(typeof(PositionReport));
            TextWriter writer = new StreamWriter(Cfg.Settings.PositionsFile);
            x.Serialize(writer, _positionReport);
        }


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

    }
}
