using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace SecuritiesPositionCalculator
{
    [XmlRoot("Positions",
        Namespace = Cfg.Settings.Namespace, IsNullable = false)]
    public class PositionReport : ICollection
    {
        public string Name;
        private readonly List<Position> _positions = new List<Position>();

        public Position this[int index]
        {
            get { return _positions[index]; }
        }

        public Position For(string securityId, string bookName)
        {
            return _positions
                .FirstOrDefault(p => p.SecurityId == securityId &&
                    p.TradingBook.Name.ToLower() == bookName.ToLower());
        }

        // Setter for updated position
        // (may not actually need this if members are accessed by reference)
        public void Update(Position position)
        {
            var old = _positions.First(p =>
                p.SecurityId.ToLower() == position.SecurityId.ToLower() &&
                p.TradingBook.Name.ToLower() == position.TradingBook.Name.ToLower());
            if (old == null) { return; }
            _positions.Remove(old);
            _positions.Add(position);
        }

        public IEnumerator GetEnumerator()
        {
            return _positions.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            _positions.CopyTo((Position[])array, index);
        }

        public int Count
        {
            get { return _positions.Count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public void Add(Position position)
        {
            _positions.Add(position);
        }

        private void CleanValues()
        {
            for (int i = 0, n = Count; i < n; i++)
            {
                this[i].ProfitLoss = Math.Round(this[i].ProfitLoss, 2);
            }
        }

        public void WriteToFile(string fileName)
        {
            CleanValues();
            XmlSerializer x = new XmlSerializer(typeof(PositionReport));
            TextWriter writer = new StreamWriter(fileName);
            x.Serialize(writer, this);
            Console.WriteLine(string.Format("Output file written to {0}",
                Cfg.Settings.PositionsFile));
        }

        public void WriteToScreen()
        {
            Console.WriteLine("\n\nYour positions:\n");
            foreach (Position position in this)
            {
                Console.WriteLine(string.Format("{0} in {1}:",
                    position.SecurityId, position.TradingBook.Name));
                Console.CursorLeft = Cfg.Settings.TabStop;
                Console.Write("Market price:");
                Console.CursorLeft = Cfg.Settings.ReportColumnWidth;
                Console.WriteLine(string.Format("{0:C}", position.MarketPrice));
                Console.CursorLeft = Cfg.Settings.TabStop;
                Console.Write("Market value:");
                Console.CursorLeft = Cfg.Settings.ReportColumnWidth;
                Console.WriteLine(string.Format("{0:C}", position.MarketValue));
                Console.CursorLeft = Cfg.Settings.TabStop;
                Console.Write("Profit/Loss:");
                Console.CursorLeft = Cfg.Settings.ReportColumnWidth;
                Console.WriteLine(string.Format("{0:C}", position.ProfitLoss));
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public PositionReport(string name, TradeOrder tradeOrder)
        {
            Name = name;

            // PositionReport is grouped by SecurityId and TradingBook name,
            // so filter out duplicates here.
            foreach (var trade in tradeOrder.Trades
                .Where(trade => !_positions
                    .Any(p =>
                        p.SecurityId.ToLower() == trade.SecurityId.ToLower() &&
                        p.TradingBook.Name.ToLower() == trade.TradingBook.Name.ToLower())))
            {
                _positions.Add(new Position(trade));
            }
        }
    }
}
