using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class RFXtrxLighting2 : RFXtrxLightingBase
    {
        public RFXtrxLighting2()
        {
        }

        public RFXtrxLighting2(string message) : this(message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        {
        }

        public RFXtrxLighting2(string[] msg)
        {
            _PacketLength = "0B";   // = msg[0];
            _PacketType = "11";     // = msg[1];
            _SubType = msg[2];
            _SeqNbr = msg[3];
            _ID = msg[4] + msg[5] + msg[6] + msg[7]; // Includes Filler1
            _Unit = msg[8];
            _Command = msg[9];
            _SignalLevel = msg[10];
            _Filler = "0";          //  = msg[11].Substring(0, 1);
            _RSSI = msg[11].Substring(1, 1);

            _id = To_id(_ID);
            _unit = HexToInt(_Unit);
            _command = To_command(_Command);
            _level = To_level(_SignalLevel);
        }

        private int To_id(string ID)
        {
            int idh = (int)(HexToInt64(ID) & 0x30000000 / 0x10);
            int idl = (int)(HexToInt64(ID) & 0xFFFFFF);
            return idh + idl;
        }

        private string To_ID(int id)
        {
            string IDh = IntToHex(id & 0x3000000 / 0x1000000).PadRight(2, '0');
            string IDl = IntToHex(id & 0xFFFFFF).PadLeft(6, '0');
            return IDh + IDl;
        }

        public override string ToString()
        {
            //int idh = (int.Parse(_ID, NumberStyles.HexNumber) & 0x30000000) / 0x4000000;
            //string _IDHigh = idh.ToString("X").PadRight(2, '0');    // Makes Filler1 to 0b000000
            //int idl = (int.Parse(_ID, NumberStyles.HexNumber) & 0xFFFFFF);
            //string _IDLow = idl.ToString("X").PadLeft(6, '0');
            //return _PacketLength + _PacketType + _SubType + _SeqNbr + _IDHigh + _IDLow + _Unit + _Command + _SignalLevel + _Filler + _RSSI;
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _ID + _Unit + _Command + _SignalLevel + _Filler + _RSSI;
        }

        private string PacketLength
        {
            get
            {
                return _PacketLength;
            }
            //set
            //{
            //    _PacketLength = value;
            //}
        }
        private string PacketType
        {
            get
            {
                return _PacketType;
            }
            //set
            //{
            //    _PacketType = value;
            //}
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
                _id = To_id(value);
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
                _unit = HexToInt(value);
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
                _command = To_command(value);
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
                _level = To_level(value);
            }
        }
        private string Filler
        {
            get
            {
                return _Filler;
            }
            //set
            //{
            //    _Filler = value;
            //}
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

        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                _ID = To_ID(value);
            }
        }
        public int unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
                _Unit = IntToHex(value);
            }
        }
        public string code
        {
            get
            {
                return To_code(_id, _unit);
            }
            //set
            //{
            //    _code = value;
            //}
        }
        public string command
        {
            get
            {
                return _command;
            }
            set
            {
                value = value.Replace(" ", ""); //.Replace("Turn O", "TurnO")
                _command = value;
                _Command = To_Command(value);
            }
        }
        public string level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
                _SignalLevel = To_SignalLevel(value);
            }
        }

        protected string _PacketLength = "0B";
        protected string _PacketType = "11";
        protected string _SubType;      // = "00";
        protected string _SeqNbr;       // = "00";
        protected string _ID;           // = "00000001"; // Includes Filler1
        protected string _Unit;         // = "01";
        protected string _Command;      // = "00";
        protected string _SignalLevel;  // = "00";
        protected string _Filler = "0";
        protected string _RSSI;         // = "0";

        protected int _id;              // = 1;
        protected int _unit;            // = 1;
        protected string _command;      // = "TurnOff";
        protected string _level;        // = "0";
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
