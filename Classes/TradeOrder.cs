using System.Xml.Serialization;

namespace SecuritiesPositionCalculator
{
    [XmlRoot("TradeOrder",
        Namespace = Cfg.Settings.Namespace, IsNullable = false)]
    public class TradeOrder
    {
        [XmlArrayAttribute("Trades")] public Trade[] Trades;
    }
}
