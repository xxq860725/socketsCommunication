using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Sockets
{
    /****************************************************************************
   *Copyright (c) 2018  All Rights Reserved.
   *CLR版本： 4.0.30319.42000
   *机器名称：WIN-OCE2SQ21FJO
   *命名空间：Sockets
   *文件名：  SocketCoreServer
   *版本号：  V1.0.0.0
   *唯一标识：992c865f-bbcb-4dd5-b141-86542d86677e
   *当前的用户域：WIN-OCE2SQ21FJO
   *创建人：  LeftYux
   *电子邮箱：87717663@qq.com
   *创建时间：2018-10-30 17:31:59
   *描述：
   *
   *=====================================================================*/
    public class SocketCoreServer : abstractSockets
    {

        /// <summary>
        /// 信号量
        /// </summary>
        private Semaphore semap = new Semaphore(5, 5000);

        /// <summary>
        /// 客户端列表集合
        /// </summary>
        public List<tempSockets> ClientList = new List<tempSockets>();


        /// <summary>
        /// 服务端实例对象
        /// </summary>
        public TcpListener Listener;

        /// <summary>
        /// 当前的ip地址
        /// </summary>
        private IPAddress IpAddress;

        /// <summary>
        /// 初始化消息
        /// </summary>
        private string InitMsg = "TCP服务端";

        /// <summary>
        /// 监听的端口
        /// </summary>
        private int Port;

        /// <summary>
        /// 当前ip和端口节点对象
        /// </summary>
        private IPEndPoint Ip;



        public override void InitSocket(IPAddress ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.Listener = new TcpListener(ipAddress,port);
        }

        public override void InitSocket(string ipAddress, int port)
        {
            this.IpAddress = IPAddress.Parse(ipAddress);
            this.Port = port;
            this.Ip = new IPEndPoint(IpAddress, Port);
            this.Listener = new TcpListener(IpAddress, Port);
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
