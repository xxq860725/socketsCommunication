using Sockets.InterfaceSocket;
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
   *创建时间：2018-10-30 17:31:59
   *描述：
   *
   *=====================================================================*/

    public delegate void Push(tempSockets sockets);//推送消息的委托
    public class SocketCoreServer : abstractSockets, IClientSend
    {
        IClientSend clentSend = null;
        public static Push messge;

        private bool IsStop = false;

        object obj = new object();
        /// <summary>
        /// 信号量
        /// </summary>
        private Semaphore semap = new Semaphore(5, 5000);

        private ManualResetEvent Done = new ManualResetEvent(false);

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
        private string InitMsg = "TCP Server Side";

        /// <summary>
        /// 监听的端口
        /// </summary>
        private int Port;

        /// <summary>
        /// 当前ip和端口节点对象
        /// </summary>
        private IPEndPoint Ip;



        public SocketCoreServer()
        {
            clentSend = new SocketCoreServer();
        }
        /// <summary>
        /// 初始化重载
        /// </summary>
        /// <param name="ipAddress">ip地址</param>
        /// <param name="port">端口号</param>
        public override void InitSocket(IPAddress ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.Listener = new TcpListener(ipAddress,port);
        }
        /// <summary>
        /// 初始化重载
        /// </summary>
        /// <param name="ipAddress">ip地址</param>
        /// <param name="port">端口号</param>
        public override void InitSocket(string ipAddress, int port)
        {
            this.IpAddress = IPAddress.Parse(ipAddress);
            this.Port = port;
            this.Ip = new IPEndPoint(IpAddress, Port);
            this.Listener = new TcpListener(IpAddress, Port);
        }

        /// <summary>
        /// Socket启动方法
        /// </summary>
        public override void Start()
        {
            try
            {
                Listener.Start();
                ThreadPool.QueueUserWorkItem(new WaitCallback(t => {
                    while (true)
                    {
                        if (IsStop){break;}
                        GetNewAcceptConnection();
                        Thread.Sleep(1);
                    }               
                }));
            }
            catch (SocketException ex)
            {
                tempSockets temp = new tempSockets();
                temp.ex = ex;
                messge.Invoke(temp);
            }
        }


        #region 针对启动函数的辅助函数封装
        /// <summary>
        /// 获取新的客户端连接
        /// </summary>
        private void GetNewAcceptConnection()
        {
           
            
            try
            {
                if (Listener.Pending())
                {
                    Done.WaitOne();
                    //接收到挂起的客户端请求链接
                    TcpClient tcpClient = Listener.AcceptTcpClient(); 
                    //维护处理客户端队列
                    Socket socket = tcpClient.Client;
                    NetworkStream stream = new NetworkStream(socket, true);
                    tempSockets tsk = new tempSockets(tcpClient.Client.RemoteEndPoint as IPEndPoint,tcpClient,stream);
                    //客户端上线讲Flag设为True
                    tsk.NewClientFlag = true;
                    //推送新的客户端连接信息
                    messge.Invoke(tsk);
                    //异步读取消息
                    tsk.nStream.BeginRead(tsk.RecBuffer,0,tsk.RecBuffer.Length,new AsyncCallback(EndReader),tsk);
                    //将当前socket对象加入到队列
                    clentSend.AddClientQueue(tsk);

                    //链接成功后主动向客户端发送一条消息
                    if (stream.CanWrite)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes("Welcome to join");
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    Done.Set();
                }

            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// 接收数据的异步回调
        /// </summary>
        /// <param name="ar">异步操作状态</param>
        private void EndReader(IAsyncResult ar)
        {
            tempSockets tks = ar.AsyncState as tempSockets;
            if (tks != null && Listener != null)
            {
                try
                {
                    if (tks.NewClientFlag || tks.Offset != 0)
                    {
                        //一个周期走完置为false
                        tks.NewClientFlag = false;
                        tks.Offset = tks.nStream.EndRead(ar);
                        messge.Invoke(tks);
                        tks.nStream.BeginRead(tks.RecBuffer, 0, tks.RecBuffer.Length, new AsyncCallback(EndReader), tks);
                    }
                }
                catch (Exception skex)
                {
                    lock (obj)
                    {
                        ClientList.Remove(tks);
                        tempSockets sk = tks;
                        sk.ClientDispose = true;
                        sk.ex = skex;
                        messge.Invoke(tks);               
                    }
                }
            }

        
        }


        /// <summary>
        /// 将新加入连接的Socket添加到集合ListSocket
        /// </summary>
        /// <param name="qts">当前操作新加入的Socket</param>
        public void AddClientQueue(tempSockets qts)
        {
            lock (this)
            {
                //在当前列表里找到是否有着一个连接进入
                //如果进如存在先删除再添加，如果不存在则添加到队列中
                tempSockets sockets = ClientList.Find(s => { return s.Ip == qts.Ip; });
                if (sockets == null)
                {
                    ClientList.Add(qts);
                }
                else { ClientList.Remove(sockets); ClientList.Add(qts); }
            }
        }
     
        #endregion

        /// <summary>
        /// Sockdet停止方法
        /// </summary>
        public override void Stop()
        {
            if (Listener != null) 
            {
                Listener.Stop();
                Listener = null;
                IsStop = true;
                messge = null;
            }
        }


        /// <summary>
        /// 向客户端发送一条消息
        /// </summary>
        /// <param name="ip">客户端ip</param>
        /// <param name="SendData">需要发送的内容</param>
        public void SendToClient(IPEndPoint ip, string SendData)
        {
           
        }




    }
}
