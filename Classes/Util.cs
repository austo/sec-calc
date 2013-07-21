using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SecuritiesPositionCalculator
{
    public static class Util
    {
        public static readonly ReaderWriterLockSlim
            MarketLock = new ReaderWriterLockSlim();

        public static bool ParseArgs(IList<string> args)
        {
            for (int i = 0, n = args.Count(); i < n; ++i)
            {
                if (!Cfg.Args.Switch.IsMatch(args[i]))
                {
                    Console.WriteLine(
                        string.Format("Usage: {0} -i <infile>, -o <outfile>," +
                                      " -v <max volitility>", args[0]));
                    return false;
                }

                switch (Cfg.Args.Switch.Match(args[i]).Groups[1].Value.ToLower())
                {
                    case "i":
                        Cfg.Settings.TradesFile = args[++i];
                        break;
                    case "o":
                        Cfg.Settings.PositionsFile = args[++i];
                        break;
                    case "v":
                        double d;
                        if (double.TryParse(args[++i], out d))
                        {
                            Cfg.Settings.Volitility = d;
                        }
                        break;
                }
            }
            return true;
        }
    }
}
