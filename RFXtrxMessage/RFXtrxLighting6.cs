using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class RFXtrxLighting6
    {
        public RFXtrxLighting6()
        {
        }

        public RFXtrxLighting6(string subtype)
        {
            _SubType = subtype;
        }
        
        public override string ToString()
        {
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _ID + _GroupCode + _UnitCode + _Command + _CmndSeqNbr + _Rfu + _SignalLevel + _Filler + _RSSI;
        }

        public string PacketLength
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
        public string PacketType
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
        public string GroupCode
        {
            get
            {
                return _GroupCode;
            }
            set
            {
                _GroupCode = value;
            }
        }
        public string UnitCode
        {
            get
            {
                return _UnitCode;
            }
            set
            {
                _UnitCode = value;
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
        public string CmndSeqNbr
        {
            get
            {
                return _CmndSeqNbr;
            }
            set
            {
                _CmndSeqNbr = value;
            }
        }
        public string Rfu
        {
            get
            {
                return _Rfu;
            }
            set
            {
                _Rfu = value;
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
        public string Filler
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
        protected string _ID = "00000001";
        protected string _GroupCode;
        protected string _UnitCode;
        protected string _Command = "00";
        protected string _CmndSeqNbr;
        protected string _Rfu;
        protected string _SignalLevel = "00";
        protected string _Filler = "0";
        protected string _RSSI = "0";
    }
}

        //struct {
        //        BYTE packetlength;= 0x0B (this byte not included)
        //        BYTE packettype;  = 0x15 = lighting6
        //        BYTE subtype;     = 0x00 = Blyss
        //        BYTE seqnbr;      = 0x00 to 0xFF This field contains a sequence number
        //        BYTE id1;         id1 – id2: remote/switch/unit ID. (id1 is high byte)
        //        BYTE id2;
        //        BYTE groupcode;   = 0x41 to 0x44 = group A, B, C or D
        //        BYTE unitcode;    = 0x01 to 0x05 = unit 1 to 5
        //        BYTE cmnd;        = 0x00 = On, 0x01 = Off, 0x02 = group On, 0x03 = group Off
        //        BYTE cmndseqnbr;  = 0x00 to 0x04 (to be incremented after each command)
        //        BYTE rfu;         reserved for future use.
        //        BYTE filler : 4;  NA
        //        BYTE rssi : 4;    = 0x0 to 0xF = weak to strong (is 0x0 for transmitter command)
        //} LIGHTING6;
