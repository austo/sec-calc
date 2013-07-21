using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace SecuritiesPositionCalculator
{
    [XmlRoot("Positions",
        Namespace = Cfg.Settings.Namespace, IsNullable = false)]
    public class PositionReport : ICollection
    {
        public string Name;
        private readonly ArrayList _positions = new ArrayList();

        public Position this[int index]
        {
            get { return (Position)_positions[index]; }
        }

        public IEnumerator GetEnumerator()
        {
            return _positions.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            _positions.CopyTo(array, index);
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

        public void Write(string fileName)
        {
            XmlSerializer x = new XmlSerializer(typeof(PositionReport));
            TextWriter writer = new StreamWriter(fileName);
            x.Serialize(writer, this);
        }

        public PositionReport(string name, TradeOrder tradeOrder)
        {
            Name = name;
            foreach (var trade in tradeOrder.Trades)
            {
                _positions.Add(new Position(trade));
            }
        }
    }
}
