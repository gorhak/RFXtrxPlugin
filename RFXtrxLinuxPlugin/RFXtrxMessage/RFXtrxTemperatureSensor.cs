using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class TemperatureSensor
    {
        public TemperatureSensor()
        {
        }

        public override string ToString()
        {
            //Int64 temphigh = (Int64.Parse(ID, NumberStyles.HexNumber) & 0xFE);
            //string _TempHigh = temphigh.ToString("X").PadRight(2, '0');
            //Int64 tempsign = (Int64.Parse(ID, NumberStyles.HexNumber) & 0x01);
            //string _TempSign = temphigh.ToString("X").PadRight(2, '0');
            //Int64 templow = (Int64.Parse(ID, NumberStyles.HexNumber) & 0xFFFFFF);
            //string _TempLow = templow.ToString("X").PadLeft(6, '0');
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _ID + _Temperature + _BatteryLevel + _RSSI;
        }

        private string PacketLength
        {
            get
            {
                return _PacketLength;
            }
            set
            {
                _PacketLength = value;
            }
        }
        private string PacketType
        {
            get
            {
                return _PacketType;
            }
            set
            {
                _PacketType = value;
            }
        }
        public string SubType
        {
            get
            {
                return _SubType;
            }
            set
            {
                _SubType = value;
            }
        }
        public string SeqNbr
        {
            get
            {
                return _SeqNbr;
            }
            set
            {
                _SeqNbr = value;
            }
        }
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        //public string ID1
        //{
        //    get
        //    {
        //        return _ID1;
        //    }
        //    set
        //    {
        //        _ID1 = value;
        //    }
        //}
        //public string ID2
        //{
        //    get
        //    {
        //        return _ID2;
        //    }
        //    set
        //    {
        //        _ID2 = value;
        //    }
        //}
        public string Temperature
        {
            get
            {
                return _Temperature;
            }
            set
            {
                _Temperature = value;
            }
        }
        //public string TemperatureHigh
        //{
        //    get
        //    {
        //        return _TemperatureHigh;
        //    }
        //    set
        //    {
        //        _TemperatureHigh = value;
        //    }
        //}
        //public string TemperatureSign
        //{
        //    get
        //    {
        //        return _TemperatureSign;
        //    }
        //    set
        //    {
        //        _TemperatureSign = value;
        //    }
        //}
        //public string TemperatureLow
        //{
        //    get
        //    {
        //        return _TemperatureLow;
        //    }
        //    set
        //    {
        //        _TemperatureLow = value;
        //    }
        //}
        public string BatteryLevel
        {
            get
            {
                return _BatteryLevel;
            }
            set
            {
                _BatteryLevel = value;
            }
        }
        public string RSSI
        {
            get
            {
                return _RSSI;
            }
            set
            {
                _RSSI = value;
            }
        }

        protected string _PacketLength = "08";
        protected string _PacketType = "50";
        protected string _SubType = "00";
        protected string _SeqNbr = "00";
        protected string _ID = "0000";
        //protected string _ID1 = "00";
        //protected string _ID2 = "00";
        protected string _Temperature = "0000";
        //protected string _TemperatureHigh = "00";
        //protected string _TemperatureSign = "0";
        //protected string _TemperatureLow = "00";
        protected string _BatteryLevel = "0";
        protected string _RSSI = "0";
    }
}

            //struct {
            //        BYTE packetlength; = 0x08 Packet length (this byte not included)
            //        BYTE packettype; = 0x50 = temperature sensors
            //        BYTE subtype;
            //        BYTE seqnbr; = 0x00 to 0xFF Sequence number.
            //        BYTE id1; id1,2 Sensor ID.
            //        BYTE id2;
            //        BYTE temperaturehigh : 7;
            //        BYTE temperaturesign : 1; 0 = positive, 1 = negative
            //        BYTE temperaturelow;
            //        BYTE battery_level :4; 0x9 = full, 0x0 = empty
            //        BYTE rssi : 4; = 0x0 to 0xF = weak to strong. Signal strength.
            //}TEMP;

            //subtype:
            //0x01 = TEMP1 is THR128/138, THC138
            //0x02 = TEMP2 is THC238/268,THN132,THWR288,THRN122,THN122,AW129/131
            //0x03 = TEMP3 is THWR800
            //0x04 = TEMP4 is RTHN318
            //0x05 = TEMP5 is La Crosse TX2, TX3, TX4, TX17
            //0x06 = TEMP6 is TS15C
            //0x07 = TEMP7 is Viking 02811
            //0x08 = TEMP8 is La Crosse WS2300
            //0x09 = TEMP9 is RUBiCSON
            //0x0A = TEMP10 is TFA 30.3133
            //
            //temperaturehigh - temperaturelow:
            //7 bits high byte and 8 bits low byte = temperature * 10
