using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maticsoft.DBUtility
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DbHelperSqLite
    {
        public static string connectionString = PubConstant.ConnectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static SQLiteDataReader ExecuteReader(string strSql)
        {
            SQLiteDataReader reader2;
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand(strSql, connection);
            try
            {
                connection.Open();
                reader2 = command.ExecuteReader();
            }
            catch (SQLiteException exception)
            {
                throw new Exception(exception.Message);
            }
            return reader2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static SQLiteDataReader ExecuteReader(string sqlString, params SQLiteParameter[] cmdParms)
        {
            SQLiteDataReader reader2;
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                PrepareCommand(cmd, conn, null, sqlString, cmdParms);
                SQLiteDataReader reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                reader2 = reader;
            }
            catch (SQLiteException exception)
            {
                throw new Exception(exception.Message);
            }
            return reader2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int ExecuteSql(string sqlString)
        {
            int num2;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(sqlString, connection);
                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch (SQLiteException exception)
                {
                    connection.Close();
                    throw new Exception(exception.Message);
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
        /// <exception cref="Exception"></exception>
        public static int ExecuteSql(string sqlString, string content)
        {
            int num2;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(sqlString, connection);
                SQLiteParameter parameter = new SQLiteParameter("@content", DbType.String)
                {
                    Value = content
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (SQLiteException exception)
                {
                    throw new Exception(exception.Message);
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
        /// <exception cref="Exception"></exception>
        public static int ExecuteSql(string sqlString, params SQLiteParameter[] cmdParms)
        {
            int num2;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    int num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return num;
                }
                catch (SQLiteException exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    cmd.Dispose();
                }
            }
            return num2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int ExecuteSqlInsertImg(string strSql, byte[] fs)
        {
            int num2;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(strSql, connection);
                SQLiteParameter parameter = new SQLiteParameter("@fs", DbType.Binary)
                {
                    Value = fs
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (SQLiteException exception)
                {
                    throw new Exception(exception.Message);
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
        /// <exception cref="Exception"></exception>
        public static void ExecuteSqlTran(ArrayList sqlStringList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection
                };
                SQLiteTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                try
                {
                    for (int i = 0; i < sqlStringList.Count; i++)
                    {
                        string str = sqlStringList[i].ToString();
                        if (str.Trim().Length > 1)
                        {
                            command.CommandText = str;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (SQLiteException exception)
                {
                    transaction.Rollback();
                    throw new Exception(exception.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStringList"></param>
        public static void ExecuteSqlTran(Hashtable sqlStringList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    try
                    {
                        foreach (DictionaryEntry entry in sqlStringList)
                        {
                            string cmdText = entry.Key.ToString();
                            SQLiteParameter[] cmdParms = (SQLiteParameter[])entry.Value;
                            PrepareCommand(cmd, connection, transaction, cmdText, cmdParms);
                            int num = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            transaction.Commit();
                        }
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
            return num != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool Exists(string strSql, params SQLiteParameter[] cmdParms)
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
            return num != 0;
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
            return single == null ? 1 : int.Parse(single.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static object GetSingle(string sqlString)
        {
            object obj3;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(sqlString, connection);
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
                catch (SQLiteException exception)
                {
                    connection.Close();
                    throw new Exception(exception.Message);
                }
                finally
                {
                    command.Dispose();
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
        /// <exception cref="Exception"></exception>
        public static object GetSingle(string sqlString, params SQLiteParameter[] cmdParms)
        {
            object obj3;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
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
                catch (SQLiteException exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    cmd.Dispose();
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
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, string cmdText, SQLiteParameter[] cmdParms)
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
                foreach (SQLiteParameter parameter in cmdParms)
                {
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
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                try
                {
                    connection.Open();
                    new SQLiteDataAdapter(sqlString, connection).Fill(dataSet, "ds");
                }
                catch (SQLiteException exception)
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
        public static DataSet Query(string sqlString, params SQLiteParameter[] cmdParms)
        {
            DataSet set2;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        adapter.Fill(dataSet, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SQLiteException exception)
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
