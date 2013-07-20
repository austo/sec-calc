using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
