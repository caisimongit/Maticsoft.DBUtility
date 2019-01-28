using System;
using System.Collections;
using System.Collections.Generic;
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
    public abstract class DbHelperOra
    {

        public static string connectionString = PubConstant.ConnectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static OracleCommand BuildIntCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new OracleParameter("ReturnValue", OracleType.Int32, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static OracleCommand BuildQueryCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static OracleDataReader ExecuteReader(string strSql)
        {
            OracleDataReader reader2;
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand command = new OracleCommand(strSql, connection);
            try
            {
                connection.Open();
                reader2 = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (OracleException exception)
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
        public static OracleDataReader ExecuteReader(string sqlString, params OracleParameter[] cmdParms)
        {
            OracleDataReader reader2;
            OracleConnection conn = new OracleConnection(connectionString);
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, conn, null, sqlString, cmdParms);
                OracleDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                reader2 = reader;
            }
            catch (OracleException exception)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand command = new OracleCommand(sqlString, connection);
                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch (OracleException exception)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand command = new OracleCommand(sqlString, connection);
                OracleParameter parameter = new OracleParameter("@content", OracleType.NVarChar)
                {
                    Value = content
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (OracleException exception)
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
        public static int ExecuteSql(string sqlString, params OracleParameter[] cmdParms)
        {
            int num2;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    int num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return num;
                }
                catch (OracleException exception)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand command = new OracleCommand(strSql, connection);
                OracleParameter parameter = new OracleParameter("@fs", OracleType.LongRaw)
                {
                    Value = fs
                };
                command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    num2 = command.ExecuteNonQuery();
                }
                catch (OracleException exception)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
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
                catch (OracleException exception)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleTransaction transaction = connection.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        foreach (DictionaryEntry entry in sqlStringList)
                        {
                            string cmdText = entry.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])entry.Value;
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
        public static bool Exists(string strSql, params OracleParameter[] cmdParms)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand command = new OracleCommand(sqlString, connection);
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
        public static object GetSingle(string sqlString, params OracleParameter[] cmdParms)
        {
            object obj3;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
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
                catch (OracleException exception)
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
        public static DataSet Query(string sqlString, params OracleParameter[] cmdParms)
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
                    set2 = dataSet;
                }
            }
            return set2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static OracleDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            connection.Open();
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                new OracleDataAdapter { SelectCommand = BuildQueryCommand(connection, storedProcName, parameters) }.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="rowsAffected"></param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                OracleCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                return (int)command.Parameters["ReturnValue"].Value;
            }
        }
    }
}
