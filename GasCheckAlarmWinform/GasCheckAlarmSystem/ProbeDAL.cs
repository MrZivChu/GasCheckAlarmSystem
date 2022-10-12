using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class ProbeDAL
{   
    public static bool EditRealtimeDataByID(int probeID, DateTime checkTime, float gasValue)
    {
        string sql = @"update Probe set CheckTime=@CheckTime,GasValue=@GasValue where ID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",probeID),
                 new SqlParameter("@CheckTime",checkTime),
                 new SqlParameter("@GasValue",gasValue),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool ResetGasValue()
    {
        string sql = @"update Probe set GasValue=0;";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }
}
