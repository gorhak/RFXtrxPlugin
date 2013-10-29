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
using SwitchKing.Server.Plugins.Interfaces;
using SwitchKing.Server.Plugins.RFXtrx.Diagnostics;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    [Plugins.ServerPlugIn("SwitchKing's RFXtrx Plugin.")]
    public partial class Plugin :
        ICommandInterceptor,
        IDataSourceValueEventListener,
        //IDeviceEventListener,
        //IInvocationLoopListener,
        //IMonitoredEventInterceptor,
        //IRESTInterceptor,
        //IScenarioEventListener,
        //ISystemModeEventListener,
        //ITelldusEventListener,
        IDisposable
    {
        // From Switch Kings constants...
        #region Constants
        public const string PARAM_NAME__DEVICE_TYPE_NAME = "DeviceTypeName";
        public const string PARAM_NAME__DEVICE_TYPE_ID = "DeviceTypeID";
        public const string PARAM_NAME__DEVICE_TYPE_MANUFACTURER = "DeviceTypeManufacturer";
        public const string PARAM_NAME__DEVICE_TYPE_CATEGORY = "DeviceTypeCategory";
        public const string PARAM_NAME__DEVICE_TYPE_DEVICE_TYPE_PROTOCOL = "DeviceTypeProtocol";
        public const string PARAM_NAME__DEVICE_TYPE_MODEL = "DeviceTypeModel";
        public const string PARAM_NAME__FEATURED_DIM_LEVEL = "DimLevel";
        public const string PARAM_NAME__FEATURED_EVENT = "Event";
        public const string PARAM_NAME__EXISTING_DEVICE = "ExistingDevice";
        public const string PARAM_NAME__OPERATION = "Operation";
        public const string PARAM_NAME__SOURCE = "Source";
        public const string PARAM_NAME__TIMESTAMP = "Timestamp";
        public const string PARAM_NAME__DEVICE_NAME = "DeviceName";
        public const string PARAM_NAME__DEVICE_ID = "DeviceId";
        public const string PARAM_NAME__DEVICE_NATIVE_ID = "DeviceNativeId";
        public const string PARAM_NAME__DEVICE_CODE = "DeviceCode";
        public const string PARAM_NAME__SENSOR_VALUE = "Value";
        public const string PARAM_NAME__SENSOR_MODEL = "Model";
        public const string PARAM_NAME__SENSOR_PROTOCOL = "Protocol";
        public const string PARAM_NAME__SENSOR_ID = "SensorID";
        public const string PARAM_NAME__SENSOR_TYPE = "SensorType";
        public const string FEATURED_EVENT__TURN_ON = "TurnOn";
        public const string FEATURED_EVENT__TURN_OFF = "TurnOff";
        public const string PARAM_NAME__RFX_PACKET_TYPE1 = "Packettype";
        public const string PARAM_NAME__RFX_PACKET_TYPE2 = "PacketType";
        public const string PARAM_NAME__RFX_SUBTYPE = "Subtype";
        public const string PARAM_NAME__RFX_ID = "ID";
        public const string PARAM_NAME__RFX_SEQNBR = "SeqNbr";
        public const string PARAM_NAME__RFX_SEQNBR_TEXT = "SeqNbr_Text";
        public const string PARAM_NAME__RFX_SIGNALLEVEL = "SignalLevel";
        public const string PARAM_NAME__RFX_UNIT = "Unit";
        public const string PARAM_NAME__RFX_COMMAND = "Command";
        public const string PARAM_NAME__RFX_COMMAND_TEXT = "Command_Text";
        #endregion Constants

        public static dBFunctions ServerDB;
        
        public Dictionary<string, CommunicationManager> RFXComPorts = new Dictionary<string, CommunicationManager>();
        public CommunicationManager RFXComm = new CommunicationManager();

        public static bool Append, WriteToFile;

        public static string HostName, HostPort, UserName, Password, RFXtrxPort, FilePath, DevicePrefix, SourcePrefix, Threshold, StatusPath;
        
        //public TimeSpan CommandThreshold;
        
        public static Dictionary<string, Dictionary<string, string>> SensorMappings = new Dictionary<string, Dictionary<string, string>>();

        public static Dictionary<string, Dictionary<string, object>> ReceivedList = new Dictionary<string, Dictionary<string, object>>();
        public static Dictionary<string, Dictionary<string, object>> Transcievers = new Dictionary<string, Dictionary<string, object>>();
        public static Dictionary<string, Dictionary<string, bool>> WhichOne = new Dictionary<string, Dictionary<string, bool>>();

        public Plugin()
        {
            var appConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            try { HostName      = appConfig.AppSettings.Settings["HostName"].Value; }       catch { HostName        = @"localhost"; }
            try { HostPort      = appConfig.AppSettings.Settings["HostPort"].Value; }       catch { HostPort        = @"8800"; }
            try { UserName      = appConfig.AppSettings.Settings["UserName"].Value; }       catch { UserName        = @"user"; }
            try { Password      = appConfig.AppSettings.Settings["Password"].Value; }       catch { Password        = @"pass"; }
            try { RFXtrxPort    = appConfig.AppSettings.Settings["RFXtrxPort"].Value; }     catch { RFXtrxPort      = @"COM4"; }
            try { FilePath      = appConfig.AppSettings.Settings["FilePath"].Value; }       catch { FilePath        = @".\RFXtrx\"; }
            try { DevicePrefix  = appConfig.AppSettings.Settings["DevicePrefix"].Value; }   catch { DevicePrefix    = @"D"; }
            try { SourcePrefix  = appConfig.AppSettings.Settings["SourcePrefix"].Value; }   catch { SourcePrefix    = @"S"; }
            try { Threshold     = appConfig.AppSettings.Settings["Threshold"].Value; }      catch { Threshold       = @"2000"; }
            try { StatusPath    = appConfig.AppSettings.Settings["StatusPath"].Value; }     catch { StatusPath      = @"C:\MyDataSource\"; }
            try { Append        = appConfig.AppSettings.Settings["Append"].Value.ToLower().Equals("true"); }        catch { Append      = false; }
            try { WriteToFile   = appConfig.AppSettings.Settings["WriteToFile"].Value.ToLower().Equals("true"); }   catch { WriteToFile = false; }

            string ds = Path.DirectorySeparatorChar.ToString();
            string ads = Path.AltDirectorySeparatorChar.ToString();

            FilePath = CheckEnvironmentPath(FilePath);
            StatusPath = CheckEnvironmentPath(StatusPath);
            //if (String.IsNullOrEmpty(FilePath)) { FilePath = @"." + ds; }
            //FilePath = FilePath.Replace(ads, ds);
            //if (FilePath.StartsWith(@"." + ds)) { FilePath = FilePath.Replace(@"." + ds, appConfig.FilePath.Substring(0, (appConfig.FilePath.LastIndexOf(ds) + 1))); }
            //if (!(FilePath.LastIndexOf(ds) == (FilePath.Length - 1))) { FilePath += ds; }
            //if (!(StatusPath.LastIndexOf(ds) == (StatusPath.Length - 1))) { StatusPath += ds; }

            if (File.Exists(Path.Combine(StatusPath, @"RFXtrxPluginTrace.txt"))) { File.Delete(Path.Combine(StatusPath, @"RFXtrxPluginTrace.txt")); }
            Trace.Listeners.Add(new TextWriterTraceListener(Path.Combine(StatusPath, @"RFXtrxPluginTrace.txt")));
            Trace.AutoFlush = true;

            try
            {
                ServerDB = new dBFunctions();

                ParseXmlFiles();
                
                RFXComm.PortName = RFXtrxPort;
                RFXComm.BaudRate = "38400";
                RFXComm.DataBits = "8";
                RFXComm.StopBits = "One";
                RFXComm.Parity = "None";
                RFXComm.CurrentTransmissionType = CommunicationManager.TransmissionType.Hex;

                RFXComm.MessageArrived += new MessageHandler(EventReceived);

                RFXComm.OpenPort();
                //reset RFXtrx
                RFXComm.WriteData("0D00000000000000000000000000");
                Thread.Sleep(200);
                RFXComm.WriteData("0D00000102000000000000000000");

                SetRFXtrxConfigs();
                //StartPollRFX(RFXComm);

                #region FileWatcher
                {
                    FileSystemWatcher watch = new FileSystemWatcher();
                    watch.Path = FilePath;

                    // Only watch xml files.
                    watch.Filter = "*.xml";

                    watch.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                    
                    watch.Changed += new FileSystemEventHandler(OnChanged);
                    watch.Created += new FileSystemEventHandler(OnChanged);
                    watch.Deleted += new FileSystemEventHandler(OnChanged);
                    //watch.Renamed += new RenamedEventHandler(OnRenamed);

                    watch.EnableRaisingEvents = true;
                }
                #endregion FileWatcher
            
            }
            catch (System.Exception ex)
            {
                var msg = String.Format("Can't Open RFXtrx on {0}!" + ex.Message, RFXtrxPort);

                try { EventLogWriter.WriteToLog(System.Diagnostics.EventLogEntryType.Information, msg); }
                catch { }
            }

            try
            {
                if (!System.IO.Directory.Exists(FilePath))
                    { throw (new System.IO.DirectoryNotFoundException(String.Format("Can't find {0}.", FilePath))); }

                #region LogWrite
                {
                    var msg = String.Format("Starting plugin RFXtrx.");

                    try { EventLogWriter.WriteToLog(System.Diagnostics.EventLogEntryType.Information, msg); } catch { }
                }
                #endregion LogWrite
            }
            catch (System.Exception ex)
            {
                var msg = String.Format("Can't Start plugin RFXtrx!" + ex.Message);

                try { EventLogWriter.WriteToLog(System.Diagnostics.EventLogEntryType.Information, msg); } catch { }
            }
        }

        #region SomeFunctions

        private string CheckEnvironmentPath(string filePath)
        {
            var appConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            string ds = Path.DirectorySeparatorChar.ToString();
            if (String.IsNullOrEmpty(filePath)) { filePath = @"." + ds; }
            filePath = filePath.Replace(@"/", ds).Replace(@"\", ds);
            if (filePath.StartsWith(@"." + ds)) { filePath = filePath.Replace(@"." + ds, appConfig.FilePath.Substring(0, (appConfig.FilePath.LastIndexOf(ds) + 1))); }
            if (!(filePath.LastIndexOf(ds) == (filePath.Length - 1))) { filePath += ds; }
            return filePath;
        }

        //private void StartPollRFX(CommunicationManager RFX)
        //{
        //    RFXPolling RFXPolling = new RFXPolling(RFX);
        //    // Create the thread object, passing in the
        //    // serverObject.InstanceMethod method using a
        //    // ThreadStart delegate.
        //    Thread InstanceCaller = new Thread(new ThreadStart(RFXPolling.Poll));
        //    InstanceCaller.Name = "RFX".ToString();
                
        //    // Start the thread.
        //    InstanceCaller.Start();
        //}

        public static void SendRestCommand(string url)
        {
            TraceIt("SendRestCommand " + url.ToString(), Plugin.WriteToFile);
            {
                SendRestCommand sendRestCommand = new SendRestCommand(@url, UserName, Password);

                // Create the thread object, passing in the
                // serverObject.InstanceMethod method using a
                // ThreadStart delegate.
                Thread InstanceCaller = new Thread(new ThreadStart(sendRestCommand.SendString));
                InstanceCaller.Name = url.ToString();

                //try
                //{
                //    var test = new SwitchKing.Server.Plugins.LittlePrince.sentTimeList[pid] = DateTime.Now;
                //}
                //catch { }

                // Start the thread.
                InstanceCaller.Start();
            }
        }

        public static string CreateUrl(string pid, string ActionName)
        {
            var url = "http://" + HostName + ":" + HostPort;

            if (pid.StartsWith(DevicePrefix))
            {
                url += "/devices/";
                string id = pid.WithoutPrefix(DevicePrefix);
                decimal newDimLevel;
                if (ActionName.Equals("TurnOn"))
                {
                    url += id + "/turnon";
                }
                else if (ActionName.Equals("TurnOff"))
                {
                    url += id + "/turnoff";
                }
                else if (ActionName.TryParseAsDecimal(out newDimLevel) && (1 <= newDimLevel) && (newDimLevel <= 100))
                {
                    Math.Round(newDimLevel);
                    url += id + "/dim/" + newDimLevel.ToString();
                }
                else
                {
                    url += id + "/turnoff";
                }
            }
            else if (pid.StartsWith(SourcePrefix))
            {
                url += "/datasources/";
                url += pid.WithoutPrefix(SourcePrefix);
                url += "/addvalue?value=";
                url += ActionName;
            }
            return url;
        }

        public static void TraceIt(string methodstr, bool WriteToFile)
        {
            if (WriteToFile)
            {
                var msg = String.Format(DateTime.Now.ToString() + ": " + methodstr);
                System.Diagnostics.Trace.WriteLine(msg);
            }
        }

        #endregion SomeFunctions

        //Defines an interface that enables a plugin to implement, for example, an intercepting feature.
        //This enables the plugin to stop signals from being sent to the actual devices and gives the plugin the option to handling the action by itself.
        #region ICommandInterceptor Members

        //Called when a command is being sent to a specific device.
        public void OnCommand(int deviceId, string deviceName, int deviceActionId, string deviceActionName, int? dimLevel, string eventSource, out bool intercepted)
        {
            // If intercepted set to true, the server will not send real signals to the device.
            //deviceId: The id of the device targetted.
            //deviceName: The name of the device targetted.
            //deviceActionId: The id of the action about to be performed.
            //deviceActionName: The name of the action about to be performed.
            //dimLevel: The dim level about to be sent to the device.
            //eventSource: The source of the event.
            //intercepted: Set by the implementing class. If set to true, the server will not send real signals to the device.

            string id = deviceId.ToString();
            string pid = id.WithPrefix(DevicePrefix);
            TraceIt("OnCommand " + pid, WriteToFile);
            intercepted = false;

            if (ReceivedList.ContainsKey(pid))
            {
                if (ReceivedList[pid]["command"].Equals(deviceActionName.Replace(" ","")))
                {
                    if (DateTime.Now < Convert.ToDateTime(ReceivedList[pid]["timestamp"]).AddMilliseconds(Convert.ToDouble(Threshold)))
                    {
                        intercepted = true;
                    }
                }
            }
            
            if (!intercepted)
            {
                if (WhichOne.ContainsKey(pid) ? WhichOne[pid]["RFXtrx"] : Transcievers["RFXtrx"]["Default"].Equals(true))
                {
                    RFXtrxOnCommand(deviceId, deviceName, deviceActionId, deviceActionName, dimLevel, eventSource, out intercepted);
                }
            }

            intercepted |= WhichOne.ContainsKey(pid) ? !WhichOne[pid]["Tellstick"] : !Transcievers["Tellstick"]["Default"].Equals(true);
        }

        #endregion ICommandInterceptor Members

        // Defines an interface that may be used to listen to data source value events.
        #region IDataSourceValueEventListener Members

        // Triggered when the data source value has been saved to the database.  Opens up for requesting actions to be taken after a value has been saved.
        public void OnDataSourceValueSaved(int dataSourceId, string value, DateTime timestamp, DateTime localTimestamp, DateTime expires, bool isFailureValue, List<object> entityParams)
        {
            // Not handled
            var id = dataSourceId.ToString();
            var pid = id.WithPrefix(SourcePrefix);
            TraceIt("OnDataSourceValueSaved " + pid, WriteToFile);
            return;
        }

        // Triggered when the data source value is about to be saved.  Opens up for modifying data before it is saved.
        public bool OnDataSourceValueSaving(int dataSourceId, string value, DateTime timestamp, DateTime localTimestamp, DateTime expires, bool isFailureValue, List<object> entityParams, out int dataSourceIdOut, out string valueOut, out DateTime timestampOut, out DateTime localTimestampOut, out DateTime expiresOut, out bool isFailureValueOut)
        {
            // Should return false if the plugin does not override the data source value items.
            var id = dataSourceId.ToString();
            var pid = id.WithPrefix(SourcePrefix);
            TraceIt("OnDataSourceValueSaving " + pid, WriteToFile);
            valueOut = value;
            dataSourceIdOut = dataSourceId;
            timestampOut = timestamp;
            localTimestampOut = localTimestamp;
            expiresOut = expires;
            isFailureValueOut = isFailureValue;

            return false;
        }

        #endregion IDataSourceValueEventListener Members

        #region Unused

        //Defines an interface that may be used to listen to events tied to devices.
        #region IDeviceEventListener Members

        ////Called when a device has been deleted.
        //public void OnDeviceDeleted(int deviceId, string deviceNativeId, string deviceName, Dictionary<string, object> data)
        //{
        //    // Not Handeled
        //    string id = deviceId.ToString();
        //    string pid = id.WithPrefix(DevicePrefix);
        //    TraceIt("OnDeviceDeleted " + pid, WriteToFile);
        //}

        ////Called when an event is being stored in the transmission queue for a specific device.
        //public void OnDeviceEventEnqueued(int deviceId, string deviceNativeId, string deviceName, string deviceActionName, int? deviceActionDimLevel, Dictionary<string, object> data)
        //{
        //    // Not Handeled
        //    string id = deviceId.ToString();
        //    string pid = id.WithPrefix(DevicePrefix);
        //    //enqueuedList.Add(pid);
        //    TraceIt("OnDeviceEventEnqueued " + pid, WriteToFile);
        //}

        ////Called when a device has been saved.
        //public void OnDeviceSaved(int deviceId, string deviceNativeId, string deviceName, bool deviceIsNew, Dictionary<string, object> data)
        //{
        //    // Not Handeled
        //    string id = deviceId.ToString();
        //    string pid = id.WithPrefix(DevicePrefix);
        //    TraceIt("OnDeviceSaved " + pid, WriteToFile);
        //}

        ////Called when a signal is sent to a specific device.
        //public void OnDeviceSignalSent(int deviceId, string deviceNativeId, string deviceName, Dictionary<string, object> data)
        //{
        //    // Not Handeled
        //    string id = deviceId.ToString();
        //    string pid = id.WithPrefix(DevicePrefix);
        //    TraceIt("OnDeviceSignalSent " + pid, WriteToFile);
        //}

        #endregion IDeviceEventListener Members

        //Defines an interface that enables a plugin to implement, for example, a repeating event.  Called on every loop for the main thread in the server.
        #region IInvocationLoopListener Members

        ////Called during every loop in the main thread.
        //public void OnLoop()
        //{
        //    //TraceIt("OnLoop", WriteToFile);
        //    foreach (var timer in Timers)
        //    {
        //        if (Convert.ToDateTime(timer.Value["End"]) < DateTime.Now)
        //        {
        //            timeOut.Add(timer.Key, "TurnOff");
        //        }
        //    }
        //    if (0 < timeOut.Count)
        //    {
        //        foreach (var unit in timeOut)
        //        {
        //            var item = Timers[unit.Key];
        //            item["End"] = DateTime.MaxValue;
        //            UnitStatus[unit.Key] = "TurnOff";
        //        }
        //        foreach (var item in timeOut) TraceIt("ToUU:OnLoop:timeOut.Contains:" + item.Key + " " + item.Value, WriteToFile);
        //        UpdateUnits(ref timeOut);
        //    }
        //}

        #endregion IInvocationLoopListener Members

        #region IMonitoredEventInterceptor Members

        //// Called when a device event (commonly Telldus Device Event) has been received.
        //public bool OnDeviceEventReceived(IDictionary<string, object> eventParams)
        //{
        //    // Return true if params have been modified, false otherwise.
        //    string id = eventParams[PARAM_NAME__DEVICE_ID].ToString();
        //    string pid = id.WithPrefix(DevicePrefix);

        //    TraceIt("OnDeviceEventReceived pid:" + pid, WriteToFile);
        //    string deviceEvent = eventParams[PARAM_NAME__FEATURED_EVENT].ToString();
        //    bool dimMode = deviceEvent.Equals("Dim");

        //    EventParamsToFile(eventParams, WriteToFile);
        //    //EventParamsToFile(eventParams, StatusPath + "OnDeviceEventReceived.txt", WriteToFile);

        //    return false;
        //}
        
        //// Called when a raw event (commonly Telldus Raw Event) has been received.
        //public bool OnRawEventReceived(IDictionary<string, object> eventParams)
        //{
        //    //True if data has been modified by the implementing class, false otherwise.
        //    TraceIt("OnRawEventReceived:", WriteToFile);
        //    EventParamsToFile(eventParams, WriteToFile);
        //    //EventParamsToFile(eventParams, StatusPath + "OnRawEventReceived.txt", WriteToFile);

        //    return false;
        //}

        //// Called when a sensor event (commonly Telldus Sensor Event) has been received.
        //public bool OnSensorEventReceived(IDictionary<string, object> eventParams)
        //{
        //    // EventParamsToFile(eventParams, StatusPath + "OnSensorEventReceived.txt", WriteToFile);
        //    TraceIt("OnSensorEventReceived:", WriteToFile);
            
        //    // True if data has been modified by the implementing class, false otherwise.
        //    EventParamsToFile(eventParams, WriteToFile);
        //    //EventParamsToFile(eventParams, StatusPath + "OnSensorEventReceived.txt", WriteToFile);
        //    return false;
        //    // Not handled
        //}

        //private static void EventParamsToFile(IDictionary<string, object> eventParams, bool WriteToFile)
        //{
        //    if (WriteToFile)
        //    {
        //        Trace.Indent();
        //        foreach (var item in eventParams)
        //        {
        //            if (item.Value != null)
        //            {
        //                TraceIt(item.Key.ToString() + ": " + item.Value.ToString(), WriteToFile);
        //            }
        //            else
        //            {
        //                TraceIt(item.Key.ToString() + ": " + "NULL", WriteToFile);
        //            }
        //        }
        //        Trace.Unindent();
        //    }
        //}

        //private static void EventParamsToFile(IDictionary<string, object> eventParams, string DebugFile, bool WriteToFile)
        //{
        //    if (WriteToFile)
        //    {
        //        foreach (var item in eventParams)
        //        {
        //            if (item.Value != null)
        //            {
        //                System.IO.File.AppendAllText(DebugFile, item.Key.ToString() + ": " + item.Value.ToString() + Environment.NewLine);
        //            }
        //            else
        //            {
        //                System.IO.File.AppendAllText(DebugFile, item.Key.ToString() + ": " + "NULL" + Environment.NewLine);
        //            }
        //        }
        //        System.IO.File.AppendAllText(DebugFile, Environment.NewLine);
        //    }
        //}

        #endregion IMonitoredEventInterceptor Members

        #region IRESTInterceptor Members

        //public string InterceptAddCommand(string operation, string target, string param1, string param2, string param3, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return operation;
        //}

        //public byte[] InterceptAddCommand(byte[] item, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return item;
        //}

        //public string InterceptCancelDeviceGroupSemiAutoByDeviceGroupId(string id, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public string InterceptCancelDeviceSemiAutoByDeviceId(string id, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public string InterceptDimDeviceByDeviceId(string id, string level, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public string InterceptDimDeviceGroupByDeviceGroupId(string id, string level, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public byte[] InterceptGetActiveScenario(byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDataSourceById(string id, byte[] collectedByServer)
        //{
        //    // Not Handeled
        //    return collectedByServer;
        //}

        //public byte[] InterceptGetDataSourceGroupById(string id, string includeDevices, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDataSourceGroupById(string id, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDataSourceGroups(string includeDevices, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDataSourceGroups(byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDataSources(byte[] collectedByServer)
        //{
        //    // Not Handeled
        //    return collectedByServer;
        //}

        //public byte[] InterceptGetDeviceById(string id, byte[] collectedByServer)
        //{
        //    // Not Handeled
        //    return collectedByServer;
        //}

        //public byte[] InterceptGetDeviceGroupById(string id, string includeDevices, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDeviceGroupById(string id, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDeviceGroups(string includeDevices, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDeviceGroups(byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetDevices(string groupId, byte[] collectedByServer)
        //{
        //    // Not Handeled
        //    return collectedByServer;
        //}

        //public byte[] InterceptGetDevices(byte[] collectedByServer)
        //{
        //    // Not Handeled
        //    return collectedByServer;
        //}

        //public byte[] InterceptGetEntityLogEntries(int maxCount, DateTime newerThan, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetFutureEvents(int maxCount, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetHistoricEvents(int maxCount, byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetLatestEntityLogEntry(byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public byte[] InterceptGetScenarios(byte[] collectedFromServer)
        //{
        //    // Not Handeled
        //    return collectedFromServer;
        //}

        //public string InterceptSynchronizeDeviceByDeviceId(string id, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public string InterceptSynchronizeDeviceGroupByDeviceGroupId(string id, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public string InterceptTurnDeviceGroupOffByDeviceGroupId(string id, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public string InterceptTurnDeviceGroupOnByDeviceGroupId(string id, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public string InterceptTurnDeviceOffByDeviceId(string id, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        //public string InterceptTurnDeviceOnByDeviceId(string id, out bool intercepted)
        //{
        //    // Not Handeled
        //    intercepted = false;
        //    return id;
        //}

        #endregion IRESTInterceptor Members

        //Defines an interface that enables a plugin to implement listening to Scenario changes.
        #region IScenarioEventListener Members

        ////Called when a scenario has been changed.
        //public void OnScenarioChanged(int scenarioId, string scenarioName, Dictionary<string, object> data)
        //{
        //    var pid = scenarioId.ToString().WithPrefix(sCenarioPrefix);
        //    TraceIt("OnScenarioChanged " + pid, WriteToFile);

        //    foreach (var item in unitList)
        //    {
        //        if (item.StartsWith(sCenarioPrefix))
        //        {
        //            if (item.Equals(pid))
        //            {
        //                if (UnitStatus.ContainsKey(item)) { UnitStatus[item] = "TurnOn"; }
        //                else { UnitStatus.Add(item, "TurnOn"); }
        //            }
        //            else
        //            {
        //                if (UnitStatus.ContainsKey(item)) { UnitStatus[item] = "TurnOff"; }
        //                else { UnitStatus.Add(item, "TurnOff"); }
        //            }
        //        }
        //    }
        //}

        #endregion IScenarioEventListener Members

        //Defines an interface that enables a plugin to implement listening to SystemMode changes.
        #region ISystemModeEventListener Members

        //void ISystemModeEventListener.OnSystemModeChanged(int newSystemModeId, string newSystemModeName, string newSystemModeAbbreviation, int? previousSystemModeId, string previousSystemModeName, string previousSystemModeAbbreviation)
        //{
        //    var pid = newSystemModeId.ToString().WithPrefix(ModePrefix);
        //    TraceIt("OnSystemModeChanged " + pid, WriteToFile);

        //    foreach (var item in unitList)
        //    {
        //        if (item.StartsWith(ModePrefix))
        //        {
        //            if (item.Equals(pid))
        //            {
        //                if (UnitStatus.ContainsKey(item)) { UnitStatus[item] = "TurnOn"; }
        //                else { UnitStatus.Add(item, "TurnOn"); }
        //            }
        //            else
        //            {
        //                if (UnitStatus.ContainsKey(item)) { UnitStatus[item] = "TurnOff"; }
        //                else { UnitStatus.Add(item, "TurnOff"); }
        //            }
        //        }
        //    }
        //}

        #endregion ISystemModeEventListener Members

        #region ITelldusEventListener Members

        //public void OnDeviceChangeEventReceived(int deviceId, int changeEvent, int changeType)
        //{
        //    // Not Handeled
        //    string id = deviceId.ToString();
        //    string pid = id.WithPrefix(DevicePrefix);
        //    TraceIt("ITelldusEventListener:OnDeviceChangeEventReceived " + pid, WriteToFile);
        //}

        //public void OnDeviceEventReceived(int deviceId, string method, string data)
        //{
        //    // Not Handeled
        //    string id = deviceId.ToString();
        //    string pid = id.WithPrefix("Telldus");
        //    TraceIt("ITelldusEventListener:OnDeviceEventReceived " + pid + " " + method + " " + data, WriteToFile);
        //}

        //public void OnRawEventReceived(string data)
        //{
        //    // Not Handeled
        //    //string id = deviceId.ToString();
        //    //string pid = id.WithPrefix(DevicePrefix);
        //    TraceIt("ITelldusEventListener:OnRawEventReceived " + data, WriteToFile);
        //}

        //public void OnSensorEventReceived(string protocol, string model, int id, string dataType, string val, DateTime timestamp)
        //{
        //    // Not Handeled
        //    string pid = id.ToString().WithPrefix("Sensor");
        //    TraceIt("ITelldusEventListener:OnSensorEventReceived " + pid, WriteToFile);
        //}

        #endregion ITelldusEventListener Members

        #endregion Unused

        #region IDisposable

        // Track whether Dispose has been called.
        private bool disposed = false;

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;
            }
        }

        // Use interop to call the method necessary
        // to clean up the unmanaged resource.
        //[System.Runtime.InteropServices.DllImport("Kernel32")]

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Plugin()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion IDisposable
    }
}
