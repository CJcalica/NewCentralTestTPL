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
using System.Diagnostics;
using System.Windows.Forms;

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
            Sqlhandler.SetToEMMS1();
            List<LotInfo> lotInfo = new List<LotInfo>();
            try
            {
                var suffix = LotNo.Substring(LotNo.LastIndexOf('-'));

                String sqltext = "[usp_CentralTest_TPL_Get_Lot_Details]";//sp name
                Sqlhandler.CreateParameter(2);
                Sqlhandler.SetParameterValues(0, "@Lot", SqlDbType.NVarChar, LotNo);
                Sqlhandler.SetParameterValues(1, "@Machine", SqlDbType.NVarChar, CentralTest.MachineName);
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

        public bool insertMasterLogs(string Logs, string lot, string custcode, string destination, string machine, string IP)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            String sqltext = "[usp_CentralTest_TPL_Insert_Logs]";//sp name
            Sqlhandler.CreateParameter(7);
            Sqlhandler.SetParameterValues(0, "@Logs", SqlDbType.NVarChar, Logs);
            Sqlhandler.SetParameterValues(1, "@Lot", SqlDbType.NVarChar, lot);
            Sqlhandler.SetParameterValues(2, "@CustCode", SqlDbType.NVarChar, custcode);
            Sqlhandler.SetParameterValues(3, "@dest", SqlDbType.NVarChar, destination);
            Sqlhandler.SetParameterValues(4, "@Machine", SqlDbType.NVarChar, machine);
            Sqlhandler.SetParameterValues(5, "@IpAdd", SqlDbType.NVarChar, IP);
            Sqlhandler.SetParameterValues(6, "@Ucode", SqlDbType.NVarChar, User.Emp_No);
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.ExecuteNonQuery(sqltext, CommandType.StoredProcedure))
                {
                    result = true;
                }
            }
            return result;
        }

        public bool StartTLPLogs(string LotAlias,
                                 string TestProgram,
                                 string LBoard,
                                 string HB1,
                                 string HB2,
                                 string HB3,
                                 string HB4,
                                 string Cable1,
                                 string Cable2,
                                 string Cable3,
                                 string Cable4,
                                 string Carrier,
                                 string Cover,
                                 string Reel,
                                 string Machine,
                                 string IpAdd)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            String sqltext = "[usp_CentralTest_TPL_LaunchApp_Insert_Logs]";//sp name
            Sqlhandler.CreateParameter(28);
            Sqlhandler.SetParameterValues(0, "@LotCode", SqlDbType.BigInt, LotInfo.LotCode);
            Sqlhandler.SetParameterValues(1, "@LotAlias", SqlDbType.NVarChar, LotAlias);
            Sqlhandler.SetParameterValues(2, "@LotNumber", SqlDbType.NVarChar, LotInfo.LotNumber);
            Sqlhandler.SetParameterValues(3, "@CustomerCode", SqlDbType.BigInt, LotInfo.CustomerCode);
            Sqlhandler.SetParameterValues(4, "@PkgLD", SqlDbType.NVarChar, LotInfo.PkgLD);
            Sqlhandler.SetParameterValues(5, "@LdType", SqlDbType.NVarChar, LotInfo.LdType);
            Sqlhandler.SetParameterValues(6, "@ProductID", SqlDbType.NVarChar, LotInfo.ProductID);
            Sqlhandler.SetParameterValues(7, "@Device", SqlDbType.NVarChar, LotInfo.Device);
            Sqlhandler.SetParameterValues(8, "@ProductCode", SqlDbType.BigInt, LotInfo.ProductCode);
            Sqlhandler.SetParameterValues(9, "@SubLotQty", SqlDbType.BigInt, LotInfo.SubLotQty);
            Sqlhandler.SetParameterValues(10, "@RecipeCode", SqlDbType.BigInt, LotInfo.RecipeCode);
            Sqlhandler.SetParameterValues(11, "@StageID", SqlDbType.NVarChar, LotInfo.StageID);
            Sqlhandler.SetParameterValues(12, "@TestProgram", SqlDbType.NVarChar, TestProgram);
            Sqlhandler.SetParameterValues(13, "@LBoard", SqlDbType.NVarChar, LBoard);
            Sqlhandler.SetParameterValues(14, "@Hibs", SqlDbType.NVarChar, HB1);
            Sqlhandler.SetParameterValues(15, "@Hibs2", SqlDbType.NVarChar, HB2);
            Sqlhandler.SetParameterValues(16, "@Hibs3", SqlDbType.NVarChar, HB3);
            Sqlhandler.SetParameterValues(17, "@Hibs4", SqlDbType.NVarChar, HB4);
            Sqlhandler.SetParameterValues(18, "@Cable", SqlDbType.NVarChar, Cable1);
            Sqlhandler.SetParameterValues(19, "@Cable2", SqlDbType.NVarChar, Cable2);
            Sqlhandler.SetParameterValues(20, "@Cable3", SqlDbType.NVarChar, Cable3);
            Sqlhandler.SetParameterValues(21, "@Cable4", SqlDbType.NVarChar, Cable4);
            Sqlhandler.SetParameterValues(22, "@CarrierTape", SqlDbType.NVarChar, Carrier);
            Sqlhandler.SetParameterValues(23, "@CoverTape", SqlDbType.NVarChar, Cover);
            Sqlhandler.SetParameterValues(24, "@Reel", SqlDbType.NVarChar, Reel);
            Sqlhandler.SetParameterValues(25, "@UserCode", SqlDbType.NVarChar, User.Emp_No);
            Sqlhandler.SetParameterValues(26, "@Machine", SqlDbType.NVarChar, Machine);
            Sqlhandler.SetParameterValues(27, "@Ipaddress", SqlDbType.NVarChar, IpAdd);

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.ExecuteNonQuery(sqltext, CommandType.StoredProcedure))
                {
                    result = true;
                }
            }
            return result;
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
