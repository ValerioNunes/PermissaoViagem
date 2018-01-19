using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace PermissaoViagem.Extension
{
    public class Utility
    {
        internal static DataTable ConvertXLSXtoDataTable(string connString, string nomeABa)
        {
            DataTable dt = new DataTable();
            OleDbConnection connection = new OleDbConnection(connString);

            try
            {
                DataSet ds = new DataSet();
                connection.Open();
                OleDbCommand command = new OleDbCommand("SELECT * FROM [" + nomeABa + "]", connection);
                OleDbDataAdapter oleda = new OleDbDataAdapter(command);
                oleda.Fill(ds);
                dt = ds.Tables[0];

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                connection.Close();
            }
            return dt;
        }

        internal static String Details(Exception e)
        {
            string detail = "";

            while (e != null)
            {
                e = e.InnerException;
                detail = (e != null) ? e.Message : detail;
            }

            return detail;
        }

    }
}