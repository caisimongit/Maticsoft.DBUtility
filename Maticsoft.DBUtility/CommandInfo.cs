using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maticsoft.DBUtility
{
    /// <summary>
    /// 命令信息
    /// </summary>
    public class CommandInfo
    {
        public string CommandText;
        public EffentNextType EffentNextType;
        public object OriginalData;
        public DbParameter[] Parameters;
        public object ShareObject;

        private event EventHandler _solicitationEvent;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler SolicitationEvent
        {
            add
            {
                this._solicitationEvent += value;
            }
            remove
            {
                this._solicitationEvent -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandInfo()
        {
            ShareObject = null;
            OriginalData = null;
            EffentNextType = EffentNextType.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="para"></param>
        public CommandInfo(string sqlText, SqlParameter[] para)
        {
            ShareObject = null;
            OriginalData = null;
            EffentNextType = EffentNextType.None;
            CommandText = sqlText;
            Parameters = para;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="para"></param>
        /// <param name="type"></param>
        public CommandInfo(string sqlText, SqlParameter[] para, EffentNextType type)
        {
            ShareObject = null;
            OriginalData = null;
            EffentNextType = EffentNextType.None;
            CommandText = sqlText;
            Parameters = para;
            EffentNextType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnSolicitationEvent()
        {
            if (_solicitationEvent != null)
            {
                _solicitationEvent(this, new EventArgs());
            }
        }
    }
}
