using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class RFXtrxRXResponse
    {
        public RFXtrxRXResponse()
        {
        }

        public override string ToString()
        {
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _Message;
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

        protected string _PacketLength = "04";
        protected string _PacketType = "02";
        protected string _SubType = "00";
        protected string _SeqNbr = "00";
        protected string _Message = "00";
    }
}

        //struct {
        //        BYTE packetlength; Packet length (this byte not included) = 0x04
        //        BYTE packettype; 0x02 = receiver/transmitter message
        //        BYTE subtype;
        //        BYTE seqnbr; Sequence number. If not used leave it zero.
        //        BYTE msg;
        //} RXRESPONSE;

        //subtype:
        //0x00 = error, receiver did not lock
        //  msg not used
        //0x01 = transmitter response.
        //  msg 0x00 = ACK, transmit OK
        //  msg 0x01 = ACK, but transmit started after 3 seconds delay anyway with RF receive data
        //  msg 0x02 = NAK, transmitter did not lock on the requested transmit frequency
        //  msg 0x03 = NAK, AC address zero in id1-id4 not allowed
