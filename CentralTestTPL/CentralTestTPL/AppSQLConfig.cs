using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.Data;

public class AppSQLConfig
{
    private string errorMessage = "";
    private string server, database, userName, passWord;
    private SqlConnection sqlConnection = new SqlConnection();
    private SqlParameter[] cmdParameter;
    private SqlTransaction sqlTrans;

    private List<string> cmdOutputParams;
    private List<string> cmdOutputValues;


    #region DatabaseConnections;

    public AppSQLConfig()
    {
        server = @"192.168.101.74";
        database = "WEBMES_CONN";
        userName = "sa";
        passWord = "dnhk0723$%";
    }

    public void SetToMesAtec()
    {
        server = @"MSDYNAMICS-DB\AXDB";
        database = "MES_ATEC";
        userName = "sa";
        passWord = "p@ssw0rd";
    }

    public void SetToEMMS1()
    {
        server = @"MSDYNAMICS-DB\AXDB";
        database = "EMMS1";
        userName = "sa";
        passWord = "p@ssw0rd";
    }

    public void SetToMachine() {
        server = @"192.168.8.12";
        database = "machines";
        userName = "administrator";
        passWord = "Dnhk$%07232022";
    }

    public void SetToHRIS()
    {
        server = @"ISPL-DB1";
        database = "HRIS";
        userName = "sa";
        passWord = "enola845&*";
    }

    public void SetTOBKM()
    {
        server = @"192.168.101.74";
        database = "ATEC_BKM";
        userName = "sa";
        passWord = "dnhk0723$%";
    }

    public void SetToCentralAcess()
    {
        server = @"MSDYNAMICS-DB\AXDB";
        database = "CentralAccess";
        userName = "sa";
        passWord = "p@ssw0rd";
    }

    //public void SetToCentralAcess()
    //{
    //    server = @"192.168.101.74";
    //    database = "CentralAccess";
    //    userName = "sa";
    //    passWord = "dnhk0723$%";
    //}


    public void SetToAXTestDB()
    {
        server = @"MSDYNAMICS-DB\AXDB";
        database = "TestDB";
        userName = "sa";
        passWord = "p@ssw0rd";
    }
    public void SetToAXCONN()
    {
        server = @"192.168.101.14";
        database = "ATECAX2012R3";
        userName = "sa";
        passWord = "dnhk0723$%";
    }
    public void SetToMESSupport()
    {
        server = @"MSDYNAMICS-DB\AXDB";
        database = "MES_ATEC_Support";
        userName = "sa";
        passWord = "p@ssw0rd";
    }

    public void SetToAXDB()
    {
        server = @"MSDYNAMICS-DB\AXDB";
        database = "AX2009DB";
        userName = "sa";
        passWord = "p@ssw0rd";
    }

    public void SetToNAVISION()
    {
        server = @"NAVISION-SERVER";
        database = "SmartTrack 3.25";
        userName = "sa";
        passWord = "25hpw2k30304$";
    }

    public void SetToReplication()
    {
        server = @"ATEC_REP_P1_A";
        database = "MES_ATEC";
        userName = "sa";
        passWord = "dnhk0723$%";
    }
    #endregion

    #region SQLConnections;
    /// <summary>
    /// Open SQL connection
    /// </summary>
    /// <returns></returns>
    public Boolean OpenConnection()
    {
        try
        {
            String connection = "";
            int connectionTimeOut = 99999;

            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }

            if (userName == "" || userName == null)
            {
                connection = "server=" + server + "; database=" + database + "; connection timeout=" + connectionTimeOut + "; Trusted_Connection=Yes;";
            }
            else
            {
                connection = "server=" + server + "; database=" + database + "; user id=" + userName + "; password=" + passWord + "; connection timeout=" + connectionTimeOut;
            }
            sqlConnection.ConnectionString = connection;
            sqlConnection.Open();
            sqlTrans = sqlConnection.BeginTransaction();

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = "Open Connection: " + ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Close current SQL connection
    /// </summary>
    /// <returns></returns>
    public Boolean CloseConnection()
    {
        try
        {
            sqlConnection.Close();
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = "Close connection: " + ex.Message;
            return false;
        }
    }

    #endregion

    #region Parameters;

    /// <summary>
    /// Return SQL Config error messages
    /// </summary>
    /// <returns></returns>
    public string GetErrorMessage()
    {
        return "Error on " + errorMessage;
    }


    /// <summary>
    /// Attach Parameter Value
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    private Boolean AttachParameter(SqlCommand cmd)
    {
        try
        {
            if (cmdParameter != null && cmdParameter.Length > 0)
            {
                foreach (object p in cmdParameter)
                {
                    cmd.Parameters.Add(p);
                }
            }
            else
            {
                errorMessage = "Attach Parameter: Invalid size of parameters";
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = "Attach Parameter: " + ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Set ouput parameter to SQL config
    /// </summary>
    /// <param name="Cmd"></param>
    private void SetOutputParamValues(SqlCommand Cmd)
    {
        if (cmdOutputParams != null)
        {
            foreach (string output in cmdOutputParams)
            {
                cmdOutputValues.Add(Cmd.Parameters[output].Value.ToString());
            }
        }
    }


    /// <summary>
    /// Create parameter length for SQL 
    /// </summary>
    /// <param name="Size"></param>
    /// <param name="_Redim"></param>
    /// <returns></returns>
    public Boolean CreateParameter(int Size, Boolean _Redim = false)
    {
        try
        {
            if (Size == 0)
            {
                errorMessage = "Create Parameter: Invalid size of parameters";
                return false;
            }
            cmdOutputParams = new List<string>();
            cmdOutputValues = new List<string>();
            cmdParameter = new SqlParameter[Size - 1 + 1];
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = "Create Parameter: " + ex.Message;
            return false;
        }
    }


    /// <summary>
    /// Set Parameter value for SQL
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="ParamName"></param>
    /// <param name="Type"></param>
    /// <param name="Value"></param>
    /// <param name="Direction"></param>
    /// <returns></returns>
    public Boolean SetParameterValues(int Position, String ParamName, SqlDbType Type, Object Value, ParameterDirection Direction = ParameterDirection.Input)
    {
        try
        {
            if (cmdParameter == null)
            {
                errorMessage = "Set Parameter Values: Invalid size of parameters";
                return false;
            }
            cmdParameter[Position] = new SqlParameter(ParamName, Type);
            cmdParameter[Position].Direction = Direction;
            if (Direction == ParameterDirection.Output)
            {
                cmdOutputParams.Add(ParamName);
                if (Type == SqlDbType.NVarChar || Type == SqlDbType.VarChar)
                {
                    cmdParameter[Position].Size = 4000;
                }
            }
            else
            {
                cmdParameter[Position].Value = Value;
            }
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = "Set Parameter Values: " + ex.Message;
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ParamName"></param>
    /// <returns></returns>
    public string GetOutputParamValue(String ParamName)
    {
        for (int i = 0; i <= cmdOutputParams.Count - 1; i++)
        {
            if (cmdOutputParams[i] == ParamName)
            {
                return cmdOutputValues[i];
            }
        }
        return "";
    }
    #endregion

    #region SQLCommands      
    /// <summary>
    /// Fill Data Reader with SQL values
    /// </summary>
    /// <param name="SqlString"></param>
    /// <param name="dr"></param>
    /// <param name="CommandType"></param>
    /// <returns></returns>
    public Boolean FillDataReader(String SqlString, ref SqlDataReader dr, CommandType CommandType)
    {
        SqlCommand command = new SqlCommand(SqlString, sqlConnection);
        command.Transaction = sqlTrans;
        command.CommandTimeout = 99999;
        command.CommandType = CommandType;
        if (cmdParameter != null)
        {
            AttachParameter(command);
        }
        sqlTrans.Commit();
        dr = command.ExecuteReader(CommandBehavior.CloseConnection);
        if (cmdParameter != null)
        {
            SetOutputParamValues(command);
        }
        command = null;
        return true;


    }
    /// <summary>
    /// Fill Datatable with SQL values
    /// </summary>
    /// <param name="SqlString"></param>
    /// <param name="dt"></param>
    /// <param name="CommandType"></param>
    /// <returns></returns>
    public Boolean FillDataTable(String SqlString, ref DataTable dt, CommandType CommandType)
    {
        //try
        //{
        SqlCommand command = new SqlCommand(SqlString, sqlConnection);
        if (OpenConnection())
        {
            SqlDataAdapter da = null;
            command.Transaction = sqlTrans;
            command.CommandTimeout = 999999;
            command.CommandType = CommandType;

            if (cmdParameter != null)
            {
                AttachParameter(command);
            }
            da = new SqlDataAdapter(command);
            sqlTrans.Commit();
            da.Fill(dt);

            command = null;
            da = null;

            CloseConnection();
            return true;
        }
        else
        {
            CloseConnection();
            return false;
        }

        //}
        //catch(Exception ex)
        //{
        //    CloseConnection();
        //    errorMessage = "Fill DataSet: " + ex.Message;
        //    return false;
        //}
    }

    /// <summary>
    /// Fill Dataset with SQL values
    /// </summary>
    /// <param name="SqlString"></param>
    /// <param name="ds"></param>
    /// <param name="CommandType"></param>
    /// <returns></returns>
    public Boolean FillDataSet(String SqlString, ref DataSet ds, CommandType CommandType)
    {
        //try
        //{
        SqlCommand command = new SqlCommand(SqlString, sqlConnection);
        if (OpenConnection())
        {
            SqlDataAdapter da = null;
            command.Transaction = sqlTrans;
            command.CommandTimeout = 999999;
            command.CommandType = CommandType;

            if (cmdParameter != null)
            {
                AttachParameter(command);
            }
            da = new SqlDataAdapter(command);
            sqlTrans.Commit();
            da.Fill(ds);

            command = null;
            da = null;

            CloseConnection();
            return true;
        }
        else
        {
            CloseConnection();
            return false;
        }

        //}
        //catch(Exception ex)
        //{
        //    CloseConnection();
        //    errorMessage = "Fill DataSet: " + ex.Message;
        //    return false;
        //}
    }


    /// <summary>
    /// Execute SQL transaction
    /// </summary>
    /// <param name="SqlString"></param>
    /// <param name="CommandType"></param>
    /// <returns></returns>
    public Boolean ExecuteNonQuery(String SqlString, CommandType CommandType)
    {
        //try
        //{
        SqlCommand command = new SqlCommand(SqlString, sqlConnection);
        if (OpenConnection())
        {
            command.Transaction = sqlTrans;
            command.CommandTimeout = 999999;
            command.CommandType = CommandType;

            if (cmdParameter != null)
            {
                AttachParameter(command);
            }
            command.ExecuteNonQuery();
            sqlTrans.Commit();
            CloseConnection();
            if (cmdParameter != null)
            {
                SetOutputParamValues(command);
            }
            command = null;
            return true;
        }
        else
        {
            CloseConnection();
            return false;
        }

        //}
        //catch(Exception ex)
        //{
        //    sqlTrans.Rollback();
        //    CloseConnection();
        //    errorMessage = "Execute Non Query: " + ex.Message;
        //    return false;
        //}
    }
    #endregion
}