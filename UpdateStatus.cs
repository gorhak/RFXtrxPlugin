using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SwitchKing.Server.Plugins.RFXtrx;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class UpdateStatus
    {
        // State information used in the task.
        //private string url;
        //private string user = null;
        //private string pass = null;
        
        private dBHelper helper = null;
        DataSet DS = new DataSet();
        DataTable DT = new DataTable();
        DataRow[] DR = new DataRow[1];
        //string str = "";
        private Dictionary<string, string> IDToCodeList = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, object>> DeviceTable = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, Dictionary<string, object>> DataSourceTable = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, Dictionary<string, object>> ScenarioTable = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, Dictionary<string, object>> SystemModeTable = new Dictionary<string, Dictionary<string, object>>();

        // The constructor obtains the state information.
        public UpdateStatus()
        {
        }

        // The constructor obtains the state information.
        public UpdateStatus(string Url)
        {
            _url = Url;
        }

        // The constructor obtains the state information.
        public UpdateStatus(string User, string Pass)
        {
            _user = User;
            _pass = Pass;
        }

        // The constructor obtains the state information.
        public UpdateStatus(string Url, string User, string Pass)
        {
            _url = Url;
            _user = User;
            _pass = Pass;
        }

        public string url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }
        public string user
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }
        public string pass
        {
            get
            {
                return _pass;
            }
            set
            {
                _pass = value;
            }
        }

        protected string _url = "";
        protected string _user = null;
        protected string _pass = null;

        public void GetActualUnitStatus()//UpdateUnits()
        {
            //url = "http://" + Plugin.HostName + ":" + Plugin.HostPort + "/";
            //UpdateDevices(@url + "devices"); //XML
            UpdateCodeIDs();
            //UpdateCodeIDs(@url + "devices"); //XML
            //UpdatedataSources(@url + "datasources"); //XML
            //UpdatesCenarios(@url + "scenarios"); //XML
            //UpdatesystemModes(@url + "systemmodes"); //XML
        }


        #region UpdateCodeIDs()
        public void UpdateCodeIDs()
        {
            Dictionary<string, string> CodeIDs = new Dictionary<string, string>();

            try
            {
                //CodeIDs = GetSQLiteToDict("DeviceCode", "DeviceID", "Devices");
                //lock (Plugin.UnitCodeIDs)
                //{
                //    foreach (var item in CodeIDs)
                //    {
                //        if (Plugin.UnitCodeIDs.ContainsKey(item.Key))
                //        { Plugin.UnitCodeIDs.Remove(item.Key); }
                //        Plugin.UnitCodeIDs.Add(item.Key, CodeIDs[item.Key].WithPrefix(Plugin.DevicePrefix));
                //        //Trace(item.Key.ToString() + ": " + CodeIDs[item.Key].ToString(), Plugin.WriteToFile);
                //        //System.IO.File.AppendAllText(@"C:\MyDataSource\UnitCodeIDs.txt", item.Key.ToString() + ": " + CodeIDs[item.Key].ToString() + Environment.NewLine);
                //    }
                //}
            }
            catch
            { }
        }
        #endregion


        #region GetIDFromDeviceTable()
        private bool GetIDFromDeviceTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string id)
        {
            id = "";
            foreach (Dictionary<string, object> tab in Table.Values) // or foreach(book b in books.Values)
            {
                foreach (var item in tab)
                {
                    if (tab["DeviceCode"].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
                    {
                        id = tab["DeviceID"].ToString();
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion


        #region GetIDFromDataSourceTable()
        private bool GetIDFromDataSourceTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string id)
        {
            id = "";
            foreach (Dictionary<string, object> tab in Table.Values) // or foreach(book b in books.Values)
            {
                try
                {
                    foreach (var item in tab)
                    {
                        if (tab["DeviceCode"].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
                        {
                            id = tab["DataSourceID"].ToString();
                            return true;
                        }
                    }
                }
                catch { }
            }
            return false;
        }
        #endregion


        #region GetParamsFromSQLIteTable()
        private Dictionary<string, Dictionary<string, object>> GetParamsFromSQLIteTable(string table)
        {
            DataTable DT = new DataTable();
            DataRow[] DR = new DataRow[1];
            DT = GetDataTable(@"SELECT * FROM " + table);
            Dictionary<string, Dictionary<string, object>> Params = new Dictionary<string, Dictionary<string, object>>();
            for (int i = 0; i <= DT.Rows.Count - 1; i++)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                try
                {
                    DR[0] = (DataRow)DT.Rows[i];
                    string id = DR[0].ItemArray[0].ToString();

                    for (int j = 0; j <= DT.Columns.Count - 2; j++)
                    {
                        string name = DT.Columns[j].ToString();
                        string val = DR[0].ItemArray[j].ToString();
                        param.Add(name, val);
                    }
                    {
                        string val = DR[0].ItemArray[DT.Columns.Count - 1].ToString();
                        if (val.Contains("::"))
                        {
                            string[] split = val.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in split)
                            {
                                string[] sp = item.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                                param.Add(sp[0], sp[1]);
                            }
                        }
                        else
                        {
                            string name = DT.Columns[DT.Columns.Count - 1].ToString();
                            param.Add(name, val);
                        }
                    }
                    if (Params.ContainsKey(id)) Params.Remove(id);
                    Params.Add(id, param);
                }
                catch
                {
                    var err = "Error";
                }
            }
            return Params;
        }
        #endregion


        #region GetDataTable()
        private DataTable GetDataTable(string commandtxt)
        {
            DataTable m_DT = new DataTable();

            // Determin the ConnectionString
            string connectionString = dBFunctions.ConnectionStringSQLite;

            // Determin the DataAdapter = CommandText + Connection

            // Make a new object
            helper = new dBHelper(connectionString);

            // Load the data
            if (helper.Load(commandtxt, "") == true)
            {
                // Show the data in the datagridview
                m_DT = helper.DataSet.Tables[0];
            }

            return m_DT;
        }
        #endregion


        #region GetSQLiteToDict
        private Dictionary<string, string> GetSQLiteToDict(string key, string val, string table)
        {
            Dictionary<string, string> m_dict = new Dictionary<string, string>();
            DataRow[] m_DR = new DataRow[1];
            string m_key, m_val;

            DataTable m_DT = GetSQLiteDataTable(String.Format(@"SELECT {0}, {1} FROM {2}", key, val, table));
            for (int i = 0; i <= m_DT.Rows.Count - 1; i++)
            {
                try
                {
                    m_DR[0] = (DataRow)m_DT.Rows[i];
                    m_key = m_DR[0].ItemArray[0].ToString();
                    m_val = m_DR[0].ItemArray[1].ToString();
                    if (m_dict.ContainsKey(m_key)) m_dict.Remove(m_key);
                    m_dict.Add(m_key, m_val);
                }
                catch
                {
                    var msg = "Not found";
                }
            }
            return m_dict;
        }
        #endregion


        #region GetSQLiteDataTable
        private DataTable GetSQLiteDataTable(string commandtxt)
        {
            dBHelper helper = null;
            DataTable m_DT = new DataTable();

            // Determin the ConnectionString
            string connectionString = dBFunctions.ConnectionStringSQLite;

            // Determin the DataAdapter = CommandText + Connection

            // Make a new object
            helper = new dBHelper(connectionString);

            // Load the data
            if (helper.Load(commandtxt, "") == true)
            {
                // Show the data in the datagridview
                m_DT = helper.DataSet.Tables[0];
            }

            return m_DT;
        }
        #endregion


        #region FixNameSpaces
        public static string FixNameSpaces(string xmlString)
        {
            while (xmlString.Contains("xmlns"))
            {
                int i1 = 0;
                int i2 = 0;
                int i3 = 0;
                i1 = xmlString.IndexOf(" xmlns");
                string string1 = xmlString.Substring(i1, xmlString.Length - i1);
                i2 = string1.IndexOf("\"");
                string string2 = string1.Substring(i2 + 1, string1.Length - (i2 + 1));
                i3 = string2.IndexOf("\"");
                string string3 = string2.Substring(i3 + 1, string2.Length - (i3 + 1));
                xmlString = xmlString.Substring(0, i1) + string3;
            }

            while (xmlString.Contains(" i:nil"))
            {
                int i = 0;
                int j = 0;
                i = xmlString.IndexOf(" i:nil");
                string string1 = xmlString.Substring(0, i);
                while (string1.Contains("<"))
                {
                    j = string1.IndexOf("<");
                    string1 = string1.Substring(j + 1, string1.Length - (j + 1));
                }
                j = xmlString.Substring(i, xmlString.Length - i).IndexOf("<");
                string string2 = xmlString.Substring(i + j, xmlString.Length - (i + j));
                xmlString = xmlString.Substring(0, i) + ">" + "i:nil=\"true\"" + "</" + string1 + ">" + string2;
            }

            if (xmlString.Contains("&amp;#"))
            {
                xmlString = xmlString.Replace("&amp;#229;", "å");
                xmlString = xmlString.Replace("&amp;#228;", "ä");
                xmlString = xmlString.Replace("&amp;#246;", "ö");
                xmlString = xmlString.Replace("&amp;#197;", "Å");
                xmlString = xmlString.Replace("&amp;#196;", "Ä");
                xmlString = xmlString.Replace("&amp;#214;", "Ö");
            }
            return xmlString;
        }
        #endregion


        #region UpdateDevices
        //public void UpdateDevices(string url)
        //{
        //    Dictionary<string, string> Status = new Dictionary<string, string>();
        //    XmlDocument xmlDocument = new XmlDocument();
        //    XDocument xDoc;

        //    try
        //    {
        //        XmlUrlResolver xmlResolver = new XmlUrlResolver();
        //        System.Net.NetworkCredential myCred;
        //        myCred = new System.Net.NetworkCredential(user, pass);
        //        xmlResolver.Credentials = myCred;
        //        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        //        xmlReaderSettings.XmlResolver = xmlResolver;
                
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.Load(XmlReader.Create(url, xmlReaderSettings));
        //        xmlDoc.InnerXml = FixNameSpaces(xmlDoc.InnerXml);

        //        using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc))
        //        {
        //            // the reader must be in the Interactive state in order to
        //            // create a LINQ to XML tree from it.
        //            nodeReader.MoveToContent();

        //            xDoc = XDocument.Load(nodeReader);

        //            //<ArrayOfRESTDevice>
        //            //<RESTDevice>
        //            //<AutoSynchronizeAllowed>true</AutoSynchronizeAllowed>
        //            //<CurrentDimLevel>-1</CurrentDimLevel>
        //            //<CurrentStateID>1</CurrentStateID>
        //            //<Description i:nil="true"/>
        //            //<DeviceCode>111:1</DeviceCode>
        //            //<DisabledByServer>false</DisabledByServer>
        //            //<Enabled>true</Enabled>
        //            //<GroupID>-1</GroupID>
        //            //<GroupName i:nil="true"/>
        //            //<ID>19</ID>
        //            //<InSemiAutoMode>false</InSemiAutoMode>
        //            //<ManualTargetDimLevel>-1</ManualTargetDimLevel>
        //            //<ManualTargetStateID>1</ManualTargetStateID>
        //            //<ModeID>2</ModeID>
        //            //<ModeType>ScheduleDriven</ModeType>
        //            //<Name> D19=C1&amp;&amp;(D21||D36)</Name>
        //            //<NativeID>129</NativeID>
        //            //<OnW>0</OnW>
        //            //<SupportsAbsoluteDimLvl>false</SupportsAbsoluteDimLvl>
        //            //<TypeCategory>SelfLearningRelay</TypeCategory>
        //            //<TypeID>2</TypeID>
        //            //<TypeModel>selflearning-switch:nexa</TypeModel>
        //            //<TypeName>Self Learning On/Off</TypeName>
        //            //<TypeProtocol>Arctech</TypeProtocol>
        //            //</RESTDevice>

        //            {
        //                var devices = new XDocument(
        //                new XElement("Request",
        //                    from el in xDoc.Element("ArrayOfRESTDevice").Elements()
        //                    select el
        //                    )
        //                );
        //                foreach (var rootelem in devices.Element("Request").Elements())
        //                {                       // rootelem = <RESTDevice>...</RESTDevice>
        //                    var pid = "0";      // <ID>...</ID>
        //                    //var code = "0";     // <DeviceCode>...</DeviceCode>
        //                    var dim = "0";      // <CurrentDimLevel>...</CurrentDimLevel>
        //                    var turn = "0";     // <CurrentStateID>..</CurrentStateID>
        //                    foreach (var item in rootelem.Elements())
        //                    {                   // item = <ID>...</ID>
        //                        if (item.Name.ToString() == "ID") pid = item.Value.ToString().WithPrefix(Plugin.DevicePrefix);
        //                        if (item.Name.ToString() == "CurrentDimLevel") dim = item.Value.ToString();
        //                        if (item.Name.ToString() == "CurrentStateID") turn = item.Value.ToString();
        //                        //if (item.Name.ToString() == "DeviceCode") code = item.Value.ToString();
        //                    }
        //                    if (Plugin.unitList.Contains(pid))
        //                    {
        //                        if (dim == "-1" || dim == "0")
        //                        {
        //                            turn = turn.Replace("1", "TurnOff").Replace("2", "TurnOn");
        //                            Status.Add(pid, turn);
        //                        }
        //                        else { Status.Add(pid, dim); }
        //                    }
        //                }
        //            }

        //            lock (Plugin.UnitStatus)
        //            {
        //                foreach (var item in Status)
        //                {
        //                    if (Plugin.UnitStatus.ContainsKey(item.Key))
        //                    {
        //                        Plugin.UnitStatus.Remove(item.Key);
        //                        Plugin.UnitStatus.Add(item.Key, Status[item.Key]);
        //                        Trace(item.Key + ":" + Status[item.Key] + " UpdateUnitStatus", Plugin.WriteToFile);
        //                    }
        //                }
        //            }
                    
        //        }
        //    }
        //    catch
        //    { }

        //}
        #endregion


        #region UpdateCodeIDs(string url)
        //public void UpdateCodeIDs(string url)
        //{
        //    Dictionary<string, string> CodeIDs = new Dictionary<string, string>();

        //    #region REST
        //    XmlDocument xmlDocument = new XmlDocument();
        //    XDocument xDoc;

        //    try
        //    {
        //        XmlUrlResolver xmlResolver = new XmlUrlResolver();
        //        System.Net.NetworkCredential myCred;
        //        myCred = new System.Net.NetworkCredential(user, pass);
        //        xmlResolver.Credentials = myCred;
        //        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        //        xmlReaderSettings.XmlResolver = xmlResolver;

        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.Load(XmlReader.Create(url, xmlReaderSettings));
        //        xmlDoc.InnerXml = FixNameSpaces(xmlDoc.InnerXml);

        //        using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc))
        //        {
        //            // the reader must be in the Interactive state in order to
        //            // create a LINQ to XML tree from it.
        //            nodeReader.MoveToContent();

        //            xDoc = XDocument.Load(nodeReader);

        //            //<ArrayOfRESTDevice>
        //            //<RESTDevice>
        //            //<AutoSynchronizeAllowed>true</AutoSynchronizeAllowed>
        //            //<CurrentDimLevel>-1</CurrentDimLevel>
        //            //<CurrentStateID>1</CurrentStateID>
        //            //<Description i:nil="true"/>
        //            //<DeviceCode>111:1</DeviceCode>
        //            //<DisabledByServer>false</DisabledByServer>
        //            //<Enabled>true</Enabled>
        //            //<GroupID>-1</GroupID>
        //            //<GroupName i:nil="true"/>
        //            //<ID>19</ID>
        //            //<InSemiAutoMode>false</InSemiAutoMode>
        //            //<ManualTargetDimLevel>-1</ManualTargetDimLevel>
        //            //<ManualTargetStateID>1</ManualTargetStateID>
        //            //<ModeID>2</ModeID>
        //            //<ModeType>ScheduleDriven</ModeType>
        //            //<Name> D19=C1&amp;&amp;(D21||D36)</Name>
        //            //<NativeID>129</NativeID>
        //            //<OnW>0</OnW>
        //            //<SupportsAbsoluteDimLvl>false</SupportsAbsoluteDimLvl>
        //            //<TypeCategory>SelfLearningRelay</TypeCategory>
        //            //<TypeID>2</TypeID>
        //            //<TypeModel>selflearning-switch:nexa</TypeModel>
        //            //<TypeName>Self Learning On/Off</TypeName>
        //            //<TypeProtocol>Arctech</TypeProtocol>
        //            //</RESTDevice>

        //            {
        //                var devices = new XDocument(
        //                new XElement("Request",
        //                    from el in xDoc.Element("ArrayOfRESTDevice").Elements()
        //                    select el
        //                    )
        //                );
        //                foreach (var rootelem in devices.Element("Request").Elements())
        //                {                       // rootelem = <RESTDevice>...</RESTDevice>
        //                    var pid = "0";      // <ID>...</ID>
        //                    var code = "0";     // <DeviceCode>...</DeviceCode>
        //                    //var dim = "0";      // <CurrentDimLevel>...</CurrentDimLevel>
        //                    //var turn = "0";     // <CurrentStateID>..</CurrentStateID>
        //                    foreach (var item in rootelem.Elements())
        //                    {                   // item = <ID>...</ID>
        //                        if (item.Name.ToString() == "ID") pid = item.Value.ToString().WithPrefix(Plugin.DevicePrefix);
        //                        //if (item.Name.ToString() == "CurrentDimLevel") dim = item.Value.ToString();
        //                        //if (item.Name.ToString() == "CurrentStateID") turn = item.Value.ToString();
        //                        if (item.Name.ToString() == "DeviceCode") code = item.Value.ToString();
        //                    }
        //                    if (!CodeIDs.ContainsKey(code)) { CodeIDs.Add(code, pid); }
        //                    //if (Plugin.unitList.Contains(pid))
        //                    //{
        //                    //    if (dim == "-1" || dim == "0")
        //                    //    {
        //                    //        turn = turn.Replace("1", "TurnOff").Replace("2", "TurnOn");
        //                    //        Status.Add(pid, turn);
        //                    //    }
        //                    //    else { Status.Add(pid, dim); }
        //                    //}
        //                }
        //            }
        //            #endregion

        //            lock (Plugin.UnitCodeIDs)
        //            {
        //                foreach (var item in CodeIDs)
        //                {
        //                    if (Plugin.UnitCodeIDs.ContainsKey(item.Key))
        //                    { Plugin.UnitCodeIDs.Remove(item.Key); }
        //                    Plugin.UnitCodeIDs.Add(item.Key, CodeIDs[item.Key].WithoutPrefix(Plugin.DevicePrefix));
        //                    Trace(item.Key + ":" + CodeIDs[item.Key] + " UnitCodeIDs", Plugin.WriteToFile);
        //                }
        //            }

        //        }
        //    }
        //    catch
        //    { }

        //}
        #endregion


        #region UpdatedataSources
        //public void UpdatedataSources(string url)
        //{
        //    Dictionary<string, string> Status = new Dictionary<string, string>();
        //    XmlDocument xmlDocument = new XmlDocument();
        //    XDocument xDoc;

        //    try
        //    {
        //        XmlUrlResolver xmlResolver = new XmlUrlResolver();
        //        System.Net.NetworkCredential myCred;
        //        myCred = new System.Net.NetworkCredential(user, pass);
        //        xmlResolver.Credentials = myCred;
        //        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        //        xmlReaderSettings.XmlResolver = xmlResolver;
                
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.Load(XmlReader.Create(url, xmlReaderSettings));
        //        xmlDoc.InnerXml = FixNameSpaces(xmlDoc.InnerXml);

        //        using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc))
        //        {
        //            // the reader must be in the Interactive state in order to
        //            // create a LINQ to XML tree from it.
        //            nodeReader.MoveToContent();

        //            xDoc = XDocument.Load(nodeReader);

        //            //<ArrayOfRESTDataSource>
        //            //<RESTDataSource>
        //            //<Description i:nil="true"/>
        //            //<Enabled>true</Enabled>
        //            //<EngineeringUnit i:nil="true"/>
        //            //<EvalMinMax>false</EvalMinMax>
        //            //<GroupID>-1</GroupID>
        //            //<GroupName i:nil="true"/>
        //            //<ID>13</ID>
        //            //<LastRun>9999-12-31T23:59:59.9999999</LastRun>
        //            //<LastValue/>
        //            //<LastValueAdditionalData/>
        //            //<LastValueDebugInfo/>
        //            //<LastValueExpires>0001-01-01T00:00:00</LastValueExpires>
        //            //<LastValueIsFailureValue>false</LastValueIsFailureValue>
        //            //<LastValueLocalTimestamp>0001-01-01T00:00:00</LastValueLocalTimestamp>
        //            //<LastValueStatus>NotCollected</LastValueStatus>
        //            //<LastValueTimestamp>0001-01-01T00:00:00</LastValueTimestamp>
        //            //<Name>S13;DSEM 940:1</Name>
        //            //<NextRun>9999-12-31T23:59:59.9999999</NextRun>
        //            //<PollScheduleRate>PT0S</PollScheduleRate>
        //            //<PollScheduleType>WhenModified</PollScheduleType>
        //            //<PollScheduleValue>PT0S</PollScheduleValue>
        //            //<Status>Unknown</Status>
        //            //<UsedValue/>
        //            //<UsedValueAdditionalData/>
        //            //<UsedValueDebugInfo/>
        //            //<UsedValueExpires>0001-01-01T00:00:00</UsedValueExpires>
        //            //<UsedValueIsFailureValue>false</UsedValueIsFailureValue>
        //            //<UsedValueLocalTimestamp>0001-01-01T00:00:00</UsedValueLocalTimestamp>
        //            //<UsedValueStatus>NotCollected</UsedValueStatus>
        //            //<UsedValueTimestamp>0001-01-01T00:00:00</UsedValueTimestamp>
        //            //</RESTDataSource>
                    
        //            {
        //                var sources = new XDocument(
        //                new XElement("Request",
        //                    from el in xDoc.Element("ArrayOfRESTDataSource").Elements()
        //                    select el
        //                    )
        //                );
        //                foreach (var rootelem in sources.Element("Request").Elements())
        //                {                       // rootelem = <RESTDataSource>...</RESTDataSource>
        //                    var pid = "0";      // <ID>...</ID>
        //                    var lv = "0";       // <LastValue>...</LastValue>
        //                    foreach (var item in rootelem.Elements())
        //                    {                   // item = <ID>...</ID>
        //                        if (item.Name.ToString() == "ID") pid = item.Value.ToString().WithPrefix(Plugin.SourcePrefix);
        //                        if (item.Name.ToString() == "LastValue") lv = item.Value.ToString();
        //                    }
        //                    Status.Add(pid, lv);
        //                }
        //            }

        //            lock (Plugin.UnitStatus)
        //            {
        //                foreach (var item in Status)
        //                {
        //                    if (Plugin.UnitStatus.ContainsKey(item.Key))
        //                    {
        //                        Plugin.UnitStatus.Remove(item.Key);
        //                        Plugin.UnitStatus.Add(item.Key, Status[item.Key]);
        //                        Trace(item.Key + ":" + Status[item.Key] + " UpdateUnitStatus", Plugin.WriteToFile);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    { }
        //}
        #endregion


        #region UpdatesCenarios
        //public void UpdatesCenarios(string url)
        //{
        //    Dictionary<string, string> Status = new Dictionary<string, string>();
        //    XmlDocument xmlDocument = new XmlDocument();
        //    XDocument xDoc;

        //    try
        //    {
        //        XmlUrlResolver xmlResolver = new XmlUrlResolver();
        //        System.Net.NetworkCredential myCred;
        //        myCred = new System.Net.NetworkCredential(user, pass);
        //        xmlResolver.Credentials = myCred;
        //        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        //        xmlReaderSettings.XmlResolver = xmlResolver;
                
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.Load(XmlReader.Create(url, xmlReaderSettings));
        //        xmlDoc.InnerXml = FixNameSpaces(xmlDoc.InnerXml);

        //        using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc))
        //        {
        //            // the reader must be in the Interactive state in order to
        //            // create a LINQ to XML tree from it.
        //            nodeReader.MoveToContent();

        //            xDoc = XDocument.Load(nodeReader);

        //            //<ArrayOfRESTScenario>
        //            //<RESTScenario>
        //            //<Abbreviation>AllAuto</Abbreviation>
        //            //<Active>true</Active>
        //            //<Enabled>true</Enabled>
        //            //<ID>1</ID>
        //            //<Name>All by schedule</Name>
        //            //</RESTScenario>

        //            {
        //                var scenarios = new XDocument(
        //                    new XElement("Request",
        //                        from el in xDoc.Element("ArrayOfRESTScenario").Elements()
        //                        select el
        //                        )
        //                    );
        //                foreach (var rootelem in scenarios.Element("Request").Elements())
        //                {                   // rootelem = <RESTScenario>...</RESTScenario>
        //                    var pid = "0";   // <ID>...</ID>
        //                    var active = "0"; // <Active>...</Active>
        //                    foreach (var item in rootelem.Elements())
        //                    {               // item = <ID>...</ID>
        //                        if (item.Name.ToString() == "ID") pid = item.Value.ToString().WithPrefix(Plugin.sCenarioPrefix);
        //                        if (item.Name.ToString() == "Active") active = item.Value.ToString();
        //                    }
        //                    active = active.Replace("false", "TurnOff").Replace("true", "TurnOn");
        //                    Status.Add(pid, active);
        //                }
        //            }

        //            lock (Plugin.unitList)
        //            {
        //                foreach (var item in Status)
        //                {
        //                    if (!Plugin.unitList.Contains(item.Key)) { Plugin.unitList.Add(item.Key); }
        //                }
        //            }

        //            lock (Plugin.UnitStatus)
        //            {
        //                foreach (var item in Status)
        //                {
        //                    if (Plugin.UnitStatus.ContainsKey(item.Key)) { Plugin.UnitStatus.Remove(item.Key); }
        //                    Plugin.UnitStatus.Add(item.Key, Status[item.Key]);
        //                    Trace(item.Key + ":" + Status[item.Key] + " UpdateUnitStatus", Plugin.WriteToFile);
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    { }
        //}
        #endregion


        #region UpdatesystemModes
        //public void UpdatesystemModes(string url)
        //{
        //    Dictionary<string, string> Status = new Dictionary<string, string>();
        //    XmlDocument xmlDocument = new XmlDocument();
        //    XDocument xDoc;

        //    try
        //    {
        //        XmlUrlResolver xmlResolver = new XmlUrlResolver();
        //        System.Net.NetworkCredential myCred;
        //        myCred = new System.Net.NetworkCredential(user, pass);
        //        xmlResolver.Credentials = myCred;
        //        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        //        xmlReaderSettings.XmlResolver = xmlResolver;
                
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.Load(XmlReader.Create(url, xmlReaderSettings));
        //        xmlDoc.InnerXml = FixNameSpaces(xmlDoc.InnerXml);

        //        using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc))
        //        {
        //            // the reader must be in the Interactive state in order to
        //            // create a LINQ to XML tree from it.
        //            nodeReader.MoveToContent();

        //            xDoc = XDocument.Load(nodeReader);
                    
        //            //<ArrayOfRESTSystemMode>
        //            //<RESTSystemMode>
        //            //<Abbreviation>Default</Abbreviation>
        //            //<Active>true</Active>
        //            //<Enabled>true</Enabled>
        //            //<ID>1</ID>
        //            //<Name>Default</Name>
        //            //</RESTSystemMode>
                    
        //            {
        //                var scenarios = new XDocument(
        //                    new XElement("Request",
        //                        from el in xDoc.Element("ArrayOfRESTSystemMode").Elements()
        //                        select el
        //                        )
        //                    );
        //                foreach (var rootelem in scenarios.Element("Request").Elements())
        //                {                   // rootelem = <RESTScenario>...</RESTScenario>
        //                    var pid = "0";   // <ID>...</ID>
        //                    var active = "0"; // <Active>...</Active>
        //                    foreach (var item in rootelem.Elements())
        //                    {               // item = <ID>...</ID>
        //                        if (item.Name.ToString() == "ID") pid = item.Value.ToString().WithPrefix(Plugin.ModePrefix);
        //                        if (item.Name.ToString() == "Active") active = item.Value.ToString();
        //                    }
        //                    active = active.Replace("false", "TurnOff").Replace("true", "TurnOn");
        //                    Status.Add(pid, active);
        //                }
        //            }

        //            lock (Plugin.unitList)
        //            {
        //                foreach (var item in Status)
        //                {
        //                    if (!Plugin.unitList.Contains(item.Key)) { Plugin.unitList.Add(item.Key); }
        //                }
        //            }

        //            lock (Plugin.UnitStatus)
        //            {
        //                foreach (var item in Status)
        //                {
        //                    if (Plugin.UnitStatus.ContainsKey(item.Key)) { Plugin.UnitStatus.Remove(item.Key); }
        //                    Plugin.UnitStatus.Add(item.Key, Status[item.Key]);
        //                    Trace(item.Key + ":" + Status[item.Key] + " UpdateUnitStatus", Plugin.WriteToFile);
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    { }
        //}
        #endregion


        #region SendString
        //public void SendString()
        //{
        //    try
        //    {
        //        //Initialisation, we use localhost, change if appliable
        //        HttpWebRequest request = GetWebRequest(url, user, pass);
        //        //Our method is post, otherwise the buffer (postvars) would be useless
        //        request.Method = "GET";
        //        //Get the response handle, we have no true response yet!

        //        RequestState rstate = new RequestState();
        //        rstate.request = request;
                
        //        request.BeginGetResponse(new AsyncCallback(RespCallback),rstate);
        //    }
        //    catch //(NullReferenceException ex)
        //    {
        //    }

        //}
        #endregion

        #region GetWebRequest
        //private HttpWebRequest GetWebRequest(string url, string user, string pass)
        //{
        //    CookieContainer myContainer = new CookieContainer();
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    req.Credentials = new NetworkCredential(user, pass);
        //    req.CookieContainer = myContainer;
        //    req.PreAuthenticate = true;

        //    return req;
        //}
        #endregion

        #region Post
        //private string Post(string url, object data)
        //{
        //    string vystup = null;

        //    try
        //    {
        //        MemoryStream stream = new MemoryStream();
        //        DataContractSerializer serializer = new DataContractSerializer(data.GetType());
        //        serializer.WriteObject(stream, data);
        //        stream.Position = 0;

        //        //Our postvars
        //        byte[] buffer = stream.GetBuffer();
        //        //Initialisation, we use localhost, change if appliable
        //        HttpWebRequest request = GetWebRequest(url, user, pass);
        //        //Our method is post, otherwise the buffer (postvars) would be useless
        //        request.Method = "POST";
        //        //We use form contentType, for the postvars.
        //        request.ContentType = "application/x-www-form-urlencoded";
        //        //The length of the buffer (postvars) is used as contentlength.
        //        request.ContentLength = buffer.Length;
        //        //We open a stream for writing the postvars
        //        Stream PostData = request.GetRequestStream();
        //        //Now we write, and afterwards, we close. Closing is always important!
        //        PostData.Write(buffer, 0, buffer.Length);
        //        PostData.Close();
        //        //Get the response handle, we have no true response yet!
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        //Now, we read the response (the string), and output it.
        //        Stream Answer = response.GetResponseStream();
        //        StreamReader _Answer = new StreamReader(Answer);
        //        vystup = _Answer.ReadToEnd();
        //        stream.Close();
        //        stream.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }

        //    return vystup;
        //}
        #endregion

        #region Get
        //private byte[] Get(string url)
        //{
        //    byte[] vystup = null;

        //    //Initialisation, we use localhost, change if appliable
        //    HttpWebRequest request = GetWebRequest(url, user, pass);
        //    //Our method is post, otherwise the buffer (postvars) would be useless
        //    request.Method = "GET";
        //    //Get the response handle, we have no true response yet!
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    // Get the stream associated with the response.
        //    Stream receiveStream = response.GetResponseStream();

        //    // Pipes the stream to a higher level stream reader with the required encoding format.
        //    StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);

        //    var data = readStream.ReadToEnd();

        //    vystup = System.Text.Encoding.UTF8.GetBytes(data);

        //    response.Close();
        //    readStream.Close();

        //    return vystup;
        //    //return data;
        //}
        #endregion

        #region SendString
        //public string SendString(string url)
        //{
        //    //Initialisation, we use localhost, change if appliable
        //    HttpWebRequest request = GetWebRequest(url, user, pass);
        //    //Our method is post, otherwise the buffer (postvars) would be useless
        //    request.Method = "GET";
        //    //Get the response handle, we have no true response yet!
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    // Get the stream associated with the response.
        //    Stream receiveStream = response.GetResponseStream();

        //    // Pipes the stream to a higher level stream reader with the required encoding format.
        //    StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);

        //    var data = readStream.ReadToEnd();

        //    response.Close();
        //    readStream.Close();

        //    return data;
        //}
        #endregion

        #region GetWebRequest
        //private HttpWebRequest GetWebRequest(string url)
        //{
        //    CookieContainer myContainer = new CookieContainer();
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    req.Credentials = new NetworkCredential(user, pass);
        //    //req.Credentials = new NetworkCredential("user", "pass");
        //    req.CookieContainer = myContainer;
        //    req.PreAuthenticate = true;

        //    return req;
        //}
        #endregion

        /// <summary>
        /// Get called by WebRequest when response is available
        /// </summary>
        /// <param name="asynchronousResult"></param>
        //private static void RespCallback(IAsyncResult asynchronousResult)
        //{
        //    Dictionary<string, string> Status = new Dictionary<string, string>();

        //    RequestState rstate = (RequestState)asynchronousResult;

        //    WebResponse response = rstate.request.EndGetResponse(asynchronousResult);

        //    // Get the stream associated with the response.
        //    Stream receiveStream = response.GetResponseStream();

        //    // Pipes the stream to a higher level stream reader with the required encoding format.
        //    StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);

        //    XmlDocument xmlDoc = new XmlDocument();

        //    xmlDoc.InnerText = readStream.ReadToEnd();
        //    // create a reader and move to the content
        //    using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc))
        //    {
        //        // the reader must be in the Interactive state in order to
        //        // create a LINQ to XML tree from it.
        //        nodeReader.MoveToContent();

        //        try
        //        {
        //            XDocument getAllDevices = XDocument.Load(nodeReader);

        //            var dev = new XDocument(
        //                new XElement("Request",
        //                    from el in getAllDevices.Element("ArrayOfRESTDevice").Elements()
        //                    select el
        //                    )
        //                );
        //            foreach (var rootelem in dev.Element("Request").Elements())
        //            {                   // rootelem = <RESTDevice>...</RESTDevice>
        //                var pid = "0";  // <ID>...</ID>
        //                var dim = "0";  // <CurrentDimLevel>...</CurrentDimLevel>
        //                var turn = "0"; // <CurrentStateID>...</CurrentStateID>
        //                foreach (var item in rootelem.Elements())
        //                {               // item = <ID>...</ID>
        //                    if (item.Name.ToString() == "ID") pid = item.Value.ToString().WithPrefix(Plugin.DevicePrefix);
        //                    if (item.Name.ToString() == "CurrentDimLevel") dim = item.Value.ToString();
        //                    if (item.Name.ToString() == "CurrentStateID") turn = item.Value.ToString();
        //                }
        //                if (Plugin.unitList.Contains(pid))
        //                {
        //                    if (dim == "-1" || dim == "0")
        //                    {
        //                        turn = turn.Replace("1", "TurnOff").Replace("2", "TurnOn");
        //                        Status.Add(pid, turn);
        //                    }
        //                    else { Status.Add(pid, dim); }
        //                }
        //            }

        //            lock (Plugin.UnitStatus)
        //            {
        //                foreach (var item in Plugin.UnitStatus)
        //                {
        //                    if (Status.ContainsKey(item.Key))
        //                    {
        //                        Plugin.UnitStatus.Remove(item.Key);
        //                        Plugin.UnitStatus.Add(item.Key, Status[item.Key]);
        //                    }

        //                }
        //            }

        //        }
        //        catch //(InvalidOperationException ex)
        //        {
        //        }

        //    }

        //    response.Close();
        //    readStream.Close();
        //}

    }

    //public class RequestState
    //{
    //    // This class stores the state of the request.
    //    const int BUFFER_SIZE = 1024;
    //    public StringBuilder requestData;
    //    public byte[] bufferRead;
    //    public WebRequest request;
    //    public WebResponse response;
    //    public Stream responseStream;
    //    public RequestState()
    //    {
    //        bufferRead = new byte[BUFFER_SIZE];
    //        requestData = new StringBuilder("");
    //        request = null;
    //        responseStream = null;
    //    }
    //}

}