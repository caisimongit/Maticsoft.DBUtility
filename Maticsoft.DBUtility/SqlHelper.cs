using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maticsoft.DBUtility
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SqlHelper
    {

        private static Hashtable _parmCache = Hashtable.Synchronized(new Hashtable());

        protected SqlHelper()
        {

        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="cacheKey">key</param>
        /// <param name="commandParameters">参数</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            _parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 对连接执行SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="commandParameters">参数</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int num = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return num;
            }
        }

        /// <summary>
        ///  对连接执行SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="trans">处理事务的对象</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="commandParameters">参数</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                int num = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return num;
            }
        }

        /// <summary>
        ///  对连接执行SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="commandParameters">参数</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int num = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return num;
            }
        }

        /// <summary>
        /// 数据读取器
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="commandParameters">参数</param>
        /// <returns>数据读取器</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlDataReader reader2;
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                reader2 = reader;
            }
            catch
            {
                conn.Close();
                throw;
            }
            return reader2;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="commandParameters">参数</param>
        /// <returns>第一行的第一列的值</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object obj2 = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj2;
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="commandParameters">参数</param>
        /// <returns>第一行的第一列的值</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //准备命令
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);

                object obj2 = cmd.ExecuteScalar();

                cmd.Parameters.Clear();

                return obj2;
            }
        }

        /// <summary>
        /// 获取缓存的参数
        /// </summary>
        /// <param name="cacheKey">key</param>
        /// <returns>参数对象</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] parameterArray = (SqlParameter[])_parmCache[cacheKey];
            if (parameterArray == null)
            {
                return null;
            }
            SqlParameter[] parameterArray2 = new SqlParameter[parameterArray.Length];
            int index = 0;
            int length = parameterArray.Length;
            while (index < length)
            {
                parameterArray2[index] = (SqlParameter)((ICloneable)parameterArray[index]).Clone();
                index++;
            }
            return parameterArray2;
        }

        /// <summary>
        /// 装备命令
        /// </summary>
        /// <param name="cmd">执行命令对象</param>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="trans">事务对象</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">参数</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open) conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
            {
                cmd.Transaction = trans;
            }

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
        }
    }
}
