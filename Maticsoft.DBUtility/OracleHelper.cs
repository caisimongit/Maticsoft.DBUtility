using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maticsoft.DBUtility
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class OracleHelper
    {
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        protected OracleHelper()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="commandParameters"></param>
        public static void CacheParameters(string cacheKey, params OracleParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, string cmdText)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(connectionString);
            PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, null);
            int num = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int num = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int num = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int num = cmd.ExecuteNonQuery();
                connection.Close();
                cmd.Parameters.Clear();
                return num;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static OracleDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleDataReader reader2;
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(OracleConnection connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connectionString, null, cmdType, cmdText, commandParameters);
            object obj2 = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return obj2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked\tor commited, please\tprovide\tan open\ttransaction.", "transaction");
            }
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
            object obj2 = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return obj2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object obj2 = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="cmdList"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="OracleException"></exception>
        public static bool ExecuteSqlTran(string conStr, List<CommandInfo> cmdList)
        {
            bool flag2;
            using (OracleConnection connection = new OracleConnection(conStr))
            {
                connection.Open();
                OracleCommand cmd = new OracleCommand
                {
                    Connection = connection
                };
                OracleTransaction trans = connection.BeginTransaction();
                cmd.Transaction = trans;
                try
                {
                    foreach (CommandInfo info in cmdList)
                    {
                        if (!string.IsNullOrEmpty(info.CommandText))
                        {
                            PrepareCommand(cmd, connection, trans, CommandType.Text, info.CommandText, (OracleParameter[])info.Parameters);
                            if ((info.EffentNextType == EffentNextType.WhenHaveContine) || (info.EffentNextType == EffentNextType.WhenNoHaveContine))
                            {
                                if (info.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    trans.Rollback();
                                    throw new Exception("Oracle:违背要求" + info.CommandText + "必须符合select count(..的格式");
                                }
                                object obj2 = cmd.ExecuteScalar();
                                bool flag = false;
                                if ((obj2 == null) && (obj2 == DBNull.Value))
                                {
                                    flag = false;
                                }
                                flag = Convert.ToInt32(obj2) > 0;
                                if (!((info.EffentNextType != EffentNextType.WhenHaveContine) || flag))
                                {
                                    trans.Rollback();
                                    throw new Exception("Oracle:违背要求" + info.CommandText + "返回值必须大于0");
                                }
                                if ((info.EffentNextType == EffentNextType.WhenNoHaveContine) && flag)
                                {
                                    trans.Rollback();
                                    throw new Exception("Oracle:违背要求" + info.CommandText + "返回值必须等于0");
                                }
                            }
                            else
                            {
                                int num = cmd.ExecuteNonQuery();
                                if ((info.EffentNextType == EffentNextType.ExcuteEffectRows) && (num == 0))
                                {
                                    trans.Rollback();
                                    throw new Exception("Oracle:违背要求" + info.CommandText + "必须有影像行");
                                }
                            }
                        }
                    }
                    trans.Commit();
                    flag2 = true;
                }
                catch (OracleException exception)
                {
                    trans.Rollback();
                    throw exception;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return flag2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="sqlStringList"></param>
        /// <exception cref="Exception"></exception>
        public static void ExecuteSqlTran(string conStr, List<string> sqlStringList)
        {
            using (OracleConnection connection = new OracleConnection(conStr))
            {
                connection.Open();
                OracleCommand command = new OracleCommand
                {
                    Connection = connection
                };
                OracleTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                try
                {
                    foreach (string str in sqlStringList)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            command.CommandText = str;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (OracleException exception)
                {
                    transaction.Rollback();
                    throw new Exception(exception.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="strOracle"></param>
        /// <returns></returns>
        public static bool Exists(string connectionString, string strOracle)
        {
            int num;
            object single = GetSingle(connectionString, strOracle);
            if (object.Equals(single, null) || object.Equals(single, DBNull.Value))
            {
                num = 0;
            }
            else
            {
                num = int.Parse(single.ToString());
            }
            if (num == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public static OracleParameter[] GetCachedParameters(string cacheKey)
        {
            OracleParameter[] parameterArray = (OracleParameter[])parmCache[cacheKey];
            if (parameterArray == null)
            {
                return null;
            }
            OracleParameter[] parameterArray2 = new OracleParameter[parameterArray.Length];
            int index = 0;
            int length = parameterArray.Length;
            while (index < length)
            {
                parameterArray2[index] = (OracleParameter)((ICloneable)parameterArray[index]).Clone();
                index++;
            }
            return parameterArray2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static object GetSingle(string connectionString, string sqlString)
        {
            object obj3;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        object objA = command.ExecuteScalar();
                        if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                        {
                            return null;
                        }
                        return objA;
                    }
                    catch (OracleException exception)
                    {
                        throw new Exception(exception.Message);
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                    }
                }
            }
            return obj3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string OraBit(bool value)
        {
            return value ? "Y" : "N";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool OraBool(string value)
        {
            return value.Equals("Y");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (OracleParameter parameter in cmdParms)
                {
                    if (((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Input)) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            if (commandParameters != null)
            {
                foreach (OracleParameter parameter in commandParameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataSet Query(string connectionString, string sqlString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                try
                {
                    connection.Open();
                    new OracleDataAdapter(sqlString, connection).Fill(dataSet, "ds");
                }
                catch (OracleException exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
                return dataSet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataSet Query(string connectionString, string sqlString, params OracleParameter[] cmdParms)
        {
            DataSet set2;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        adapter.Fill(dataSet, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OracleException exception)
                    {
                        throw new Exception(exception.Message);
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                    }
                    set2 = dataSet;
                }
            }
            return set2;
        }
    }
}
