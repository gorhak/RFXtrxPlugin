using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;


namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class dBFunctions
    {
        public dBFunctions()
        {
        }

        //public dBFunctions(string connectionstring)
        //{
        //    _ConnectionString = connectionstring;
        //}

        private static string ConnectionStringSQLite
        {
            get
            {
                string database =
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("DB", "switchKing.server.db3"));
                    //AppDomain.CurrentDomain.BaseDirectory + @"DB\switchKing.server.db3";
                string connectionString =
                    @"Data Source=" + Path.GetFullPath(database);
                return connectionString;
            }
        }

        //public string ConnectionString
        //{
        //    get
        //    {
        //        return _ConnectionString;
        //    }
        //    set
        //    {
        //        _ConnectionString = value;
        //    }
        //}

        //protected string _ConnectionString;


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

        public bool GetDataSourceValueMappingsFromDataSourceID(string check, out string str)
        {
            string res;
            check = "%" + check + "%";
            if (GetDataString("DataSources", "DataSourceID", "DataSourceValueMappings", check, out res))
            {
                if ((res != null) && (res.Contains("::")) && (res.Contains(check)))
                {
                    string[] split = res.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in split)
                    {
                        if (split.Contains(check))
                        {
                            str = item;
                            return true;
                        }
                    }
                }
            }
            str = "";
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

    }
}
