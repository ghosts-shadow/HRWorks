using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.OleDb;

namespace HRworks.Models
{
    public static class Utility
    {
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    if (rows.Length > 1)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i].Trim();
                        }

                        dt.Rows.Add(dr);
                    }
                }

            }


            return dt;
        }

        public static DataTable ConvertXSLXtoDataTable(string strFilePath, string connString)
        {
            using (OleDbConnection oledbConn = new OleDbConnection(connString))
            {
                DataTable dt = new DataTable();

                try
                {
                    oledbConn.Open();

                    // ✅ Get First Sheet Name Automatically
                    DataTable schema = oledbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (schema == null || schema.Rows.Count == 0)
                        return dt;

                    string sheetName = schema.Rows[0]["TABLE_NAME"].ToString();

                    using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheetName + "]", oledbConn))
                    {
                        OleDbDataAdapter oleda = new OleDbDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        oleda.Fill(ds);
                        dt = ds.Tables[0];
                    }
                }
                catch
                {
                }

                return dt;
            }
        }

    }
}