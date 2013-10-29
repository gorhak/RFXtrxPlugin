using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class RFXtrxLighting2
    {
        public RFXtrxLighting2()
        {
        }

        public RFXtrxLighting2(string subtype)
        {
            _SubType = subtype;
        }
        
        public override string ToString()
        {
            Int64 idh = (Int64.Parse(ID, NumberStyles.HexNumber) & 0x30000000) / 0x4000000;
            string _IDHigh = idh.ToString("X").PadRight(2, '0');    // Makes Filler1 to 0b000000
            Int64 idl = (Int64.Parse(ID, NumberStyles.HexNumber) & 0xFFFFFF);
            string _IDLow = idl.ToString("X").PadLeft(6, '0');
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _IDHigh + _IDLow + _Unit + _Command + _SignalLevel + _Filler + _RSSI;
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
        public string Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                _Unit = value;
            }
        }
        public string Command
        {
            get
            {
                return _Command;
            }
            set
            {
                _Command = value;
            }
        }
        public string SignalLevel
        {
            get
            {
                return _SignalLevel;
            }
            set
            {
                _SignalLevel = value;
            }
        }
        private string Filler
        {
            get
            {
                return _Filler;
            }
            set
            {
                _Filler = value;
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

        protected string _PacketLength = "0B";
        protected string _PacketType = "11";
        protected string _SubType = "00";
        protected string _SeqNbr = "00";
        protected string _ID = "00000001"; // Includes Filler1
        protected string _Unit = "01";
        protected string _Command = "00";
        protected string _SignalLevel = "00";
        protected string _Filler = "0";
        protected string _RSSI = "0";

    }
}

        //struct {
        //        BYTE packetlength;= 0x0B (this byte not included)
        //        BYTE packettype;  = 0x11
        //        BYTE subtype;     = 0x00 = AC, 0x01 = HomeEasy EU, 0x02 = ANSLUT
        //        BYTE seqnbr;      = 0x00 to 0xFF a sequence number if used
        //        BYTE id1 : 2;     id1 – id4: = 0x00 00 00 01 to 0x03 FF FF FF (id1 is high byte)
        //        BYTE filler1 : 6; NA
        //        BYTE id2;
        //        BYTE id3;
        //        BYTE id4;
        //        BYTE unitcode;    = 0x01 to 0x10, unit 1 to 16
        //        BYTE cmnd;        = 0x00 = Off, 0x01 = On, 0x02 = set level, 0x03 = group Off, 0x04 = group On, 0x05 = set group level
        //        BYTE level;       = 0x0 to 0xF = 0% to 100%
        //        BYTE filler2 : 4; NA
        //        BYTE rssi : 4;    = 0x0 to 0xF = weak to strong (is 0x0 for transmitter command)
        //} LIGHTING2;
