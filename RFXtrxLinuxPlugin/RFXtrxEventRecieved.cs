using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
//using ExpressionParser;
using SwitchKing.Server.Plugins.RFXtrx;
using SwitchKing.Server.Plugins.Interfaces;
using SwitchKing.Server.Plugins.RFXtrx.Diagnostics;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public partial class Plugin //:
        //ICommandInterceptor,
        //IMonitoredEventInterceptor,
        //IDisposable
    {
        private string HexToString(string hex)
        {
            return Convert.ToChar(int.Parse(hex, NumberStyles.HexNumber)).ToString();
            //return hex
            //    .Replace("41", "A").Replace("42", "B").Replace("43", "C").Replace("44", "D")
            //    .Replace("45", "E").Replace("46", "F").Replace("47", "G").Replace("48", "H")
            //    .Replace("49", "I").Replace("4A", "J").Replace("4B", "K").Replace("4C", "L")
            //    .Replace("4D", "M").Replace("4E", "N").Replace("4F", "O").Replace("50", "P");
        }

        private string StringToHex(string str)
        {
            return str
                .Replace("A", "41").Replace("B", "42").Replace("C", "43").Replace("D", "44")
                .Replace("E", "45").Replace("F", "46").Replace("G", "47").Replace("H", "48")
                .Replace("I", "49").Replace("J", "4A").Replace("K", "4B").Replace("L", "4C")
                .Replace("M", "4D").Replace("N", "4E").Replace("O", "4F").Replace("P", "50");
            //var v1 = ;
            //var v2 = int.Parse(v1, NumberStyles.Integer);
            //lighting1.UnitCode = c1.ToString("X").PadLeft(2, '0');
            //var v3 = Convert.ToChar(int.Parse(hex, NumberStyles.HexNumber)).ToString();
            //return Convert.ToChar(int.Parse(hex, NumberStyles.HexNumber)).ToString();
            //return v2.ToString("X").PadLeft(2, '0');
        }

        public void PidIn(string pid, string command)
        {
            if (!Plugin.ReceivedList.ContainsKey(pid))
            {
                Dictionary<string, object> received = new Dictionary<string, object>();
                received.Add("command", command);
                received.Add("timestamp", DateTime.MinValue);
                Plugin.ReceivedList.Add(pid, received);
            }
        }

        private bool NotInTreshold(string pid, string command)
        {
            return ((!Plugin.ReceivedList[pid]["command"].ToString().Equals(command)) ||
                   (Convert.ToDateTime(Plugin.ReceivedList[pid]["timestamp"]).AddMilliseconds(Convert.ToDouble(Plugin.Threshold)) < DateTime.Now));
        }

        private bool InTreshold(string pid, string command)
        {
            return ((Plugin.ReceivedList[pid]["command"].ToString().Equals(command)) &&
                   (Convert.ToDateTime(Plugin.ReceivedList[pid]["timestamp"]).AddMilliseconds(Convert.ToDouble(Plugin.Threshold)) > DateTime.Now));
        }

        public void PidOut(string pid, string command)
        {
            Plugin.ReceivedList.Remove(pid);
            Dictionary<string, object> updated = new Dictionary<string, object>();
            updated.Add("command", command);
            updated.Add("timestamp", DateTime.Now);
            Plugin.ReceivedList.Add(pid, updated);
        }

        private string AppendToHex(string[] buf, string[] msg)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder();//(buf.Length + msg.Length) * 3);
            //loop through each byte in the array
            foreach (string item in buf)
                //convert the byte to a string and add to the stringbuilder
                builder.Append(item.PadRight(3, ' '));
            //return the converted value
            foreach (string item in msg)
                //convert the byte to a string and add to the stringbuilder
                builder.Append(item.PadRight(3, ' '));
            builder.Remove(builder.Length -2, 1);
            return builder.ToString().ToUpper();
        }

        public void EventRecieved(string message)
        {
            Plugin.TraceIt(message + ": " + "RFXtrxEventReceived", true);
            
            string[] msg = message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (msg[1])
            {
                case "01":  // IResponse ***
                    #region
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
                    //        BYTE msg3;
                    //        BYTE msg4;
                    //        BYTE msg5;
                    //        BYTE msg6;
                    //        BYTE msg7;
                    //        BYTE msg8;
                    //        BYTE msg9;
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
                    Plugin.TraceIt(message + ": " + "Interface Message", true);
                    #endregion
                    break;
                case "02":  // RXResponse ***
                    #region
                    //9.4. 0x02: Receiver/Transmitter Message
                    //9.4.1. 0x00 – 0x01: messages

                    //Responses or messages send by the receiver or transmitter.
                    //struct {
                    //        BYTE packetlength; Packet length (this byte not included) = 0x04
                    //        BYTE packettype; 0x02 = receiver/transmitter message
                    //        BYTE subtype;
                    //        BYTE seqnbr; Sequence number, can be used in the application to synchronize the commands sent with response messages. If not used leave it zero.
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
                    {
                        RFXtrxRXResponse RXResponse = new RFXtrxRXResponse();
                        //RXResponse.PacketLength = "0";
                        //RXResponse.PacketType = "02";
                        RXResponse.SubType = msg[2];
                        RXResponse.SeqNbr = msg[3];
                        RXResponse.Message = msg[4];

                        switch (RXResponse.SubType)
                        {
                            case "00":
                                #region
                                // 0x00 = error, receiver did not lock
                                Plugin.TraceIt(message + ": " + "error, receiver did not lock", true);
                                #endregion
                                break;
                            case "01":
                                #region
                                switch (RXResponse.Message)
                                // msg 0x00 = ACK, transmit OK
                                // msg 0x01 = ACK, but transmit started after 3 seconds delay anyway with RF receive data
                                // msg 0x02 = NAK, transmitter did not lock on the requested transmit frequency
                                // msg 0x03 = NAK, AC address zero in id1-id4 not allowed
                                {
                                    case "00":
                                    case "01":
                                        #region
                                        // ACK
                                        Plugin.TraceIt(message + ": " + "ACK", true);
                                        #endregion
                                        break;
                                    case "02":
                                    case "03":
                                        #region
                                        // NAK
                                        Plugin.TraceIt(message + ": " + "NAK", true);
                                        #endregion
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                    break;
                case "03":  // Undecoded RF Message
                    #region
                    //9.5. 0x03: Undecoded RF Message
                    //9.5.1. 0x00 – 0x12: message type
                    Plugin.TraceIt(message + ": " + "Undecoded RF Message", true);
                    #endregion
                    break;
                case "10":  // Lighting1 *** X10, ARC, ELRO, Waveman, EMW200, IMPULS, RisingSun, Philips
                    #region
                    //9.6. 0x10, Lighting1
                    //9.6.1. 0x00-0x07: X10, ARC, ELRO, Waveman, EMW200, IMPULS, RisingSun, Philips

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
                    Plugin.TraceIt(message + ": " + "lighting1", true);
                    {
                        RFXtrxLighting1 lighting1 = new RFXtrxLighting1();
                        //lighting1.PacketLength = "07"
                        //lighting1.PacketType = "10";
                        lighting1.SubType = msg[2];
                        lighting1.SeqNbr = msg[3];
                        lighting1.HouseCode = msg[4];
                        lighting1.UnitCode = msg[5];
                        lighting1.Command = msg[6];
                        //lighting1.Filler = "0";
                        lighting1.RSSI = msg[7].Substring(0, 2);

                        string housecode = HexToString(lighting1.HouseCode);
                        int unitcode = int.Parse(lighting1.UnitCode, NumberStyles.HexNumber);
                        string code = housecode + ":" + unitcode.ToString();
                        string command = lighting1.Command == "00" ? "TurnOff" : "TurnOn";

                        {
                            string pid;
                            if (Plugin.ServerDB.GetDeviceIDFromDeviceCode(code, out pid))
                            {
                                if (Plugin.WhichOne.ContainsKey(pid) ? Plugin.WhichOne[pid]["RFXtrx"] : Plugin.Transcievers["RFXtrx"]["Default"].Equals(true))
                                {
                                    PidIn(pid, command);

                                    if (!InTreshold(pid, command))
                                    {
                                        string url = Plugin.CreateUrl(pid, command);
                                        Plugin.SendRestCommand(url);
                                    }

                                    PidOut(pid, command);
                                }

                                if (Plugin.ServerDB.GetDataSourceIDFromDeviceCode(code, out pid))
                                {
                                    if (Plugin.WhichOne.ContainsKey(pid) ? Plugin.WhichOne[pid]["RFXtrx"] : Plugin.Transcievers["RFXtrx"]["Default"].Equals(true))
                                    {
                                        PidIn(pid, command);

                                        if (!InTreshold(pid, command))
                                        {
                                            string url = Plugin.CreateUrl(pid, command);
                                            Plugin.SendRestCommand(url);
                                        }

                                        PidOut(pid, command);
                                    }
                                }
                            }
                        }

                    }
                    #endregion
                    break;
                case "11":  // Lighting2 ***
                    #region
                    //9.7. 0x11, Lighting2
                    //9.7.1. 0x00-0x02: AC, HomeEasy EU, ANSLUT

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

                    switch (msg[2])
                    {
                        case "00":  // AC
                        case "01":  // HomeEasy EU
                        case "02":  // ANSLUT
                            #region
                            Plugin.TraceIt(message + ": " + "lighting2", true);
                            {
                                RFXtrxLighting2 lighting2 = new RFXtrxLighting2();
                                //lighting2.PacketLength = "0B";
                                //lighting2.PacketType = "11";
                                lighting2.SubType = msg[2];
                                lighting2.SeqNbr = msg[3];
                                lighting2.ID = msg[4] + msg[5] + msg[6] + msg[7];
                                lighting2.Unit = msg[8];
                                lighting2.Command = msg[9];
                                lighting2.SignalLevel = msg[10];
                                //lighting2.Filler = "0";
                                lighting2.RSSI = msg[11].Substring(0, 2);

                                int ID = int.Parse(lighting2.ID, NumberStyles.HexNumber);
                                int unit = int.Parse(lighting2.Unit, NumberStyles.HexNumber);
                                string code = ID.ToString() + ":" + unit.ToString();
                                string command = lighting2.Command == "00" ? "TurnOff" : "TurnOn";
                                string level = (int.Parse(lighting2.SignalLevel, NumberStyles.HexNumber) * 100 / 15).ToString();

                                {
                                    string pid;
                                    if (Plugin.ServerDB.GetDeviceIDFromDeviceCode(code, out pid))
                                    {
                                        if (Plugin.WhichOne.ContainsKey(pid) ? Plugin.WhichOne[pid]["RFXtrx"] : Plugin.Transcievers["RFXtrx"]["Default"].Equals(true))
                                        {
                                            PidIn(pid, command);

                                            if (!InTreshold(pid, command))
                                            {
                                                string url;
                                                //if (lighting2.SignalLevel.Equals("0F"))
                                                //{
                                                //    url = Plugin.CreateUrl(pid, command);
                                                //}
                                                //else
                                                {
                                                    url = Plugin.CreateUrl(pid, level);
                                                }
                                                Plugin.SendRestCommand(url);
                                            }

                                            PidOut(pid, command);
                                        }
                                    }

                                    if (Plugin.ServerDB.GetDataSourceIDFromDeviceCode(code, out pid))
                                    {
                                        if (Plugin.WhichOne.ContainsKey(pid) ? Plugin.WhichOne[pid]["RFXtrx"] : Plugin.Transcievers["RFXtrx"]["Default"].Equals(true))
                                        {
                                            PidIn(pid, command);

                                            if (!InTreshold(pid, command))
                                            {
                                                string url = Plugin.CreateUrl(pid, command);
                                                Plugin.SendRestCommand(url);
                                            }

                                            PidOut(pid, command);
                                        }
                                    }
                                }
                            }
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;
                case "12":  // Lighting3
                    #region
                    //9.8. 0x12, Lighting3
                    //9.8.1. 0x00-: Koppla
                    Plugin.TraceIt(message + ": " + "Lighting3", true);
                    #endregion
                    break;
                case "13":  // Lighting4
                    #region
                    //9.9. 0x13, Lighting4
                    //9.9.1. 0x00: PT2262
                    Plugin.TraceIt(message + ": " + "Lighting4", true);
                    #endregion
                    break;
                case "14":  // Lighting5
                    #region
                    //9.10. 0x14, Lighting5
                    //9.10.1. 0x00-0x01: LightwaveRF, Siemens, EMW100, BBSB
                    Plugin.TraceIt(message + ": " + "Lighting5", true);
                    #endregion
                    break;
                case "15":  // Lighting6
                    #region
                    //9.11. 0x15, Lighting6
                    //9.11.1. 0x00: Blyss

                    //struct {
                    //        BYTE packetlength; = 0x0B (this byte not included)
                    //        BYTE packettype;  = 0x15 = lighting6
                    //        BYTE subtype;     = 0x00 = Blyss
                    //        BYTE seqnbr;      This field contains a sequence number from 0x00 to 0xFF.
                    //        BYTE id1;         id1 – id2: remote/switch/unit ID. (id1 is high byte)
                    //        BYTE id2;
                    //        BYTE groupcode;   = 0x41 to 0x44 = group A, B, C or D
                    //        BYTE unitcode;    = 0x01 to 0x05 = unit 1 to 5
                    //        BYTE cmnd;        = 0x00 = On, 0x01 = Off, 0x02 = group On, 0x03 = group Off
                    //        BYTE cmndseqnbr;  = 0x00 to 0x04 (to be incremented after each command)
                    //        BYTE rfu;         reserved for future use.
                    //        BYTE filler : 4;
                    //        BYTE rssi : 4;    = 0x0 to 0xF = weak to strong (is 0x0 for transmitter command)
                    //} LIGHTING6;
                    Plugin.TraceIt(message + ": " + "Lighting6", true);
                    #endregion
                    break;
                case "18":  // Curtain1
                    #region
                    //9.12. 0x18, Curtain1
                    //9.12.1. 0x00: Harrison
                    Plugin.TraceIt(message + ": " + "Curtain1", true);
                    #endregion
                    break;
                case "19":  // Blinds1
                    #region
                    //9.13. 0x19, Blinds1
                    //9.13.1. 0x00-0x03: RollerTrol, Hasta, A-OK
                    Plugin.TraceIt(message + ": " + "Blinds1", true);
                    #endregion
                    break;
                case "20":  // Security1
                    #region
                    //9.14. 0x20, Security1
                    //9.14.1. 0x00-0x08: X10, KD101, Visonic, Meiantech
                    Plugin.TraceIt(message + ": " + "Security1", true);
                    #endregion
                    break;
                case "28":  // Camera1
                    #region
                    //9.15. 0x28, Camera1
                    //9.15.1. 0x00: X10 Ninja/Robocam
                    Plugin.TraceIt(message + ": " + "Camera1", true);
                    #endregion
                    break;
                case "30":  // Remote control and IR
                    #region
                    //9.16. 0x30, Remote control and IR
                    //9.16.1. 0x00-0x04: ATI, Medion, PC Remote
                    Plugin.TraceIt(message + ": " + "Remote control and IR", true);
                    #endregion
                    break;
                case "40":  // Thermostat1
                    #region
                    //9.17. 0x40, Thermostat1
                    //9.17.1. 0x00-0x01: Digimax
                    Plugin.TraceIt(message + ": " + "Thermostat1", true);
                    #endregion
                    break;
                case "41":  // Thermostat2
                    #region
                    //9.18. 0x41, Thermostat2
                    //9.18.1. 0x00-0x01: HomeEasy HE105, RTS10
                    Plugin.TraceIt(message + ": " + "Thermostat2", true);
                    #endregion
                    break;
                case "42":  // Thermostat3
                    #region
                    //9.19. 0x42, Thermostat3
                    //9.19.1. 0x00-0x01: Mertik-Maxitrol G6R-H4T1 / G6R-H4TB
                    Plugin.TraceIt(message + ": " + "Thermostat3", true);
                    #endregion
                    break;
                case "50":  // Temperature sensors
                    #region
                    //9.20. 0x50, Temperature sensors
                    //9.20.1. 0x01-0x0A: TEMP1-TEMP10

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

                    TemperatureSensor TempSensor = new TemperatureSensor();
                    //PacketLength = "08";
                    //PacketType = "50";
                    TempSensor.SubType = msg[2]; //"00";
                    TempSensor.SeqNbr = msg[2]; //"00";
                    TempSensor.ID = msg[4] + msg[5]; //"0000";
                    TempSensor.Temperature = msg[6] + msg[7]; //"0000";
                    TempSensor.BatteryLevel = msg[8].Substring(0, 1); //"0";
                    TempSensor.RSSI = msg[8].Substring(1, 1); //"0";

                    #region
                    Plugin.TraceIt(message + ": " + "Temperature sensors", true);
                    {
                        string sensorid = int.Parse(TempSensor.ID, NumberStyles.HexNumber).ToString();
                        int m = int.Parse(TempSensor.Temperature, NumberStyles.HexNumber);
                        string sign = (((m & 0x8000) >> 15) == 1) ? "-" : "+";
                        //string high = Decimal.Round((Convert.ToDecimal(m & 0xF700) / 2), 2, MidpointRounding.ToEven).ToString();
                        //string low = Decimal.Round((Convert.ToDecimal(m & 0x00FF)), 2, MidpointRounding.ToEven).ToString();
                        //int h1 = m & 0x7F00;
                        //int h2 = h1 >> 1;
                        //int high = h1;
                        //int high = (m & 0x7F00);
                        //int high = int.Parse(((m & 0xF700) / 2).ToString());
                        //int low = int.Parse(((m & 0x00FF)).ToString());
                        //string value = Decimal.Round((Convert.ToDecimal(high + low) / 10), 1, MidpointRounding.ToEven).ToString();
                        string value = Decimal.Round((Convert.ToDecimal((m & 0x7FFF)) / 10), 1, MidpointRounding.ToEven).ToString();
                        foreach (var item in Plugin.SensorMappings)
                        {
                            if (item.Value["Type"].Equals("TempSensor") && (item.Value["SensorID"].Equals(sensorid) || item.Value["SensorID"].Equals("*")))
                            {
                                string pid = item.Key;
                                var url = Plugin.CreateUrl(pid, sign + value);
                                Plugin.SendRestCommand(@url);
                            }
                        }
                    }
                    #endregion

                    switch (TempSensor.SubType)
                    {
                        case "01": //0x01 = TEMP1 is THR128/138, THC138
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP1 is THR128/138, THC138", true);
                            #endregion
                            break;
                        case "02": //0x02 = TEMP2 is THC238/268,THN132,THWR288,THRN122,THN122,AW129/131
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP2 is THC238/268,THN132,THWR288,THRN122,THN122,AW129/131", true);
                            #endregion
                            break;
                        case "03": //0x03 = TEMP3 is THWR800
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP3 is THWR800", true);
                            #endregion
                            break;
                        case "04": //0x04 = TEMP4 is RTHN318
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP4 is RTHN318", true);
                            #endregion
                            break;
                        case "05": //0x05 = TEMP5 is La Crosse TX2, TX3, TX4, TX17
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP5 is La Crosse TX2, TX3, TX4, TX17", true);
                            #endregion
                            break;
                        case "06": //0x06 = TEMP6 is TS15C
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP6 is TS15C", true);
                            #endregion
                            break;
                        case "07": //0x07 = TEMP7 is Viking 02811
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP7 is Viking 02811", true);
                            #endregion
                            break;
                        case "08": //0x08 = TEMP8 is La Crosse WS2300
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP8 is La Crosse WS2300", true);
                            #endregion
                            break;
                        case "09": //0x09 = TEMP9 is RUBiCSON
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP9 is RUBiCSON", true);
                            #endregion
                            break;
                        case "0A": //0x0A = TEMP10 is TFA 30.3133
                            #region
                            Plugin.TraceIt(message + ": " + "TEMP10 is TFA 30.3133", true);
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;
                case "51":  // Humidity sensors
                    #region
                    //9.21. 0x51: Humidity sensors
                    //9.21.1. 0x01: TX3
                    Plugin.TraceIt(message + ": " + "Humidity sensors", true);
                    #endregion
                    break;
                case "52":  // Temperature and humidity sensors
                    #region
                    //9.22. 0x52: Temperature and humidity sensors
                    //9.22.1. 0x01-0x09: TH1-TH9
                    Plugin.TraceIt(message + ": " + "Temperature and humidity sensors", true);
                    #endregion
                    break;
                case "53":  // Barometric sensors
                    #region
                    //9.23. 0x53: Barometric sensors
                    //9.23.1. 0x01: reserved for future use
                    Plugin.TraceIt(message + ": " + "Barometric sensors", true);
                    #endregion
                    break;
                case "54":  // Temperature, humidity and barometric sensors
                    #region
                    //9.24. 0x54: Temperature, humidity and barometric sensors
                    //9.24.1. 0x01-0x02: THB1-THB2
                    Plugin.TraceIt(message + ": " + "Temperature, humidity and barometric sensors", true);
                    #endregion
                    break;
                case "55":  // Rain sensors
                    #region
                    //9.25. 0x55: Rain sensors
                    //9.25.1. 0x01-0x05: RAIN1-RAIN5
                    Plugin.TraceIt(message + ": " + "Rain sensors", true);
                    #endregion
                    break;
                case "56":  // Wind sensors
                    #region
                    //9.26. 0x56: Wind sensors
                    //9.26.1. 0x01-0x06: WIND1-WIND6
                    Plugin.TraceIt(message + ": " + "Wind sensors", true);
                    #endregion
                    break;
                case "57":  // UV sensors
                    #region
                    //9.27. 0x57: UV sensors
                    //9.27.1. 0x01-0x02: UV1-UV2
                    Plugin.TraceIt(message + ": " + "UV sensors", true);
                    #endregion
                    break;
                case "58":  // Date/time sensors
                    #region
                    //9.28. 0x58: Date/time sensors
                    //9.28.1. 0x01: DT1
                    Plugin.TraceIt(message + ": " + "Date/time sensors", true);
                    #endregion
                    break;
                case "59":  // Current sensors
                    #region
                    //9.29. 0x59: Current sensors
                    //9.29.1. 0x01: ELEC1
                    Plugin.TraceIt(message + ": " + "Current sensors", true);
                    #endregion
                    break;
                case "5A":  // Energy usage sensors
                    #region
                    //9.30. 0x5A: Energy usage sensors
                    //9.30.1. 0x01-0x02: ELEC2-ELEC3
                    Plugin.TraceIt(message + ": " + "Energy usage sensors", true);
                    #endregion
                    break;
                case "5B":  // Gas usage sensors
                    #region
                    //9.31. 0x5B: Gas usage sensors
                    //9.31.1. 0x01: reserved
                    Plugin.TraceIt(message + ": " + "Gas usage sensors", true);
                    #endregion
                    break;
                case "5C":  // Water usage sensors
                    #region
                    //9.32. 0x5C: Water usage sensors
                    //9.32.1. 0x01: reserved
                    Plugin.TraceIt(message + ": " + "Water usage sensors", true);
                    #endregion
                    break;
                case "5D":  // Weighting scale
                    #region
                    //9.33. 0x5D: Weighting scale
                    //9.33.1. 0x01-0x02: WEIGHT1-WEIGHT2
                    Plugin.TraceIt(message + ": " + "Weighting scale", true);
                    #endregion
                    break;
                case "70":  // RFXsensor ***
                    #region
                    //9.34. 0x70: RFXsensor
                    //9.34.1. 0x00-0x03: Temp, Voltage, A/D, message

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
                    //0x0084 = no end of read signal Received from 1-Wire device
                    //0x0085 = 1-Wire scratchpad CRC error

                    RFXtrxRFXsensor RFXsensor = new RFXtrxRFXsensor();
                    //RFXsensor.PacketLength = msg[0];
                    //RFXsensor.PacketType = msg[1];
                    RFXsensor.SubType = msg[2];
                    RFXsensor.SeqNbr = msg[3];
                    RFXsensor.ID = msg[4];
                    RFXsensor.Message = msg[5] + msg[6];
                    //RFXsensor.Filler = msg[7].Substring(0, 1); // "0"; 
                    RFXsensor.RSSI = msg[7].Substring(1, 1);// = "0";
                    switch (RFXsensor.SubType)
                    {
                        case "00": //0x00 = RFXSensor temperature
                            #region
                            //msg1-msg2 for subtype 00, 01 or 10
                            //measured value in Celsius * 100.
                            //Bit 7 in msg1 is a sign bit. For a negative temperature this bit is 1.
                            Plugin.TraceIt(message + ": " + "RFXSensor temperature", true);
                            {
                                string sensorid = int.Parse(RFXsensor.ID, NumberStyles.HexNumber).ToString();
                                //int d = int.Parse(RFXsensor.ID, NumberStyles.HexNumber);
                                int m = int.Parse(RFXsensor.Message, NumberStyles.HexNumber);
                                //string s = ((m & 32768) == 1) ? "-" : "+";
                                string sign = ((m & 0x8000) == 1) ? "-" : "+";
                                //string t = Decimal.Round((Convert.ToDecimal(m & 32767) / 100), 2, MidpointRounding.ToEven).ToString();
                                string value = Decimal.Round((Convert.ToDecimal(m & 0x7FFF) / 100), 2, MidpointRounding.ToEven).ToString();
                                //string temp = sign + value;
                                foreach (var item in Plugin.SensorMappings)
                                {
                                    //string type = item.Value["Type"];
                                    //string sensorid = item.Value["SensorID"];
                                    //bool v1 = type.Equals("RFXsensor");

                                    if (item.Value["Type"].Equals("RFXsensor") && item.Value["SensorID"].Equals(sensorid))
                                    {
                                        string pid = item.Key;
                                        var url = Plugin.CreateUrl(pid, sign + value);
                                        Plugin.SendRestCommand(@url);
                                    }
                                }
                            }
                            #endregion
                            break;
                        case "01": //0x01 = RFXSensor A/D
                            #region
                            //msg1-msg2 for subtype 01 or 10
                            //measured value in mV
                            Plugin.TraceIt(message + ": " + "RFXSensor A/D", true);
                            #endregion
                            break;
                        case "02": //0x02 = RFXSensor voltage
                            #region
                            //msg1-msg2 for subtype 01 or 10
                            //measured value in mV
                            Plugin.TraceIt(message + ": " + "RFXSensor voltage", true);
                            #endregion
                            break;
                        case "03": //0x03 = RFXSensor message
                            #region
                            //msg1-msg2 for subtype 11
                            //0x0001 = sensor addresses incremented
                            //0x0002 = battery low detected
                            //0x0081 = no 1-wire device connected
                            //0x0082 = 1-Wire ROM CRC error
                            //0x0083 = 1-Wire device connected is not a DS18B20 or DS2438
                            //0x0084 = no end of read signal Received from 1-Wire device
                            //0x0085 = 1-Wire scratchpad CRC error
                            Plugin.TraceIt(message + ": " + "RFXSensor message", true);
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;
                case "71":  // RFXMeter
                    #region
                    //9.35. 0x71: RFXMeter
                    //9.35.1. 0x00-0x0F: counter, messages
                    Plugin.TraceIt(message + ": " + "RFXMeter", true);
                    #endregion
                    break;
                default:
                    #region
                    // Unknown message
                    Plugin.TraceIt(message + ": " + "Unknown message", true);
                    #endregion
                    break;
            }
        }

    }
}
