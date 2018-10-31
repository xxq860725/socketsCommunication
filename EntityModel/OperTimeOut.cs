
using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace EntityModel
{
    /****************************************************************************
    *Copyright (c) 2018  All Rights Reserved.
    *CLR版本： 4.0.30319.42000
    *机器名称：WIN-OCE2SQ21FJO
    *命名空间：EntityModel
    *文件名：  OperTimeOut
    *版本号：  V1.0.0.0
    *唯一标识：a7958d43-21cb-41be-aac4-35acbeb66de7
    *当前的用户域：WIN-OCE2SQ21FJO
    *创建人：  LeftYux
    *创建时间：2018-10-31 11:07:38

    *描述：
    *
    *=====================================================================*/
   public class OperTimeOut
    {
        /// <summary>
        /// 操作的开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccessful { get; set; }
        /// <summary>
        /// 延时的时间，单位毫秒
        /// </summary>
        public int DelayTime { get; set; }
        /// <summary>
        /// 连接超时用的Socket
        /// </summary>
        public Socket WorkSocket { get; set; }

        /// <summary>
        /// 连接超时用的opc对象
        /// </summary>
        public OPCServer WorkOPc { get; set; }
        /// <summary>
        /// 用于超时执行的方法
        /// </summary>
        public Action Operator { get; set; }
    }
}
