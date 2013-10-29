using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    partial class Plugin
    {
        public bool ParseXmlFiles()
        {
            try
            {
                GetTrancieversXml();
                GetDevicesXml();
                GetSensorsXml();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GetTrancieversXml()
        {
            try
            {
                XDocument xDocument = XDocument.Load(FilePath + "Trancievers.xml");
                //XDocument xDocument = XDocument.Load(@"C:\Program Files (x86)\Switch King\Switch King Server\Plugins\RFXtrx\RFXtrxPlugin.xml");
                var version = xDocument.Element("RFXtrxPlugin").FirstAttribute.Value;
                foreach (var item in xDocument.Element("RFXtrxPlugin").Element("Units").Elements())
                {                   // item = <Unit>...</Unit>
                    Dictionary<string, object> transing = new Dictionary<string, object>();
                    string name = "";
                    foreach (var field in item.Elements())
                    {
                        switch (field.Name.ToString())
                        {
                            case "Name":
                                name = field.Value.ToString();
                                break;
                            case "Default":
                                transing.Add(field.Name.ToString(), field.Value.ToString().ToLower().Equals("true"));
                                break;
                            default:
                                transing.Add(field.Name.ToString(), field.Value.ToString());
                                break;
                        }
                    }
                    Transcievers.Add(name, transing);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GetDevicesXml()
        {
            try
            {
                Dictionary<string, Dictionary<string, bool>> NewOne = new Dictionary<string, Dictionary<string, bool>>();
                XDocument xDocument = XDocument.Load(FilePath + "Devices.xml");
                //XDocument xDocument = XDocument.Load(@"C:\Program Files (x86)\Switch King\Switch King Server\Plugins\RFXtrx\RFXtrxPlugin.xml");
                var version = xDocument.Element("RFXtrxPlugin").FirstAttribute.Value;
                foreach (var item in xDocument.Element("RFXtrxPlugin").Element("Devices").Elements())
                {                   // item = <Device>...</Device>
                    Dictionary<string, bool> ThisOne = new Dictionary<string, bool>();
                    string pid = "";
                    foreach (var field in item.Elements())
                    {
                        switch (field.Name.ToString())
                        {
                            case "ID":
                                pid = field.Value.ToString().WithPrefix(DevicePrefix);
                                break;
                            default:
                                ThisOne.Add(field.Name.ToString(), field.Value.ToString().ToLower().Equals("true"));
                                break;
                        }
                    }
                    //if (WhichOne.ContainsKey(pid)) { WhichOne.Remove(pid); }
                    NewOne.Add(pid, ThisOne);
                }
                WhichOne = NewOne;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GetSensorsXml()
        {
            try
            {
                Dictionary<string, Dictionary<string, string>> MakeSens = new Dictionary<string, Dictionary<string, string>>();
                XDocument xDocument = XDocument.Load(FilePath + "Sensors.xml");
                //XDocument xDocument = XDocument.Load(@"C:\Program Files (x86)\Switch King\Switch King Server\Plugins\RFXtrx\RFXtrxPlugin.xml");
                var version = xDocument.Element("RFXtrxPlugin").FirstAttribute.Value;
                foreach (var item in xDocument.Element("RFXtrxPlugin").Element("Sensors").Elements())
                {                   // item = <Sensor>...</Sensor>
                    Dictionary<string, string> sensor = new Dictionary<string, string>();
                    string pid = "";
                    foreach (var field in item.Elements())
                    {
                        switch (field.Name.ToString())
                        {
                            case "ID":
                                pid = field.Value.ToString().WithPrefix(SourcePrefix);
                                break;
                            default:
                                sensor.Add(field.Name.ToString(), field.Value.ToString());
                                break;
                        }
                    }
                    //if (SensorMappings.ContainsKey(thisID)) { SensorMappings.Remove(thisID); }
                    MakeSens.Add(pid, sensor);
                }
                SensorMappings = MakeSens;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetRFXtrxConfigs()
        {
            RFXtrxModeCommand modeCommand = new RFXtrxModeCommand();

            modeCommand.Command = "03";
            
            // Set default values
            modeCommand.EnableAC(true);
            modeCommand.EnableAD(true);
            modeCommand.EnableAE(false);
            modeCommand.EnableARC(true);
            modeCommand.EnableATI(true);
            modeCommand.EnableBlindsT0(false);
            modeCommand.EnableBlindsT1T2T3(false);
            modeCommand.EnableFineOffsetViking(false);
            modeCommand.EnableHidekiUPM(true);
            modeCommand.EnableHomeEasy(true);
            modeCommand.EnableLaCrosse(true);
            modeCommand.EnableMeiantech(false);
            modeCommand.EnableMertik(false);
            modeCommand.EnableOregonScientific(true);
            modeCommand.EnableRubicson(false);
            modeCommand.EnableUndecoded(false);
            modeCommand.EnableX10(true);

            try
            {
                XDocument xDocument = XDocument.Load(Path.Combine(FilePath, "Configs.xml"));
                //XDocument xDocument = XDocument.Load(@"C:\Program Files (x86)\Switch King\Switch King Server\Plugins\RFXtrx\RFXtrxPlugin.xml");
                var version = xDocument.Element("RFXtrxPlugin").FirstAttribute.Value;
                foreach (var item in xDocument.Element("RFXtrxPlugin").Element("Configs").Elements())
                {                   // item = <Config>...</Config>
                    Dictionary<string, object> transing = new Dictionary<string, object>();
                    string name = "";
                    foreach (var field in item.Elements())
                    {
                        bool yes = field.Value.ToString().ToLower().Equals("true");
                        switch (field.Name.ToString())
                        {
                            case "Name":
                                name = field.Value.ToString();
                                break;
                            case "EnableAC":
                                modeCommand.EnableAC(yes);
                                break;
                            case "EnableAD":
                                modeCommand.EnableAD(yes);
                                break;
                            case "EnableAE":
                                modeCommand.EnableAE(yes);
                                break;
                            case "EnableARC":
                                modeCommand.EnableARC(yes);
                                break;
                            case "EnableATI":
                                modeCommand.EnableATI(yes);
                                break;
                            case "EnableBlindsT0":
                                modeCommand.EnableBlindsT0(yes);
                                break;
                            case "EnableBlindsT1T2T3":
                                modeCommand.EnableBlindsT1T2T3(yes);
                                break;
                            case "EnableFineOffsetViking":
                                modeCommand.EnableFineOffsetViking(yes);
                                break;
                            case "EnableHidekiUPM":
                                modeCommand.EnableHidekiUPM(yes);
                                break;
                            case "EnableHomeEasy":
                                modeCommand.EnableHomeEasy(yes);
                                break;
                            case "EnableLaCrosse":
                                modeCommand.EnableLaCrosse(yes);
                                break;
                            case "EnableMeiantech":
                                modeCommand.EnableMeiantech(yes);
                                break;
                            case "EnableMertik":
                                modeCommand.EnableMertik(yes);
                                break;
                            case "EnableOregonScientific":
                                modeCommand.EnableOregonScientific(yes);
                                break;
                            case "EnableRubicson":
                                modeCommand.EnableRubicson(yes);
                                break;
                            case "EnableUndecoded":
                                modeCommand.EnableUndecoded(yes);
                                break;
                            case "EnableX10":
                                modeCommand.EnableX10(yes);
                                break;
                            default:
                                break;
                        }
                    }
                }
                //RFXComPorts[name].WriteData(modeCommand.ToString());
                RFXComm.WriteData(modeCommand.ToString());
                TraceIt("modeCommand: " + modeCommand.ToString(), WriteToFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Specify what is done when a file is changed, created, or deleted.
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            string path = e.FullPath.Substring(0, e.FullPath.LastIndexOf(@"\") + 1);
            string file = e.FullPath.Substring(e.FullPath.LastIndexOf(@"\") + 1, e.FullPath.Length - (e.FullPath.LastIndexOf(@"\") + 1));
            XDocument xDoc = new XDocument();

            switch (e.ChangeType)
            {

                #region FileChangeChanged

                case WatcherChangeTypes.Changed:

                    try
                    {
                        var loaded = false;
                        int i = 0;
                        do
                        {
                            try
                            {
                                xDoc = XDocument.Load(e.FullPath);
                                loaded = true;
                            }
                            catch
                            { i++; }
                        }
                        while (!loaded & (i < 10));
                    }
                    catch
                    { }

                    switch (file)
                    {
                        case "Trancievers.xml":
                            try
                            {
                                //GetTrancieversXml();
                            }
                            catch
                            { }
                            break;

                        case "Configs.xml":
                            try
                            {
                                SetRFXtrxConfigs();
                            }
                            catch
                            { }
                            break;

                        case "Devices.xml":
                            try
                            {
                                GetDevicesXml();
                            }
                            catch
                            { }
                            break;

                        case "Sensors.xml":
                            try
                            {
                                GetSensorsXml();
                            }
                            catch
                            { }
                            break;

                        default:
                            break;
                    }
                    break;

                #endregion FileChangeChanged

                #region FileChangeCreated
                case WatcherChangeTypes.Created:

                    try
                    {
                        var loaded = false;
                        int i = 0;
                        do
                        {
                            try
                            {
                                xDoc = XDocument.Load(e.FullPath);
                                loaded = true;
                            }
                            catch
                            { i++; }
                        }
                        while (!loaded & (i < 10));
                    }
                    catch
                    { }

                    switch (file)
                    {
                        case "Trancievers.xml":
                            try
                            {
                                //GetTrancieversXml();
                            }
                            catch
                            { }
                            break;

                        case "Configs.xml":
                            try
                            {
                                SetRFXtrxConfigs();
                            }
                            catch
                            { }
                            break;

                        case "Devices.xml":
                            try
                            {
                                GetDevicesXml();
                            }
                            catch
                            { }
                            break;

                        case "Sensors.xml":
                            try
                            {
                                GetSensorsXml();
                            }
                            catch
                            { }
                            break;

                        default:
                            break;
                    }
                    break;
                #endregion FileChangeCreated

                #region FileChangeDeleted
                case WatcherChangeTypes.Deleted:
                    
                    switch (file)
                    {
                        case "Trancievers.xml":
                            try
                            {
                                //GetTrancieversXml();
                            }
                            catch
                            { }
                            break;

                        case "Configs.xml":
                            try
                            {
                                SetRFXtrxConfigs();
                            }
                            catch
                            { }
                            break;

                        case "Devices.xml":
                            try
                            {
                                GetDevicesXml();
                            }
                            catch
                            { }
                            break;

                        case "Sensors.xml":
                            try
                            {
                                GetSensorsXml();
                            }
                            catch
                            { }
                            break;

                        default:
                            break;
                    }
                    break;
                #endregion FileChangeDeleted

                default:
                    break;
            }
        }

        #region FileChangeRenamed
        public void OnRenamed(object source, RenamedEventArgs e)
        {
        }
        #endregion FileChangeRenamed

    }
}
