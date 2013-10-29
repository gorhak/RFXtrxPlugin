using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;


namespace SwitchKing.Server.Plugins.RFXtrx
{
    class dBHelper
    {
        // Declartion internal variables
        private SQLiteConnection m_connection = null;
        private string m_connectionString = "";
        private SQLiteDataAdapter m_dataAdapter = null;
        private DataSet m_dataSet = null;
        private string m_fieldNameID = "";

        // The DataSet is filled with the methode LoadDataSet
        public DataSet DataSet
        {
            get { return m_dataSet; }
        }

        // Constructor -> ConnectionString is required
        public dBHelper(string connectionString)
        {
            m_connectionString = connectionString;
        }

        // Load the DataSet 
        public bool Load(string commandText, string fieldNameID)
        {
            // Save the variables
            m_fieldNameID = fieldNameID;

            try
            {
                // Open the connection
                m_connection = new SQLiteConnection(m_connectionString);
                m_connection.Open();

                // Make one DataAdapter
                m_dataAdapter = new SQLiteDataAdapter(commandText, m_connection);

                //// Koppel een eventhandler aan het RowUpdated-event van de DataAdapter
                //// m_dataAdapter.RowUpdated += new SqlRowUpdatedEventHandler(m_dataAdapter_RowUpdated);
                //m_dataAdapter.RowUpdated += m_dataAdapter_RowUpdated;
                m_dataSet = new DataSet();

                //// Voor eventueel opslaan --> Commands maken
                //if (!string.IsNullOrEmpty(fieldNameID))
                //{
                //    SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(m_dataAdapter);
                //    m_dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                //    m_dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                //    m_dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                //}

                // Fill the DataSet
                m_dataAdapter.Fill(m_dataSet);

                // We are here, alles okay!
                return true;
            }

            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Always Close
                m_connection.Close();
            }
        }

        // Load the DataSet 
        public bool Load(string commandText)
        {
            return Load(commandText, "");
        }

        //// Sla de DataSet op
        //public bool Save()
        //{
        //    // Hij kan alleen gegevens opslaan in ID bekend is
        //    if (m_fieldNameID.Trim().Length == 0)
        //    {
        //        return false;
        //    }

        //    try
        //    {
        //        // Open de connectie
        //        m_connection.Open();

        //        // Sla de DataRow op. Dit vuurt het event OnRowUpdated af
        //        m_dataAdapter.Update(m_dataSet);

        //        // We zijn hier, alles okay!
        //        return true;
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        // Altijd netjes sluiten
        //        m_connection.Close();
        //    }
        //}

        //// Rij is opgeslagen, bepaal eventueel de nieuwe ID
        //void m_dataAdapter_RowUpdated(object sender, System.Data.Common.RowUpdatedEventArgs e)
        //{
        //    // Het (zojuist verkregen?) ID is alleen interessant bij een nieuwe record

        //    if (e.StatementType == StatementType.Insert)
        //    {
        //        // Bepaal het zojuist verkregen ID
        //        // SQLiteCommand command = new SQLiteCommand("SELECT @@IDENTITY", m_connection);
        //        SQLiteCommand command = new SQLiteCommand("SELECT last_insert_rowid() AS ID", m_connection);
                

        //        // Bepaal de nieuwe ID en sla deze op in het juiste veld
        //        object nieuweID = command.ExecuteScalar();

        //        // Bij evt. fouten geen ID --> Daarom testen
        //        if (nieuweID == System.DBNull.Value == false)
        //        {
        //            // Zet de ID in de juiste kolom in de DataRow
        //            e.Row[m_fieldNameID] = Convert.ToInt32(nieuweID);
        //        }
        //    }
        //}

    }
}
