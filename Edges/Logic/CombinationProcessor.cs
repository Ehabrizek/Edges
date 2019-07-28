using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Edges.Logic
{
    class CombinationProcessor
    {
        string _now = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

        #region Public Methods
        public void WriteCombinations(string fullPathToExcel, string sheetName, string geneHeader, string pathwayHeader, Action incrementProgressBar, Action<long> setProgressBarMax)
        {
            string conn = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fullPathToExcel};Extended Properties='Excel 12.0;HDR=yes'";

            DataTable pathwaysTable = GetDataTable($"SELECT [{pathwayHeader}] FROM [{sheetName}$];", conn);
            DataTable distinctPathwaysTable = GetDataTable($"SELECT distinct [{pathwayHeader}] FROM [{sheetName}$];", conn);
            DataTable geneIdsTable = GetDataTable($"SELECT [{geneHeader}] FROM [{sheetName}$];", conn);

            List<List<long>> listOLists = new List<List<long>>();

            List<string> pathways = convertToStringList(pathwaysTable.Rows);
            List<string> distinctPathways = convertToStringList(distinctPathwaysTable.Rows);
            List<long> geneIds = convertTolongList(geneIdsTable.Rows);

            setProgressBarMax(distinctPathways.Count);

            foreach (string distinctPathway in distinctPathways)
            {
                //Create 1 list per grouping of the same pathways.
                List<long> combination = new List<long>();
                for (int i = 0; i < geneIds.Count; i++)
                {
                    if (pathways[i] == distinctPathway)
                    {
                        combination.Add(geneIds[i]);
                    }
                }
                listOLists.Add(combination);
            }

            using (StreamWriter writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, $"Edges Processed on {_now}.txt"), true))
            {
                for (int i = 0; i < listOLists.Count; i++)
                {
                    long[] arr = listOLists[i].ToArray();
                    long r = 2;
                    long n = arr.Length;
                    prlongCombination(arr, n, r, distinctPathways[i], writer);
                    incrementProgressBar();
                }
            }
        }
        #endregion

        #region Private Methods
        void prlongCombination(long[] arr, long n, long r, string pathWayName, StreamWriter writer)
        {
            long[] data = new long[r];

            combinationUtilRecursive(arr, data, 0, n - 1, 0, r, pathWayName, writer);
        }

        void combinationUtilRecursive(long[] arr, long[] data, long start, long end, long index, long r, string pathWayName, StreamWriter writer)
        {
            if (index == r)
            {
                for (long j = 0; j < r; j++)
                {
                    writer.Write(data[j] + "," + " ");
                }
                writer.Write(pathWayName);
                writer.WriteLine("");
                return;
            }

            for (long i = start; i <= end && end - i + 1 >= r - index; i++)
            {
                data[index] = arr[i];
                combinationUtilRecursive(arr, data, i + 1, end, index + 1, r, pathWayName, writer);
            }
        }

        DataTable GetDataTable(string sql, string connectionString)
        {
            DataTable dt = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    using (OleDbDataReader rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                        return dt;
                    }
                }
            }
        }

        List<string> convertToStringList(DataRowCollection dataRows)
        {
            List<string> stringList = new List<string>();
            foreach (DataRow row in dataRows)
            {
                stringList.Add(row.ItemArray[0].ToString());
            }
            return stringList;
        }

        List<long> convertTolongList(DataRowCollection dataRows)
        {
            List<long> longList = new List<long>();
            foreach (DataRow row in dataRows)
            {
                longList.Add(Convert.ToInt64(row.ItemArray[0]));
            }
            return longList;
        }
        #endregion
    }
}
