using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class RFXtrxIResponse
    {
        public RFXtrxIResponse()
        {
        }

        public override string ToString()
        {
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _Command + _ReceiverTransceiverType + _FirmwareVersion + _EnabledProtocols + _NotUsed;
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
        public string ReceiverTransceiverType
        {
            get
            {
                return _ReceiverTransceiverType;
            }
            set
            {
                _ReceiverTransceiverType = value;
            }
        }
        public string FirmwareVersion
        {
            get
            {
                return _FirmwareVersion;
            }
            set
            {
                _FirmwareVersion = value;
            }
        }
        public string EnabledProtocols
        {
            get
            {
                return _EnabledProtocols;
            }
            set
            {
                _EnabledProtocols = value;
            }
        }
        public string NotUsed
        {
            get
            {
                return _NotUsed;
            }
            set
            {
                _NotUsed = value;
            }
        }

        protected string _PacketLength = "0D";
        protected string _PacketType = "01";
        protected string _SubType = "00";
        protected string _SeqNbr = "00";
        protected string _Command = "00";
        protected string _ReceiverTransceiverType = "53";
        protected string _FirmwareVersion = "00";
        protected string _EnabledProtocols = "000000";
        protected string _NotUsed = "00000000";
    }
}

        //9.3. 0x01: Interface Message
        //9.3.1. 0x00: Response on a mode command

        //Responses or status messages send by the interface.
        //struct {
        //        BYTE packetlength; Packet length (this byte not included) = 0x0D
        //        BYTE packettype; 0x01 = interface control
        //        BYTE subtype; 0x00 = response on a mode command
        //        BYTE seqnbr; Sequence number, can be used in the application to synchronize the commands sent with response messages
        //        BYTE cmnd; This field contains the command type to which this is the response. See Interface Control – Mode command for the list of command codes.
        //        BYTE msg1; receiver/transceiver type
        //        BYTE msg2; Firmware version of the transceiver / receiver firmware
        //        BYTE msg3; Enabled protocols
        //        BYTE msg4; Enabled protocols
        //        BYTE msg5; Enabled protocols
        //        BYTE msg6; Not used
        //        BYTE msg7; Not used
        //        BYTE msg8; Not used
        //        BYTE msg9; Not used
        //} IRESPONSE;

        //msg1:
        //receiver/transceiver type
        //  0x50 = 310MHz
        //  0x51 = 315MHz
        //  0x52 = 433.92MHz receiver only
        //  0x53 = 433.92MHz transceiver
        //  0x55 = 868.00MHz
        //  0x56 = 868.00MHz FSK
        //  0x57 = 868.30MHz
        //  0x58 = 868.30MHz FSK
        //  0x59 = 868.35MHz
        //  0x5A = 868.35MHz FSK
        //  0x5B = 868.95MHz
