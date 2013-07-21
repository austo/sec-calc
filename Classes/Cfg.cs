using System;
using System.Reflection;
using System.Configuration;
using System.Text.RegularExpressions;

namespace SecuritiesPositionCalculator
{
    public static class Cfg
    {
        public static class Settings
        {
            public static string TradesFile = "bad filename";
            public static string PositionsFile = "second bad filename";
            public static double Volitility = 8.5;

            public const string Namespace = "www.sac.com";

            static Settings()
            {
                var t = typeof(Settings);
                ReadConfigSettings(t);
            }
        }

        private static readonly string[] ClosingReasons = {
                                                             "excessive leverage", "insider trading",
                                                             "corporate malfeasance", "massive sell-off",
                                                             "pension obligations", "CFTC crackdown"
                                                         };

        public static string ClosingReason
        {
            get
            {
                var r = new Random();
                var index = r.Next()%ClosingReasons.Length - 1;
                return ClosingReasons[index];
            }
        }

        public static class Arguments
        {
            public static class Flags
            {
                public static Regex InFile = new Regex(@"-?i", RegexOptions.IgnoreCase);
                public static Regex OutFile = new Regex(@"-?o", RegexOptions.IgnoreCase);
                public static Regex Volitility = new Regex(@"-?v", RegexOptions.IgnoreCase);
            }
        }

        public static void ReadConfigSettings(Type t)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var f in fields)
            {
                object appSetting = "";
                string settingName = t.Name + "." + f.Name;
                {
                    if (appSettings[settingName] != null)
                    {
                        if (f.FieldType == typeof(string))
                        {
                            appSetting = ConfigurationManager.AppSettings[settingName];
                        }
                        else if (f.FieldType == typeof(int))
                        {
                            appSetting = int.Parse(ConfigurationManager.AppSettings[settingName]);
                        }
                        else if (f.FieldType == typeof(bool))
                        {
                            appSetting = bool.Parse(ConfigurationManager.AppSettings[settingName]);
                        }
                        else if (f.FieldType == typeof(Enum))
                        {
                            appSetting = Enum.Parse(f.FieldType, ConfigurationManager.AppSettings[settingName]);
                        }
                    }
                    if (appSetting.ToString() != "")
                    {
                        f.SetValue(null, appSetting);
                    }
                }
            }
        }
    }
}
