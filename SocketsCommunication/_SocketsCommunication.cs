
using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketsCommunication
{
   
    /****************************************************************************
    *Copyright (c) 2018  All Rights Reserved.
    *CLR版本： 4.0.30319.42000
    *机器名称：WIN-OCE2SQ21FJO
    *命名空间：SocketsCommunication
    *文件名：  _SocketsCommunication
    *版本号：  V1.0.0.0
    *唯一标识：28a56ab0-3e25-4914-bd54-a890078a78ba
    *当前的用户域：WIN-OCE2SQ21FJO
    *创建人：  LeftYux
    *创建时间：2018-11-01 17:07:34
    *描述：
    *Socket服务器接收一次最大连接数字为 255
    *
    *=====================================================================*/
   public  class _SocketsCommunication
   {
       /// <summary>
       /// 推送消息到界面的事件
       /// </summary>
       public static CustomEventHandler._CustomEventHandler.TellCustomEventHandler<ClientSocket> PushClient;


       List<ClientSocket> list = new List<ClientSocket>();

       object obj1 = new object();
       /// <summary>
       /// 信号量
       /// </summary>
       private Semaphore semap = new Semaphore(5, 5000);
       ManualResetEvent mdone = new ManualResetEvent(false);
       #region ServerSide


       public void ServerConnect(string ip, int port)
       {
           Result result = new Result();
           var sockets = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           ManualResetEvent mandone = new ManualResetEvent(false);
           sockets.Bind(new IPEndPoint(IPAddress.Parse(ip),port));
           sockets.Listen(255);
           ClientSocket state = new ClientSocket();
           try
           {
               state.ClientSockets = sockets;
               ThreadPool.QueueUserWorkItem(new WaitCallback(GetAccectCallBack), state);

             
           }
           catch (Exception sex)
           {
               ClientSocket s = new ClientSocket();
               result.Message = sex.ToString() ;
               PushClient.Invoke(s);
           }
          
       }

       private void GetAccectCallBack(object obj)
       {
           ClientSocket state = obj as ClientSocket;
           try
           {
               if (state != null)
               {
                   while (true)
                   {
                       semap.WaitOne();
                       Socket client = state.ClientSockets.Accept();
                       NetworkStream stream = new NetworkStream(client, true);
                       ClientSocket cst = new ClientSocket(client.RemoteEndPoint as IPEndPoint, client, stream);
                       //客户端上线讲Flag设为True
                       cst.NewClientFlag = true;
                       PushClient.Invoke(cst);
                       cst.nStream.BeginRead(cst.RecBuffer, 0, cst.RecBuffer.Length, new AsyncCallback(AsyncEndReader), cst);
                       AddClientQueue(cst);
                       if (stream.CanWrite)
                       {
                           byte[] buffer = Encoding.UTF8.GetBytes("Welcome to join");
                           stream.Write(buffer, 0, buffer.Length);
                       }
                       semap.Release();
                       Thread.Sleep(1);
                   }
               }
           }
           catch (Exception ex)
           {
               if (state.ClientSockets != null)
               {
                   state.ClientSockets.Close();
               }
               return;
           }
          
       }

       /// <summary>
       /// 异步流中读取对象的回调
       /// </summary>
       /// <param name="ir"></param>
       private void AsyncEndReader(IAsyncResult ir)
       {
           ClientSocket tks =ir.AsyncState as ClientSocket;
           if (tks != null)
           {
               try
               {
                   if (tks.NewClientFlag || tks.Offset != 0)
                   {
                       //一个周期走完置为false
                       tks.NewClientFlag = false;
                       tks.Offset = tks.nStream.EndRead(ir);
                       PushClient.Invoke(tks);
                       tks.nStream.BeginRead(tks.RecBuffer, 0, tks.RecBuffer.Length, new AsyncCallback(AsyncEndReader), tks);
                   }
               }
               catch (Exception skex)
               {
                   lock (obj1)
                   {
                       list.Remove(tks);
                       ClientSocket sk = tks;
                       sk.ClientDispose = true;
                       sk.ex = skex;
                       PushClient.Invoke(tks);
                   }
               }
           }
       }

       /// <summary>
       /// 将新加入连接的Socket添加到集合ListSocket
       /// </summary>
       /// <param name="qts">当前操作新加入的Socket</param>
       public void AddClientQueue(ClientSocket qts)
       {
           lock (this)
           {
               //在当前列表里找到是否有着一个连接进入
               //如果进如存在先删除再添加，如果不存在则添加到队列中
               ClientSocket sockets = list.Find(s => { return s.Ip == qts.Ip; });
               if (sockets == null)
               {
                   list.Add(qts);
               }
               else { list.Remove(sockets); list.Add(qts); }
           }
       }


       /// <summary>
       /// 向单独的一个客户端发送消息
       /// </summary>
       /// <param name="ip"></param>
       /// <param name="SendData"></param>
       public void SendToClient(IPEndPoint ip, string SendData)
       {
           try
           {
               ClientSocket sks = list.Find(o => { return o.Ip == ip; });
               if (sks == null || !sks.ClientSockets.Connected)
               {
                   ClientSocket ks = new ClientSocket();
                   //标识客户端下线
                   sks.ClientDispose = true;
                   sks.ex = new Exception("客户端没有连接");
                   PushClient.Invoke(sks);
               }
               if (sks.ClientSockets.Connected)
               {
                   //获取当前流进行写入.
                   NetworkStream nStream = sks.nStream;
                   if (nStream.CanWrite)
                   {
                       byte[] buffer = Encoding.UTF8.GetBytes(SendData);
                       nStream.Write(buffer, 0, buffer.Length);
                   }
                   else
                   {
                       //避免流被关闭,重新从对象中获取流
                       nStream = sks.nStream;
                       if (nStream.CanWrite)
                       {
                           byte[] buffer = Encoding.UTF8.GetBytes(SendData);
                           nStream.Write(buffer, 0, buffer.Length);
                       }
                       else
                       {
                           //如果还是无法写入,那么认为客户端中断连接.
                           list.Remove(sks);
                           ClientSocket ks = new ClientSocket();
                           sks.ClientDispose = true;//如果出现异常,标识客户端下线
                           sks.ex = new Exception("客户端无连接");
                           PushClient.Invoke(sks);//推送至UI

                       }
                   }
               }
           }
           catch (Exception skex)
           {
               ClientSocket sks = new ClientSocket();
               sks.ClientDispose = true;//如果出现异常,标识客户端退出
               sks.ex = skex;
               PushClient.Invoke(sks);//推送至UI
           }
       }
       #endregion
   }
}
