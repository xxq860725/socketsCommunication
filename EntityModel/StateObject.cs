
using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EntityModel
{
    /****************************************************************************
    *Copyright (c) 2018  All Rights Reserved.
    *CLR版本： 4.0.30319.42000
    *机器名称：WIN-OCE2SQ21FJO
    *命名空间：EntityModel
    *文件名：  StateObject
    *版本号：  V1.0.0.0
    *唯一标识：ff2c2ce2-e842-49f8-82bf-621ca7acaa34
    *当前的用户域：WIN-OCE2SQ21FJO
    *创建人：  LeftYux
    *电子邮箱：87717663@qq.com
    *创建时间：2018-10-31 14:08:59

    *描述：
    *
    *=====================================================================*/
   public  class StateObject
   {   /// <summary>
       /// 网络套接字
       /// </summary>
       public Socket WorkSocket { get; set; }

       /// <summary>
       /// 工作的opc
       /// </summary>
       public OPCServer WorkOpc { get; set; }


       /// <summary>
       /// 是否关闭了通道
       /// </summary>
       public bool IsClose { get; set; }

       public ManualResetEvent WaitDone { get; set; }

       /// <summary>
       /// 是否发生了错误
       /// </summary>
       public bool IsError { get; set; }

       /// <summary>
       /// 错误消息
       /// </summary>
       public string ErrerMsg { get; set; }

       public void Clear()
       {
           IsError = false;
           IsClose = false;
       }

       /// <summary>
       /// 异步对象中的IP地址
       /// </summary>
       public string Ip { get; set; }
       /// <summary>
       /// 异步对象中的opc服务器的主机名称
       /// </summary>
       public string Name { get; set; }

       /// <summary>
       /// 连接状态
       /// </summary>
       public bool OpcStatus { get; set; }

       /// <summary>
       /// 异步传输的元素
       /// </summary>
       public IList<string> tempList { get; set; }
    }
}
