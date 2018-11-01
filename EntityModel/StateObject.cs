
using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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


       #region OPc
       /// <summary>
       /// 工作的opc
       /// </summary>
       public OPCServer WorkOpc { get; set; }
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
       #endregion
   }

   public class ClientSocket : StateObject
   {
       public ClientSocket()
       {
          
       }
       public ClientSocket(IPEndPoint ip, Socket client, NetworkStream ns)
       {
           this.Ip = ip;
           this.ClientSockets = client;
           this.nStream = ns;
       }
       /// <summary>
       /// 承载客户端Socket的网络流
       /// </summary>
       public NetworkStream nStream { get; set; }
       /// <summary>
       /// 接收缓冲区大小8k
       /// </summary>
       public byte[] RecBuffer = new byte[8 * 1024];

       /// <summary>
       /// 发送缓冲区大小8k
       /// </summary>
       public byte[] SendBuffer = new byte[8 * 1024];


       /// <summary>
       /// 发生异常时不为null.
       /// </summary>
       public Exception ex { get; set; }

       /// <summary>
       /// 新客户端标识.如果推送器发现此标识为true,那么认为是客户端上线
       /// 仅服务端有效
       /// </summary>
       public bool NewClientFlag { get; set; }

       /// <summary>
       /// 当前IP地址,端口号
       /// </summary>
       public IPEndPoint Ip { get; set; }

       /// <summary>
       /// 客户端套接字
       /// </summary>
       public Socket ClientSockets { get; set; }


       public int Offset { get; set; }

       /// 客户端退出标识.如果服务端发现此标识为true,那么认为客户端下线
       /// 客户端接收此标识时,认为客户端异常.
       /// </summary>
       public bool ClientDispose { get; set; }

   }
}
