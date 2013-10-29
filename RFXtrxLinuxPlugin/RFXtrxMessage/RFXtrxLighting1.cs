using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    partial class RFXtrxLighting1
    {
        public RFXtrxLighting1()
        {
        }

        public RFXtrxLighting1(string subtype)
        {
            _SubType = subtype;
        }
        
        public override string ToString()
        {
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _HouseCode + _UnitCode + _Command + _Filler + _RSSI;
        }

        protected string PacketLength
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
        protected string PacketType
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
        public string HouseCode
        {
            get
            {
                return _HouseCode;
            }
            set
            {
                _HouseCode = value;
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
        protected string Filler
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

        protected string _PacketLength = "07";
        protected string _PacketType = "10";
        protected string _SubType = "01";
        protected string _SeqNbr = "00";
        protected string _HouseCode = "41"; // =A
        protected string _UnitCode = "01";
        protected string _Command = "00";
        protected string _Filler = "0";
        protected string _RSSI = "0";
    }
}

        //struct {
        //      BYTE packetlength; = 0x07 (this byte not included)
        //      BYTE packettype; = 0x10 = lighting1
        //      BYTE subtype;
        //      BYTE seqnbr; = 0x00 to 0xFF This field contains a sequence number
        //      BYTE housecode;
        //      BYTE unitcode;
        //      BYTE cmnd;
        //      BYTE filler : 4;
        //      BYTE rssi : 4;
        //} LIGHTING1;

        //subtype:
        //0x00 = X10 lighting
        //0x01 = ARC
        //0x02 = ELRO AB400D (Flamingo)
        //0x03 = Waveman
        //0x04 = Chacon EMW200
        //0x05 = IMPULS
        //0x06 = RisingSun
        //0x07 = Philips SBC
        //
        //housecode:
        //0x41 to 0x50 = house code A to P
        //Except:
        //EMW200 0x41 to 0x43 = house code A, B, C
        //RisingSun 0x41 to 0x44 = house code A, B, C, D
        //
        //unitcode:
        //0x01 to 0x10 = unit 1 to 16
        //Except:
        //AB400 and IMPULS = unit 1 to 64,
        //EMW200 and RisingSun = unit 1 to 4
        //Philips = unit 1 to 8
