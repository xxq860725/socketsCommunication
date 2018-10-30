using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Sockets
{
    /// <summary>
    /// 定义一个Scoket初始化的抽象类,子类继承需要重写
    /// </summary>
    public abstract class abstractSockets
    {
        /// <summary>
        /// 初始化Socket方法
        /// </summary>
        /// <param name="ipAddress">ip终结点</param>
        /// <param name="port">端口</param>
        public abstract void InitSocket(IPAddress ipAddress, int port);

        public abstract void InitSocket(string ipAddress, int port);

        /// <summary>
        /// Socket启动方法
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Sockdet停止方法
        /// </summary>
        public abstract void Stop();

      
    }
}
