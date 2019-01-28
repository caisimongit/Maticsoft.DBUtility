﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maticsoft.DBUtility
{
    /// <summary>
    /// MySql数据库操作帮助
    /// </summary>
    public abstract class DbHelperMySql
    {
        public static string connectionString = PubConstant.ConnectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static MySqlDataReader ExecuteReader(string strSql)
        {
            MySqlDataReader reader2;
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(strSql, connection);
            try
            {
                connection.Open();
                reader2 = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (MySqlException exception)
            {
                throw exception;
            }
            return reader2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static MySqlDataReader ExecuteReader(string sqlString, params MySqlParameter[] cmdParms)
        {
            MySqlDataReader reader2;
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, conn, null, sqlString, cmdParms);
                MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                reader2 = reader;
            }
            catch (MySqlException exception)
            {
                throw exception;
            }
            return reader2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static int ExecuteSql(string sqlString)
        {
            int num2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlString, connection);
                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch (MySqlException exception)
                {
                    connection.Close();
                    throw exception;
                }
                finally
                {
                    command.Dispose();
                }
            }
            return num2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static int ExecuteSql(string sqlString, string content)
        {
            int num2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlString, connection);
                MySqlParameter parameter = new MySqlParameter("@content", SqlDbType.NText)
                {
                    Value = content
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (MySqlException exception)
                {
                    throw exception;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return num2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static int ExecuteSql(string sqlString, params MySqlParameter[] cmdParms)
        {
            int num2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    int num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return num;
                }
                catch (MySqlException exception)
                {
                    throw exception;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            return num2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static int ExecuteSqlByTime(string sqlString, int times)
        {
            int num2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlString, connection);
                try
                {
                    connection.Open();
                    command.CommandTimeout = times;
                    return command.ExecuteNonQuery();
                }
                catch (MySqlException exception)
                {
                    connection.Close();
                    throw exception;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return num2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static object ExecuteSqlGet(string sqlString, string content)
        {
            object obj3;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlString, connection);
                MySqlParameter parameter = new MySqlParameter("@content", SqlDbType.NText)
                {
                    Value = content
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    object objA = command.ExecuteScalar();
                    if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                    {
                        return null;
                    }
                    obj3 = objA;
                }
                catch (MySqlException exception)
                {
                    throw exception;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return obj3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static int ExecuteSqlInsertImg(string strSql, byte[] fs)
        {
            int num2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(strSql, connection);
                MySqlParameter parameter = new MySqlParameter("@fs", SqlDbType.Image)
                {
                    Value = fs
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (MySqlException exception)
                {
                    throw exception;
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return num2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStringList"></param>
        public static void ExecuteSqlTran(Hashtable sqlStringList)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        foreach (DictionaryEntry entry in sqlStringList)
                        {
                            string cmdText = entry.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])entry.Value;
                            PrepareCommand(cmd, connection, transaction, cmdText, cmdParms);
                            int num = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdList"></param>
        /// <returns></returns>
        public static int ExecuteSqlTran(List<CommandInfo> cmdList)
        {
            int num3;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int num = 0;
                        foreach (CommandInfo info in cmdList)
                        {
                            string commandText = info.CommandText;
                            MySqlParameter[] parameters = (MySqlParameter[])info.Parameters;
                            PrepareCommand(cmd, connection, transaction, commandText, parameters);
                            if ((info.EffentNextType == EffentNextType.WhenHaveContine) || (info.EffentNextType == EffentNextType.WhenNoHaveContine))
                            {
                                if (info.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    transaction.Rollback();
                                    return 0;
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
                                    transaction.Rollback();
                                    return 0;
                                }
                                if ((info.EffentNextType == EffentNextType.WhenNoHaveContine) && flag)
                                {
                                    transaction.Rollback();
                                    return 0;
                                }
                            }
                            else
                            {
                                int num2 = cmd.ExecuteNonQuery();
                                num += num2;
                                if ((info.EffentNextType == EffentNextType.ExcuteEffectRows) && (num2 == 0))
                                {
                                    transaction.Rollback();
                                    return 0;
                                }
                                cmd.Parameters.Clear();
                            }
                        }
                        transaction.Commit();
                        num3 = num;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return num3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStringList"></param>
        /// <returns></returns>
        public static int ExecuteSqlTran(List<string> sqlStringList)
        {
            int num3;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection
                };
                MySqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                try
                {
                    int num = 0;
                    for (int i = 0; i < sqlStringList.Count; i++)
                    {
                        string str = sqlStringList[i];
                        if (str.Trim().Length > 1)
                        {
                            command.CommandText = str;
                            num += command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                    num3 = num;
                }
                catch
                {
                    transaction.Rollback();
                    num3 = 0;
                }
            }
            return num3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="oracleCmdSqlList"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="MySqlException"></exception>
        public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
        {
            int num2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = connection
                };
                MySqlTransaction trans = connection.BeginTransaction();
                cmd.Transaction = trans;
                try
                {
                    foreach (CommandInfo info in list)
                    {
                        object obj2;
                        bool flag;
                        string commandText = info.CommandText;
                        MySqlParameter[] parameters = (MySqlParameter[])info.Parameters;
                        PrepareCommand(cmd, connection, trans, commandText, parameters);
                        if (info.EffentNextType == EffentNextType.SolicitationEvent)
                        {
                            if (info.CommandText.ToLower().IndexOf("count(") == -1)
                            {
                                trans.Rollback();
                                throw new Exception("违背要求" + info.CommandText + "必须符合select count(..的格式");
                            }
                            obj2 = cmd.ExecuteScalar();
                            flag = false;
                            if ((obj2 == null) && (obj2 == DBNull.Value))
                            {
                                flag = false;
                            }
                            flag = Convert.ToInt32(obj2) > 0;
                            if (flag)
                            {
                                info.OnSolicitationEvent();
                            }
                        }
                        if ((info.EffentNextType == EffentNextType.WhenHaveContine) || (info.EffentNextType == EffentNextType.WhenNoHaveContine))
                        {
                            if (info.CommandText.ToLower().IndexOf("count(") == -1)
                            {
                                trans.Rollback();
                                throw new Exception("SQL:违背要求" + info.CommandText + "必须符合select count(..的格式");
                            }
                            obj2 = cmd.ExecuteScalar();
                            flag = false;
                            if ((obj2 == null) && (obj2 == DBNull.Value))
                            {
                                flag = false;
                            }
                            flag = Convert.ToInt32(obj2) > 0;
                            if (!((info.EffentNextType != EffentNextType.WhenHaveContine) || flag))
                            {
                                trans.Rollback();
                                throw new Exception("SQL:违背要求" + info.CommandText + "返回值必须大于0");
                            }
                            if ((info.EffentNextType == EffentNextType.WhenNoHaveContine) && flag)
                            {
                                trans.Rollback();
                                throw new Exception("SQL:违背要求" + info.CommandText + "返回值必须等于0");
                            }
                        }
                        else
                        {
                            int num = cmd.ExecuteNonQuery();
                            if ((info.EffentNextType == EffentNextType.ExcuteEffectRows) && (num == 0))
                            {
                                trans.Rollback();
                                throw new Exception("SQL:违背要求" + info.CommandText + "必须有影响行");
                            }
                            cmd.Parameters.Clear();
                        }
                    }
                    if (!OracleHelper.ExecuteSqlTran(PubConstant.GetConnectionString("ConnectionStringPPC"), oracleCmdSqlList))
                    {
                        trans.Rollback();
                        throw new Exception("执行失败");
                    }
                    trans.Commit();
                    num2 = 1;
                }
                catch (MySqlException exception)
                {
                    trans.Rollback();
                    throw exception;
                }
                catch (Exception exception2)
                {
                    trans.Rollback();
                    throw exception2;
                }
            }
            return num2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStringList"></param>
        public static void ExecuteSqlTranWithIndentity(List<CommandInfo> sqlStringList)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int num = 0;
                        foreach (CommandInfo info in sqlStringList)
                        {
                            string commandText = info.CommandText;
                            MySqlParameter[] parameters = (MySqlParameter[])info.Parameters;
                            foreach (MySqlParameter parameter in parameters)
                            {
                                if (parameter.Direction == ParameterDirection.InputOutput)
                                {
                                    parameter.Value = num;
                                }
                            }
                            PrepareCommand(cmd, connection, transaction, commandText, parameters);
                            int num2 = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter parameter in parameters)
                            {
                                if (parameter.Direction == ParameterDirection.Output)
                                {
                                    num = Convert.ToInt32(parameter.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStringList"></param>
        public static void ExecuteSqlTranWithIndentity(Hashtable sqlStringList)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int num = 0;
                        foreach (DictionaryEntry entry in sqlStringList)
                        {
                            string cmdText = entry.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])entry.Value;
                            foreach (MySqlParameter parameter in cmdParms)
                            {
                                if (parameter.Direction == ParameterDirection.InputOutput)
                                {
                                    parameter.Value = num;
                                }
                            }
                            PrepareCommand(cmd, connection, transaction, cmdText, cmdParms);
                            int num2 = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter parameter in cmdParms)
                            {
                                if (parameter.Direction == ParameterDirection.Output)
                                {
                                    num = Convert.ToInt32(parameter.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static bool Exists(string strSql)
        {
            int num;
            object single = GetSingle(strSql);
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
        /// <param name="strSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
        {
            int num;
            object single = GetSingle(strSql, cmdParms);
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
        /// <param name="fieldName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static int GetMaxId(string fieldName, string tableName)
        {
            object single = GetSingle("select max(" + fieldName + ")+1 from " + tableName);
            if (single == null)
            {
                return 1;
            }
            return int.Parse(single.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static object GetSingle(string sqlString)
        {
            object obj3;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlString, connection);
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
                catch (MySqlException exception)
                {
                    connection.Close();
                    throw exception;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return obj3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static object GetSingle(string sqlString, int times)
        {
            object obj3;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlString, connection);
                try
                {
                    connection.Open();
                    command.CommandTimeout = times;
                    object objA = command.ExecuteScalar();
                    if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                    {
                        return null;
                    }
                    return objA;
                }
                catch (MySqlException exception)
                {
                    connection.Close();
                    throw exception;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return obj3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static object GetSingle(string sqlString, params MySqlParameter[] cmdParms)
        {
            object obj3;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    object objA = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                    {
                        return null;
                    }
                    return objA;
                }
                catch (MySqlException exception)
                {
                    throw exception;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            return obj3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
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
                foreach (MySqlParameter parameter in cmdParms)
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
        /// <param name="sqlString"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataSet Query(string sqlString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                try
                {
                    connection.Open();
                    new MySqlDataAdapter(sqlString, connection).Fill(dataSet, "ds");
                }
                catch (MySqlException exception)
                {
                    throw new Exception(exception.Message);
                }
                return dataSet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataSet Query(string sqlString, int times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                try
                {
                    connection.Open();
                    new MySqlDataAdapter(sqlString, connection) { SelectCommand = { CommandTimeout = times } }.Fill(dataSet, "ds");
                }
                catch (MySqlException exception)
                {
                    throw new Exception(exception.Message);
                }
                return dataSet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataSet Query(string sqlString, params MySqlParameter[] cmdParms)
        {
            DataSet set2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        adapter.Fill(dataSet, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySqlException exception)
                    {
                        throw new Exception(exception.Message);
                    }
                    set2 = dataSet;
                }
            }
            return set2;
        }
    }
}
