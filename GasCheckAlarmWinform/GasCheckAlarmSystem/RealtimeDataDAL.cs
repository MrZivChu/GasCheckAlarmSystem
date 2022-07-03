using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class RealtimeDataDAL
{   
    public static bool EditRealtimeDataByID(int probeID, DateTime checkTime, float gasValue)
    {
        string sql = @"update RealtimeData set CheckTime=@CheckTime,GasValue=@GasValue where ProbeID=@ProbeID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ProbeID",probeID),
                 new SqlParameter("@CheckTime",checkTime),
                 new SqlParameter("@GasValue",gasValue),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool ResetGasValue()
    {
        string sql = @"update RealtimeData set GasValue=0;";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }
}
