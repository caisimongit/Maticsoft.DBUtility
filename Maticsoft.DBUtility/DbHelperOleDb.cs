using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maticsoft.DBUtility
{
   /// <summary>
   /// 
   /// </summary>
   public abstract class DbHelperOleDb
    {
        public static string connectionString = PubConstant.ConnectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static OleDbDataReader ExecuteReader(string strSql)
        {
            OleDbDataReader reader2;
            OleDbConnection connection = new OleDbConnection(connectionString);
            OleDbCommand command = new OleDbCommand(strSql, connection);
            try
            {
                connection.Open();
                reader2 = command.ExecuteReader();
            }
            catch (OleDbException exception)
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
        public static OleDbDataReader ExecuteReader(string sqlString, params OleDbParameter[] cmdParms)
        {
            OleDbDataReader reader2;
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand();
            try
            {
                PrepareCommand(cmd, conn, null, sqlString, cmdParms);
                OleDbDataReader reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                reader2 = reader;
            }
            catch (OleDbException exception)
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
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(sqlString, connection);
                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch (OleDbException exception)
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
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(sqlString, connection);
                OleDbParameter parameter = new OleDbParameter("@content", OleDbType.VarChar)
                {
                    Value = content
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (OleDbException exception)
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
        public static int ExecuteSql(string sqlString, params OleDbParameter[] cmdParms)
        {
            int num2;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand cmd = new OleDbCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    int num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return num;
                }
                catch (OleDbException exception)
                {
                    throw new Exception(exception.Message);
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
        /// <param name="strSql"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int ExecuteSqlInsertImg(string strSql, byte[] fs)
        {
            int num2;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(strSql, connection);
                OleDbParameter parameter = new OleDbParameter("@fs", OleDbType.Binary)
                {
                    Value = fs
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (OleDbException exception)
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
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand
                {
                    Connection = connection
                };
                OleDbTransaction transaction = connection.BeginTransaction();
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
                catch (OleDbException exception)
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
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                using (OleDbTransaction transaction = connection.BeginTransaction())
                {
                    OleDbCommand cmd = new OleDbCommand();
                    try
                    {
                        foreach (DictionaryEntry entry in sqlStringList)
                        {
                            string cmdText = entry.Key.ToString();
                            OleDbParameter[] cmdParms = (OleDbParameter[])entry.Value;
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
            object single = DbHelperSql.GetSingle(strSql);
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
        public static bool Exists(string strSql, params OleDbParameter[] cmdParms)
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
        public static int GetMaxID(string fieldName, string tableName)
        {
            object single = DbHelperSql.GetSingle("select max(" + fieldName + ")+1 from " + tableName);
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
        /// <exception cref="Exception"></exception>
        public static object GetSingle(string sqlString)
        {
            object obj3;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(sqlString, connection);
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
                catch (OleDbException exception)
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
        public static object GetSingle(string sqlString, params OleDbParameter[] cmdParms)
        {
            object obj3;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand cmd = new OleDbCommand();
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
                catch (OleDbException exception)
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
        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, string cmdText, OleDbParameter[] cmdParms)
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
                foreach (OleDbParameter parameter in cmdParms)
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
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                try
                {
                    connection.Open();
                    new OleDbDataAdapter(sqlString, connection).Fill(dataSet, "ds");
                }
                catch (OleDbException exception)
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
        public static DataSet Query(string sqlString, params OleDbParameter[] cmdParms)
        {
            DataSet set2;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        adapter.Fill(dataSet, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OleDbException exception)
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
