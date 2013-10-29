using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class RFXtrxRFXsensor
    {
        public RFXtrxRFXsensor()
        {
        }

        public override string ToString()
        {
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _ID + _Message + _Filler + _RSSI;
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
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
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

        protected string _PacketLength = "06";
        protected string _PacketType = "70";
        protected string _SubType = "00";
        protected string _SeqNbr = "00";
        protected string _ID = "00";
        protected string _Message = "0000";
        protected string _Filler = "0";
        protected string _RSSI = "0";
    }
}

        //struct {
        //        BYTE packetlength; = 0x06 Packet length (this byte not included)
        //        BYTE packettype; = 0x70 = RFXSensor
        //        BYTE subtype;
        //        BYTE seqnbr; = 0x00 to 0xFF Sequence number.
        //        BYTE id; Sensor ID.
        //        BYTE msg1;
        //        BYTE msg2;
        //        BYTE filler : 4;
        //        BYTE rssi : 4; = 0x0 to 0xF = weak to strong. Signal strength.
        //}RFXSENSOR;

        //subtype:
        //0x00 = RFXSensor temperature
        //0x01 = RFXSensor A/D
        //0x02 = RFXSensor voltage
        //0x03 = RFXSensor message
        //
        //msg1-msg2 for subtype 00, 01 or 10
        //measured value in Celsius * 100.
        //Bit 7 in msg1 is a sign bit. For a negative temperature this bit is 1.
        //
        //msg1-msg2 for subtype 01 or 10
        //measured value in mV
        //
        //msg1-msg2 for subtype 11
        //0x0001 = sensor addresses incremented
        //0x0002 = battery low detected
        //0x0081 = no 1-wire device connected
        //0x0082 = 1-Wire ROM CRC error
        //0x0083 = 1-Wire device connected is not a DS18B20 or DS2438
        //0x0084 = no end of read signal received from 1-Wire device
        //0x0085 = 1-Wire scratchpad CRC error
