using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class RFXtrxModeCommand
    {
        public RFXtrxModeCommand()
        {
        }

        public override string ToString()
        {
            return _PacketLength + _PacketType + _SubType + _SeqNbr + _Command + _Msg1 + _Msg2 + _Msg3 + _Msg4 + _Msg5 + _Msg6 + _Msg7 + _Msg8 + _Msg9;
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
        protected string SubType
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
        public string Msg1
        {
            get
            {
                return _Msg1;
            }
            set
            {
                _Msg1 = value;
            }
        }
        public string Msg3
        {
            get
            {
                return _Msg3;
            }
            set
            {
                _Msg3 = value;
            }
        }
        public string Msg4
        {
            get
            {
                return _Msg4;
            }
            set
            {
                _Msg4 = value;
            }
        }
        public string Msg5
        {
            get
            {
                return _Msg5;
            }
            set
            {
                _Msg5 = value;
            }
        }

        protected string _PacketLength = "0D";
        protected string _PacketType = "00";
        protected string _SubType = "00";
        protected string _SeqNbr = "00";
        protected string _Command = "02"; // Get Mode as Default
        protected string _Msg1 = "53";
        protected string _Msg2 = "00"; // Not Used
        protected string _Msg3 = "00";
        protected string _Msg4 = "00";
        protected string _Msg5 = "00";
        protected string _Msg6 = "00"; // Not Used
        protected string _Msg7 = "00"; // Not Used
        protected string _Msg8 = "00"; // Not Used
        protected string _Msg9 = "00"; // Not Used

        //msg3:
        //
        //  7   Enable display of undecoded
        public void EnableUndecoded(bool yes)
        {
            if (yes)
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) | int.Parse("80", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) & int.Parse("7F", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  6   RFU6
        public void EnableRFU6(bool yes)
        {
            if (yes)
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) | int.Parse("40", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) & int.Parse("BF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  5   RFU5
        public void EnableRFU5(bool yes)
        {
            if (yes)
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) | int.Parse("20", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) & int.Parse("DF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  4   RFU4
        public void EnableRFU4(bool yes)
        {
            if (yes)
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) | int.Parse("10", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) & int.Parse("EF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  3   RFU3
        public void EnableRFU3(bool yes)
        {
            if (yes)
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) | int.Parse("08", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) & int.Parse("F7", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  2   FineOffset/Viking   433.92
        public void EnableFineOffsetViking(bool yes)
        {
            if (yes)
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) | int.Parse("04", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) & int.Parse("FB", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  1   Rubicson            433.92
        public void EnableRubicson(bool yes)
        {
            if (yes)
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) | int.Parse("02", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) & int.Parse("FD", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  0   AE                  433.92
        public void EnableAE(bool yes)
        {
            if (yes)
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) | int.Parse("01", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg3 = (int.Parse(_Msg3, NumberStyles.HexNumber) & int.Parse("FE", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }

        //msg4:
        //
        //  7   BlindsT1/T2/T3      433.92
        public void EnableBlindsT1T2T3(bool yes)
        {
            if (yes)
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) | int.Parse("80", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) & int.Parse("7F", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  6   BlindsT0            433.92
        public void EnableBlindsT0(bool yes)
        {
            if (yes)
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) | int.Parse("40", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) & int.Parse("BF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        ////  5   ProGuard            868.35 FSK
        //public void EnableProGuard(bool yes)
        //{
        //    if (yes)
        //        _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) | int.Parse("20", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        //    else
        //        _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) & int.Parse("DF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        //}
        ////  4   FS20                868.35
        //public void EnableFS20(bool yes)
        //{
        //    if (yes)
        //        _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) | int.Parse("10", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        //    else
        //        _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) & int.Parse("EF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        //}
        //  3   La Crosse           433.92 / 868.30
        public void EnableLaCrosse(bool yes)
        {
            if (yes)
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) | int.Parse("08", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) & int.Parse("F7", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  2   Hideki/UPM          433.92
        public void EnableHidekiUPM(bool yes)
        {
            if (yes)
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) | int.Parse("04", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) & int.Parse("FB", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  1   AD                  433.92
        public void EnableAD(bool yes) 
        {
            if (yes)
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) | int.Parse("02", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) & int.Parse("FD", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  0   Mertik              433.92
        public void EnableMertik(bool yes)
        {
            if (yes)
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) | int.Parse("01", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg4 = (int.Parse(_Msg4, NumberStyles.HexNumber) & int.Parse("FE", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }

        //msg5:
        //
        ////  7   Visonic             315 / 868.95
        //public void EnableVisonic(bool yes)
        //{
        //    if (yes)
        //        _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) | int.Parse("80", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        //    else
        //        _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) & int.Parse("7F", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        //}
        //  6   ATI                 433.92
        public void EnableATI(bool yes)
        {
            if (yes)
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) | int.Parse("40", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) & int.Parse("BF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  5   Oregon Scientific   433.92
        public void EnableOregonScientific(bool yes)
        {
            if (yes)
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) | int.Parse("20", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) & int.Parse("DF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  4   Meiantech           433.92
        public void EnableMeiantech(bool yes)
        {
            if (yes)
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) | int.Parse("10", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) & int.Parse("EF", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  3   HomeEasy EU         433.92
        public void EnableHomeEasy(bool yes)
        {
            if (yes)
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) | int.Parse("08", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) & int.Parse("F7", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  2   AC                  433.92
        public void EnableAC(bool yes)
        {
            if (yes)
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) | int.Parse("04", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) & int.Parse("FB", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  1   ARC                 433.92
        public void EnableARC(bool yes)
        {
            if (yes)
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) | int.Parse("02", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) & int.Parse("FD", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
        //  0   X10                 310 / 433.92
        public void EnableX10(bool yes)
        {
            if (yes)
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) | int.Parse("01", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
            else
                _Msg5 = (int.Parse(_Msg5, NumberStyles.HexNumber) & int.Parse("FE", NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
        }
    }
}

        //struct {
        //        BYTE packetlength; = 0x0D Packet length (this byte not included)
        //        BYTE packettype; = 0x00 = interface command
        //        BYTE subtype; = 0x00 = mode command
        //        BYTE seqnbr; = Sequence number. If not used in the application leave it zero.
        //        BYTE cmnd; = 
        //        BYTE msg1; // Select receiver/transceiver frequency
        //        BYTE msg2; // RFU
        //        BYTE msg3; // mode select bits
        //        BYTE msg4; // mode select bits
        //        BYTE msg5; // mode select bits
        //        BYTE msg6; // RFU
        //        BYTE msg7; // RFU
        //        BYTE msg8; // RFU
        //        BYTE msg9; // RFU
        //} ICMND;

        //cmnd:
        //  0x00 = Reset the receiver/transceiver. No answer is transmitted!
        //  0x01 = not used.
        //  0x02 = Get Status, return firmware versions and configuration of the interface.
        //  0x03 = Set Mode msg1-msg5, return firmware versions and configuration of the interface.
        //  0x04 = enable all receiving modes of the receiver/transceiver
        //  0x05 = enable reporting of undecoded packets
        //  0x06 = save receiving modes of the receiver/transceiver in non-volatile memory
        //  0x07 = not used
        //  0x08 = T1 – for internal use by RFXCOM
        //  0x09 = T2 – for internal use by RFXCOM
        //  0x10 = disable receiving of X10
        //  0x11 = disable receiving of ARC
        //  0x12 = disable receiving of AC
        //  0x13 = disable receiving of HomeEasy EU
        //  0x14 = disable receiving of Meiantech
        //  0x15 = disable receiving of Oregon Scientific
        //  0x16 = disable receiving of ATI Remote Wonder
        //  0x17 = disable receiving of Visonic
        //  0x18 = disable receiving of Mertik
        //  0x19 = disable receiving of AD
        //  0x1A = disable receiving of Hideki
        //  0x1B = disable receiving of La Crosse
        //  0x1C = disable receiving of FS20
        //  0x50 = select 310MHz in the 310/315 transceiver
        //  0x51 = select 315MHz in the 310/315 transceiver
        //  0x52 = not used
        //  0x53 = not used
        //  0x54 = not used
        //  0x55 = select 868.00MHz in the 868 transceiver
        //  0x56 = select 868.00MHz FSK in the 868 transceiver
        //  0x57 = select 868.30MHz in the 868 transceiver
        //  0x58 = select 868.30MHz FSK in the 868 transceiver
        //  0x59 = select 868.35MHz in the 868 transceiver
        //  0x5A = select 868.35MHz FSK in the 868 transceiver
        //  0x5B = select 868.95MHz in the 868 transceiver
        //msg1 – msg5:
        //  These bytes are only used by a Set Mode command.
        //msg1:
        //Select receiver/transceiver frequency.
        //  The 4 RFXtrx types use different hardware.
        //  It is for example not possible to select 315MHz in an RFXtrx433.
        //RFXtrx315:
        //  0x50 = 310MHz
        //  0x51 = 315MHz
        //RFXrec433:
        //  0x52 = 433.92MHz
        //RFXtrx433:
        //  0x53 = 433.92MHz
        //RFXtrx868:
        //  0x55 = 868.00MHz
        //  0x56 = 868.00MHz FSK
        //  0x57 = 868.30MHz
        //  0x58 = 868.30MHz FSK
        //  0x59 = 868.35MHz
        //  0x5A = 868.35MHz FSK
        //  0x5B = 868.95MHz
        //msg2:
        //  not used
        //msg3-msg5:
        //  Select operation modes
        //msg6-msg9:
        //  not used

        //msg3:
        //  Set a bit to 1 to enable the protocol in the RFXtrx
        //  bit protocol                    
        //  7   Enable display of undecoded
        //  6   RFU6
        //  5   RFU5
        //  4   RFU4
        //  3   RFU3
        //  2   FineOffset/Viking   433.92
        //  1   Rubicson            433.92
        //  0   AE                  433.92
        //
        //msg4:
        //  Set a bit to 1 to enable the protocol in the RFXtrx
        //  bit protocol        
        //  7   BlindsT1/T2/T3      433.92
        //  6   BlindsT0            433.92
        //  5   ProGuard            868.35 FSK
        //  4   FS20                868.35
        //  3   La Crosse           433.92 / 868.30
        //  2   Hideki/UPM          433.92
        //  1   AD                  433.92
        //  0   Mertik              433.92
        //
        //msg5:
        //  Set a bit to 1 to enable the protocol in the RFXtrx
        //  bit protocol            
        //  7   Visonic             315 / 868.95
        //  6   ATI                 433.92
        //  5   Oregon Scientific   433.92
        //  4   Meiantech           433.92
        //  3   HomeEasy EU         433.92
        //  2   AC                  433.92
        //  1   ARC                 433.92
        //  0   X10                 310 / 433.92
