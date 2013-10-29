using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class RFXtrxLightingBase
    {
        public RFXtrxLightingBase()
        {
        }

        public string IntToHex(int int32)
        {
            return int32.ToString("X");
        }

        public string Int64ToHex(int int64)
        {
            return int64.ToString("X");
        }

        public int HexToInt(string hex)
        {
            return int.Parse(hex, NumberStyles.HexNumber);
        }
        
        public Int64 HexToInt64(string hex)
        {
            return Int64.Parse(hex, NumberStyles.HexNumber);
        }

        public string To_code(int id, int unit)
        {
            return id.ToString() + ":" + unit.ToString();
        }

        public string To_command(string Com)
        {
            return (Com == "00") ? "TurnOff" : "TurnOn";
        }

        public string To_Command(string com)
        {
            return (com == "TurnOff") ? "00" : "01";
        }

        public string To_level(string Lev)
        {
            return IntToHex((HexToInt(Lev) * 100 / 15));
        }

        public string To_SignalLevel(string Lev)
        {
            return IntToHex((HexToInt(Lev) * 15 / 100));
        }

    }
}
