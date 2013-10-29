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
using SwitchKing.Server.Plugins.Interfaces;
using SwitchKing.Server.Plugins.RFXtrx.Diagnostics;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public partial class Plugin //:
        //ICommandInterceptor,
        //IMonitoredEventInterceptor,
        //IDisposable
    {
        private string IntToHex(int value)
        {
            return value.ToString("X");
        }

        private string IntToHex(int value, int length)
        {
            return value.ToString("X").PadLeft(length, '0');
        }

        private int HexToInt(string hex)
        {
            return int.Parse(hex, NumberStyles.HexNumber);
        }

        public void RFXtrxOnCommand(int deviceId, string deviceName, int deviceActionId, string deviceActionName, int? dimLevel, string eventSource, out bool intercepted)
        {
            string id = deviceId.ToString();
            string pid = id.WithPrefix(DevicePrefix);
            //TraceIt(pid + " OnCommand", WriteToFile);
            intercepted = false;

            if (WhichOne.ContainsKey(pid) ? WhichOne[pid]["RFXtrx"].ToString().ToLower().Equals("true") : Transcievers["RFXtrx"]["Default"].Equals(true))
            {
                string type;

                if (ServerDB.GetDeviceTypeIDFromDeviceID(pid, out type))
                {
                    switch (type)
                    {
                        case "1":       // Nexa, Code Switch, arctech
                            // X10, ARC, ELRO, Waveman, EMW200, IMPULS, RisingSun, Philips
                            // ARC is the protocol used by different brands with units having code wheels A-P/1-16:
                            // KlikAanKlikUit, NEXA, CHACON, HomeEasy, Proove, DomiaLite, InterTechno, AB600
                            #region 1

                            //struct {
                            //        BYTE packetlength; = 0x07
                            //        BYTE packettype; 0x10 = lighting1
                            //        BYTE subtype;
                            //        BYTE seqnbr; 0x00 to 0xFF.
                            //        BYTE housecode; 0x41 to 0x50 = house code A to P
                            //        BYTE unitcode; 0x01 to 0x10 = unit 1 to 16
                            //        BYTE cmnd;
                            //        BYTE filler : 4;
                            //        BYTE rssi : 4;
                            //} LIGHTING1;

                            try
                            {
                                RFXtrxLighting1 lighting1 = new RFXtrxLighting1();

                                ////lighting1.PacketLength = "07";
                                ////lighting1.PacketType = "10";
                                //lighting1.SubType = "01";
                                //lighting1.SeqNbr = "00";
                                //lighting1.HouseCode = "41";
                                //lighting1.UnitCode = "01";
                                //lighting1.Command = "00";
                                ////lighting1.Filler = "0";
                                //lighting1.RSSI = "0";

                                lighting1.Command = deviceActionName.Replace("Turn O", "TurnO").Replace("TurnOff", "00").Replace("TurnOn", "01");

                                TraceIt("RFXComm.WriteData(\"" + lighting1.ToString() + "\")", WriteToFile);

                                {
                                    string ret;
                                    if (ServerDB.GetDeviceCodeFromDeviceID(pid, out ret))
                                    {
                                        string[] c = ret.Split(new char[] { ':' });
                                        lighting1.HouseCode = c[0]
                                            .Replace("A", "41").Replace("B", "42").Replace("C", "43").Replace("D", "44")
                                            .Replace("E", "45").Replace("F", "46").Replace("G", "47").Replace("H", "48")
                                            .Replace("I", "49").Replace("J", "4A").Replace("K", "4B").Replace("L", "4C")
                                            .Replace("M", "4D").Replace("N", "4E").Replace("O", "4F").Replace("P", "50");
                                        int c1 = int.Parse(c[1], NumberStyles.Integer);
                                        lighting1.UnitCode = c1.ToString("X").PadLeft(2, '0');

                                        RFXComm.WriteData(lighting1.ToString());
                                    }
                                }
                                intercepted = false;
                                //intercepted = true;
                            }
                            catch { }
                            break;
                            #endregion

                        case "21":      // Proove, Self Learning On/Off, arctech
                        case "3":       // Nexa, Self Learning Dimmer, arctech
                        case "2":       // Nexa, Self Learning On/Off, arctech
                            // AC, HomeEasy EU, ANSLUT
                            // AC is the protocol used by different brands with units having a learning mode button:
                            // KlikAanKlikUit, NEXA, CHACON, HomeEasy UK
                            #region 2

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
                            //        BYTE rssi : 4;    = 0x0 to 0xF = weak to strong (is 0x0 for transmitter command), Signal strength.
                            //} LIGHTING2;

                            try
                            {

                                RFXtrxLighting2 lighting2 = new RFXtrxLighting2();
                                ////lighting2.PacketLength = "0B";
                                ////lighting2.PacketType = "11";
                                //lighting2.SubType = "00";
                                //lighting2.SeqNbr = "00";
                                //lighting2.ID = "0000006F";
                                //lighting2.Unit = "01";
                                //lighting2.Command = "00";
                                //lighting2.SignalLevel = "00";
                                ////lighting2.Filler = "0";
                                //lighting2.RSSI = "0";

                                {
                                string ret;
                                if (ServerDB.GetDeviceCodeFromDeviceID(pid, out ret))
                                {
                                    {
                                        string anslut;
                                        if (type == "21" && ServerDB.GetDeviceDescriptionFromDeviceID(pid, out anslut))
                                        {
                                            if (anslut.Contains("ANSLUT")) { lighting2.SubType = "02"; }
                                        }
                                    }

                                    string[] c = ret.Split(new char[] { ':' });
                                    int c0 = int.Parse(c[0], NumberStyles.Integer);
                                    int c1 = int.Parse(c[1], NumberStyles.Integer);
                                    lighting2.ID = c0.ToString("X").PadLeft(8, '0');
                                    lighting2.Unit = c1.ToString("X").PadLeft(2, '0');

                                    lighting2.Command = deviceActionName.Replace("Turn Off", "00").Replace("Turn On", "01");

                                    if (lighting2.Command == "00")  // (0 = dimLevel Client, or dimLevel = null REST)
                                    {
                                        lighting2.SignalLevel = "00";
                                    }
                                    else if (lighting2.Command == "01")  // (0 < dimLevel <= 100, or dimLevel(100) = null REST)
                                    {
                                        if (dimLevel != null && dimLevel < 100)  // (1 < dimLevel < 99 Client, or 1 < dimLevel < 99 REST)
                                        {
                                            int dim = (int)Decimal.Round((Convert.ToDecimal(dimLevel.Value * 15 / 100)), 0, MidpointRounding.ToEven);
                                            lighting2.SignalLevel = IntToHex(dim, 2);
                                            lighting2.Command = "02";
                                        }
                                        else  // (dimLevel = 100 Client, or dimLevel(100) = null REST)
                                        {
                                            lighting2.SignalLevel = "0F";
                                            if (type == "3")
                                            {
                                                lighting2.Command = "02";
                                            }
                                        }
                                    }


                                    TraceIt("RFXComm.WriteData(\"" + lighting2.ToString() + "\")", WriteToFile);

                                    RFXComm.WriteData(lighting2.ToString());

                                    PidOut(pid, deviceActionName);// dimLevel.ToString());
                                    //ReceivedList.Remove(pid);
                                    //Dictionary<string, object> updated = new Dictionary<string, object>();
                                    //updated.Add("command", dimLevel);
                                    //updated.Add("timestamp", DateTime.Now);
                                    //ReceivedList.Add(pid, updated);
                                    }
                                }
                                intercepted = false;
                                //intercepted = true;
                            }

                            catch { }
                            break;
                            #endregion

                        case "Lighting1":   // X10, ARC, ELRO, Waveman, EMW200, IMPULS, RisingSun, Philips
                        case "Lighting3":   // Ikea Koppla
                        case "Lighting4":   // PT2262
                        case "Lighting5":   // LightwaveRF, Siemens, EMW100, BBSB
                        case "Lighting6":   // Blyss

                        //case "1":  // Nexa, Code Switch, arctech
                        //case "2":  // Nexa, Self Learning On/Off, arctech
                        //case "3":  // Nexa, Self Learning Dimmer, arctech
                        //case "4":  // Nexa, Bell, arctech
                        //case "5":  // GAO, Code Switch, risingsun
                        //case "6":  // Waveman, Code Switch, waveman
                        //case "7":  // Elro, Code Switch, sartano
                        //case "8":  // HomeEasy, Code Switch, arctech
                        //case "9":  // HomeEasy, Self Learning Dimmer, arctech
                        //case "10":  // Intertechno, Code Switch, arctech
                        //case "11":  // Kjell o Company, Code Switch, risingsun
                        //case "12":  // KlikAndKlikUit, Code Switch, arctech
                        //case "13":  // KlikAndKlikUit, Self Learning On/Off, arctech
                        //case "14":  // KlikAndKlikUit, Self Learning Dimmer, arctech
                        //case "15":  // KlikAndKlikUit, Bell, arctech
                        //case "16":  // Chacon, Code Switch, arctech
                        //case "17":  // Chacon, Self Learning On/Off, arctech
                        //case "18":  // Chacon, Self Learning Dimmer, arctech
                        //case "19":  // Chacon, Bell, arctech
                        //case "20":  // Proove, Code Switch, arctech
                        //case "21":  // Proove, Self Learning On/Off, arctech
                        //case "22":  // Proove, Self Learning Dimmer, arctech
                        //case "23":  // Proove, Bell, arctech
                        //case "24":  // Sartano, Code Switch, sartano
                        //case "25":  // CoCo, Code Switch, arctech
                        //case "26":  // CoCo, Self Learning On/Off, arctech
                        //case "27":  // CoCo, Self Learning Dimmer, arctech
                        //case "28":  // CoCo, Bell, arctech
                        //case "29":  // Roxcore, Code Switch, brateck
                        //case "30":  // Ikea, Self Learning Dimmer, ikea
                        //case "31":  // HQ, Code Switch, fuhaote
                        //case "32":  // Conrad, Self Learning On/Off, risingsun
                        //case "33":  // GAO, Self Learning On/Off, everflourish
                        //case "34":  // HomeEasy, Self Learning On/Off, arctech
                        //case "35":  // Kappa, Code Switch, arctech
                        //case "36":  // Kappa, Self Learning On/Off, arctech
                        //case "37":  // Kappa, Self Learning Dimmer, arctech
                        //case "38":  // Kappa, Bell, arctech
                        //case "39":  // Rusta, Code Switch, sartano
                        //case "40":  // Rusta, Self Learning Dimmer, arctech
                        //case "41":  // UPM, Self Learning On/Off, upm
                        //case "42":  // X10, Code Switch, x10
                        //case "43":  // Intertechno, Self Learning On/Off, arctech
                        //case "44":  // Intertechno, Self Learning Dimmer, arctech
                        //case "45":  // Intertechno, Bell, arctech
                        //case "46":  // Elro, Code Switch, arctech
                        //case "47":  // Bye Bye Standby, Code Switch, arctech
                        //case "48":  // Goobay, Code Switch, yidong
                        //case "49":  // Otio, Self Learning On/Off, risingsun
                        //case "50":  // Ecosavers, Self Learning On/Off, silvanchip
                        //case "51":  // Brennenstuhl, Code Switch, sartano
                        //case "52":  // Hasta, Self Learning On/Off, hasta
                        default:
                            break;
                    }
                }
            }
        }
    }
}
