using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Sockets.InterfaceSocket
{
   public  interface IClientSend
    {

       /// <summary>
       /// 向客户端发送一条消息
       /// </summary>
       /// <param name="ip"></param>
       /// <param name="SendData"></param>
       void SendToClient(IPEndPoint ip, string SendData);

       /// <summary>
       /// 将新连接的客户端加入到队列
       /// </summary>
       /// <param name="sk"></param>
       void AddClientQueue(tempSockets qts);
    }
}
