using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
