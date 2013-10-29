using System;
using System.Globalization;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public static class ExtensionMethods
    {
        public static string WithPrefix(this String id, string prefix)
        {
            if (id.StartsWith(prefix))
                return id;
            else
                return prefix + id;
        }

        public static string WithoutPrefix(this String pid, string prefix)
        {
            string id = pid;
            while (id.StartsWith(prefix))
            { 
                id = pid.Substring(prefix.Length, pid.Length - prefix.Length);
            }
            return id;
        }

        //public static bool IsPid(this String str)
        //{
        //    string[] Prefix = { Plugin.SourcePrefix, Plugin.DevicePrefix, Plugin.FilePrefix, Plugin.TablePrefix };
        //    foreach (var prefix in Prefix)
        //    {
        //        if (str.StartsWith(prefix))
        //        {
        //            var num = str.Substring(prefix.Length, str.Length - prefix.Length);
        //            //if (num.Contains(Plugin.Delimiter)) { num = num.Substring(0, num.IndexOf(Plugin.Delimiter)); }
        //            decimal id;
        //            if (num.TryParseAsDecimal(out id) && num == Convert.ToString(Convert.ToInt32(id)))
        //            { 
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        
        
        /// <summary>
        /// Tries to parse the string as decimal value.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <param name="outValue">The parsed value.</param>
        /// <returns>True if parsing was successfull, false otherwise.</returns>
        public static bool TryParseAsDecimal(this String str, out decimal outValue)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture("sv-se");
            NumberFormatInfo nfi = ci.NumberFormat;

            if (!decimal.TryParse(str, NumberStyles.Number, nfi, out outValue))
            {
                //Try to parse the value using "." as decimal separator (US settings)
                ci = new CultureInfo("en-US");
                nfi = ci.NumberFormat;

                return decimal.TryParse(str, NumberStyles.Number, nfi, out outValue);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Parses the string as a decimal.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>The decimal representation of the string.</returns>
        public static Decimal ToDecimalByParsing(this String str)
        {
            Decimal value;

            if (!str.TryParseAsDecimal(out value))
            {
                throw new ArgumentException(String.Format(
                    "The string with data {0} could not be parsed as decimal.",
                    String.IsNullOrEmpty(str) ? "N/A" : str));
            }
            else
            {
                return value;
            }
        }

    }
}
    
    

