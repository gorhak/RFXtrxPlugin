using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class SQLiteFunctions
    {
        public SQLiteFunctions(string connectionstring)
        {
            _ConnectionString = connectionstring;
            //DeviceTable = GetParamsFromSQLIteTable("Devices");
            //DataSourceTable = GetParamsFromSQLIteTable("DataSources");
            //ScenarioTable = GetParamsFromSQLIteTable("Scenarios");
            //SystemModeTable = GetParamsFromSQLIteTable("ScheduleModes");
        }

        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
            }
        }

        protected string _ConnectionString;
        //protected Dictionary<string, Dictionary<string, object>> DeviceTable = new Dictionary<string, Dictionary<string, object>>();
        //protected Dictionary<string, Dictionary<string, object>> DataSourceTable = new Dictionary<string, Dictionary<string, object>>();
        //protected Dictionary<string, Dictionary<string, object>> ScenarioTable = new Dictionary<string, Dictionary<string, object>>();
        //protected Dictionary<string, Dictionary<string, object>> SystemModeTable = new Dictionary<string, Dictionary<string, object>>();


        public bool GetDeviceIDFromDeviceCode(string check, out string pid)
        {
            if (GetDataString("Devices", "DeviceID", "DeviceCode", check, out pid))
            {
                pid = pid.WithPrefix(Plugin.DevicePrefix);
                return true;
            }
            return false;
        }

        public bool GetDeviceCodeFromDeviceID(string check, out string str)
        {
            check = check.WithoutPrefix(Plugin.DevicePrefix);
            return GetDataString("Devices", "DeviceCode", "DeviceID", check, out str);
        }

        public bool GetDeviceDescriptionFromDeviceID(string check, out string str)
        {
            check = check.WithoutPrefix(Plugin.DevicePrefix);
            return GetDataString("Devices", "DeviceDescription", "DeviceID", check, out str);
        }

        public bool GetDeviceTypeIDFromDeviceID(string check, out string str)
        {
            check = check.WithoutPrefix(Plugin.DevicePrefix);
            return GetDataString("Devices", "DeviceTypeID", "DeviceID", check, out str);
        }

        public bool GetDataSourceIDFromDeviceCode(string check, out string pid)
        {
            check = "%" + check + "%";
            if (GetDataString("DataSources", "DataSourceID", "DataSourceParameters", check, out pid))
            {
                pid = pid.WithPrefix(Plugin.SourcePrefix);
                return true;
            }
            return false;
        }

      
        private bool GetDataString(string table, string searchtype, string selecttype, string select, out string result)
        {
            result = "";

            // Determin the ConnectionString
            string connectionString = dBFunctions.ConnectionStringSQLite;

            // Determin the DataAdapter = CommandText + Connection
            string commandText = @"SELECT " + searchtype + @" FROM (SELECT " + searchtype + @", " + selecttype + @" FROM " + table + @" WHERE " + selecttype + @" LIKE '" + select + @"')";

            // Make a new object
            dBHelper helper = new dBHelper(connectionString);

            // Load the data
            if (helper.Load(commandText, "") == true)
            {
                try
                {// Show the data in the datagridview
                    result = helper.DataSet.Tables[0].Rows[0].ItemArray[0].ToString();
                    return true;
                }
                catch
                { }
            }
            return false;
        }


        private DataTable GetDataTable(string commandtxt)
        {
            DataTable m_DT = new DataTable();

            // Determin the ConnectionString
            string connectionString = dBFunctions.ConnectionStringSQLite;

            // Determin the DataAdapter = CommandText + Connection

            // Make a new object
            dBHelper helper = new dBHelper(_ConnectionString);

            // Load the data
            if (helper.Load(commandtxt, "") == true)
            {
                // Show the data in the datagridview
                m_DT = helper.DataSet.Tables[0];
            }

            return m_DT;
        }


        private DataSet GetDataSet(string commandtxt)
        {
            DataSet m_DS = new DataSet();

            // Determin the ConnectionString
            string connectionString = dBFunctions.ConnectionStringSQLite;

            // Determin the DataAdapter = CommandText + Connection

            // Make a new object
            dBHelper helper = new dBHelper(_ConnectionString);

            // Load the data
            if (helper.Load(commandtxt, "") == true)
                {
                // Show the data in the datagridview
                m_DS = helper.DataSet;
            }

            return m_DS;
        }


        //private Dictionary<string, Dictionary<string, object>> GetParamsFromSQLIteTable(string table)
        //{
        //    DataTable DT = new DataTable();
        //    DataRow[] DR = new DataRow[1];
        //    DT = GetDataTable(@"SELECT * FROM " + table);
        //    Dictionary<string, Dictionary<string, object>> Params = new Dictionary<string, Dictionary<string, object>>();
        //    for (int i = 0; i <= DT.Rows.Count - 1; i++)
        //    {
        //        Dictionary<string, object> param = new Dictionary<string, object>();
        //        try
        //        {
        //            DR[0] = (DataRow)DT.Rows[i];
        //            string id = DR[0].ItemArray[0].ToString();

        //            for (int j = 0; j <= DT.Columns.Count - 2; j++)
        //            {
        //                string name = DT.Columns[j].ToString();
        //                string val = DR[0].ItemArray[j].ToString();
        //                param.Add(name, val);
        //            }
        //            {
        //                string val = DR[0].ItemArray[DT.Columns.Count - 1].ToString();
        //                if (val.Contains("::"))
        //                {
        //                    string[] split = val.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
        //                    foreach (var item in split)
        //                    {
        //                        string[] sp = item.Split(new string[] { "::" }, StringSplitOptions.None);
        //                        param.Add(sp[0], sp[1]);
        //                    }
        //                }
        //                else
        //                {
        //                    string name = DT.Columns[DT.Columns.Count - 1].ToString();
        //                    param.Add(name, val);
        //                }
        //            }
        //            if (Params.ContainsKey(id)) Params.Remove(id);
        //            Params.Add(id, param);
        //        }
        //        catch
        //        {
        //            var err = "Error";
        //        }
        //    }
        //    return Params;
        //}


        //private bool GetFromTable(Dictionary<string, Dictionary<string, object>> Table, string searchtag, string returntag, string check, out string ret)
        //{
        //    ret = "";
        //    foreach (Dictionary<string, object> tab in Table.Values)
        //    {
        //        foreach (var item in tab)
        //        {
        //            if (tab[searchtag].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                ret = tab[returntag].ToString();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private bool GetIDFromDeviceTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string ret)
        //{
        //    ret = "";
        //    foreach (Dictionary<string, object> tab in Table.Values)
        //    {
        //        foreach (var item in tab)
        //        {
        //            if (tab["DeviceCode"].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                ret = tab["DeviceID"].ToString();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private bool GetCodeFromDeviceTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string ret)
        //{
        //    ret = "";
        //    foreach (Dictionary<string, object> tab in Table.Values)
        //    {
        //        foreach (var item in tab)
        //        {
        //            if (tab["DeviceID"].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                ret = tab["DeviceCode"].ToString();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private bool GetTypeFromDeviceTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string ret)
        //{
        //    ret = "";
        //    foreach (Dictionary<string, object> tab in Table.Values)
        //    {
        //        foreach (var item in tab)
        //        {
        //            if (tab["DeviceID"].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                ret = tab["DeviceTypeID"].ToString();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private bool GetDescriptionFromDeviceTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string ret)
        //{
        //    ret = "";
        //    foreach (Dictionary<string, object> tab in Table.Values)
        //    {
        //        foreach (var item in tab)
        //        {
        //            if (tab["DeviceID"].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                ret = tab["DeviceDescription"].ToString();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private bool GetIDFromDataSourceTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string ret)
        //{
        //    ret = "";
        //    foreach (Dictionary<string, object> tab in Table.Values)
        //    {
        //        try
        //        {
        //            foreach (var item in tab)
        //            {
        //                if (tab["DeviceCode"].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    ret = tab["DataSourceID"].ToString();
        //                    return true;
        //                }
        //            }
        //        }
        //        catch { }
        //    }
        //    return false;
        //}


        //private bool GetIDFromScenarioTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string ret)
        //{
        //    ret = "";
        //    foreach (Dictionary<string, object> tab in Table.Values)
        //    {
        //        foreach (var item in tab)
        //        {
        //            if (tab[""].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                ret = tab[""].ToString();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private bool GetIDFromSystemModeTable(Dictionary<string, Dictionary<string, object>> Table, string check, out string ret)
        //{
        //    ret = "";
        //    foreach (Dictionary<string, object> tab in Table.Values)
        //    {
        //        foreach (var item in tab)
        //        {
        //            if (tab[""].ToString().Equals(check, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                ret = tab[""].ToString();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

    }
}
