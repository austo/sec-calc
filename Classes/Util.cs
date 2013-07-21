using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SecuritiesPositionCalculator
{
    public static class Util
    {
        private static readonly Regex ActionRe = new Regex(@"^\s*(buy|sell)\s*$", RegexOptions.IgnoreCase);
        public static Action Parse(this Action action, string s)
        {
            if (ActionRe.IsMatch(s))
            {
                var temp = ActionRe.Match(s).ToString().Trim().ToLower();
                return temp == "buy" ? Action.Buy : Action.Sell;
            }
            throw new ArgumentException(string.Format("Invalid action type \"{0}\"", s));
        }

        public static bool ParseArgs(IList<string> args)
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
                else if (Cfg.Arguments.Flags.Volitility.IsMatch(args[i]))
                {
                    double d;
                    if (double.TryParse(args[++i], out d))
                    {
                        Cfg.Settings.Volitility = d;
                    }
                }
                else
                {
                    Console.WriteLine(
                        string.Format("Usage: {0} -i <infile>, -o <outfile>, -v <max volitility>", args[0]));
                    return false;
                }
            }
            return true;
        }
    }
}
