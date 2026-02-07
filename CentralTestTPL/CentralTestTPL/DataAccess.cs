using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Data.SqlClient;

namespace CentralTestTPL
{
    public class DataAccess
    {
        AppSQLConfig Sqlhandler = new AppSQLConfig();

        public List<User> selectUser(string empNo)
        {
            Sqlhandler.SetToEMMS1();
            List<User> userDetails = new List<User>();
            String sqltext = "[usp_CentralTest_TPL_Select_Emp]";//sp name
            Sqlhandler.CreateParameter(1);
            Sqlhandler.SetParameterValues(0, "@Emp", SqlDbType.NVarChar, empNo);
            DataTable dt = new DataTable();

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                {

                }
                userDetails = ConvertDataTable<User>(dt);
            }
            Sqlhandler.CloseConnection();
            return userDetails;
        }

        public List<CentralTest> selectMachine(string machine)
        {
            Sqlhandler.SetToEMMS1();
            List<CentralTest> machDetails = new List<CentralTest>();
            String sqltext = "[usp_CentralTest_TPL_Select_Machine]";//sp name
            Sqlhandler.CreateParameter(1);
            Sqlhandler.SetParameterValues(0, "@Machine", SqlDbType.NVarChar, machine);

            DataTable dt = new DataTable();

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                {

                }
                machDetails = ConvertDataTable<CentralTest>(dt);
            }
            Sqlhandler.CloseConnection();
            return machDetails;
        }

        public List<LotInfo> SelectLotInfo(string LotNo)
        {
            Sqlhandler.SetToMesAtec();
            List<LotInfo> lotInfo = new List<LotInfo>();
            try
            {
                var suffix = LotNo.Substring(LotNo.LastIndexOf('-'));

                String sqltext = "[usp_WebMES_RecipeLoader_Get_Lot_Details]";//sp name
                Sqlhandler.CreateParameter(2);
                Sqlhandler.SetParameterValues(0, "@Lot", SqlDbType.NVarChar, LotNo);
                Sqlhandler.SetParameterValues(1, "@StageCode", SqlDbType.NVarChar, 23);


                DataTable dt = new DataTable();

                if (Sqlhandler.OpenConnection())
                {
                    if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                    {
                        lotInfo = ConvertDataTable<LotInfo>(dt);
                    }
                }
                Sqlhandler.CloseConnection();
            }
            catch (Exception ex)
            {

            }
            return lotInfo;
        }

        public List<Paths> selectPaths(long CustomerCode)
        {
            Sqlhandler.SetToEMMS1();
            List<Paths> pathInfo = new List<Paths>();
            String sqltext = "[TPL_SELECTPATH]";//sp name
            Sqlhandler.CreateParameter(1);
            Sqlhandler.SetParameterValues(0, "@CustomerCode", SqlDbType.BigInt, CustomerCode);

            DataTable dt = new DataTable();

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                {
     
                }
                pathInfo = ConvertDataTable<Paths>(dt);
            }
            Sqlhandler.CloseConnection();
            return pathInfo;
        }


        public bool selectToLogs(string fname)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            SqlDataReader dr = null;
            String sqltext = "[usp_TPL_IXYS_Select_Logs]";
            Sqlhandler.CreateParameter(1);
            Sqlhandler.SetParameterValues(0, "@FName", SqlDbType.NVarChar, fname);
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataReader(sqltext, ref dr, CommandType.StoredProcedure))
                {
                    if (dr.HasRows)
                    {
                        result = true;
                    }
                }
            }
            Sqlhandler.CloseConnection();
            return result;
        }

        public bool insertMasterLogs(string Lotnumber,
                                     string LotAlias,
                                     Int64 LotCode,
                                     string TestProg,
                                     Int64 custCode,
                                     Int64 recipeCode,
                                     string fileDest,
                                     string FileName,
                                     string stageID,
                                     string empNum)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            String sqltext = "[usp_TPL_Vishay_Insert_Logs]";//sp name
            Sqlhandler.CreateParameter(10);
            Sqlhandler.SetParameterValues(0, "@LotNumber", SqlDbType.NVarChar, Lotnumber);
            Sqlhandler.SetParameterValues(1, "@LotAlias", SqlDbType.NVarChar, LotAlias);
            Sqlhandler.SetParameterValues(2, "@lotCode", SqlDbType.BigInt, LotCode);
            Sqlhandler.SetParameterValues(3, "@testProg", SqlDbType.NVarChar, TestProg);
            Sqlhandler.SetParameterValues(4, "@custCode", SqlDbType.BigInt, custCode);
            Sqlhandler.SetParameterValues(5, "@recipeCode", SqlDbType.BigInt, recipeCode);
            Sqlhandler.SetParameterValues(6, "@fileDest", SqlDbType.NVarChar, fileDest);
            Sqlhandler.SetParameterValues(7, "@Filename", SqlDbType.NVarChar, FileName);
            Sqlhandler.SetParameterValues(8, "@stageID", SqlDbType.NVarChar, stageID);
            Sqlhandler.SetParameterValues(9, "@empNo", SqlDbType.NVarChar, empNum);



            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.ExecuteNonQuery(sqltext, CommandType.StoredProcedure))
                {
                    result = true;
                }
            }
            return result;
        }

        public bool insertErrorLogs(Int64 LotCode,
                                     string LotAlias,
                                     string Error)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            Sqlhandler.CreateParameter(5);
            Sqlhandler.SetParameterValues(0, "@Lotcode", SqlDbType.BigInt, LotCode);
            Sqlhandler.SetParameterValues(1, "@LotAlias", SqlDbType.NVarChar, LotAlias);
            Sqlhandler.SetParameterValues(2, "@ComputerName", SqlDbType.NVarChar, Environment.MachineName);
            Sqlhandler.SetParameterValues(3, "@Error", SqlDbType.NVarChar, Error);
            Sqlhandler.SetParameterValues(4, "@User", SqlDbType.NVarChar, User.Emp_No);



            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.ExecuteNonQuery("[usp_TPL_Insert_Error_Logs]", CommandType.StoredProcedure))
                {
                    result = true;
                }
            }
            return result;
        }


        public List<FTPInfo> selectFTPInfo(Int64 CustomerCode)
        {
            Sqlhandler.SetToEMMS1();
            List<FTPInfo> FTPInfos = new List<FTPInfo>();
            String sqltext = "[FTP_GetInfo]";//sp name
            Sqlhandler.CreateParameter(2);
            Sqlhandler.SetParameterValues(0, "@AppName", SqlDbType.NVarChar, "IXYS-TPL");
            Sqlhandler.SetParameterValues(1, "@CustomerCode", SqlDbType.BigInt, CustomerCode);

            DataTable dt = new DataTable();

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                {

                }
                FTPInfos = ConvertDataTable<FTPInfo>(dt);

            }
            Sqlhandler.CloseConnection();
            return FTPInfos;
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

    }
}
