using EntityModel;
using SocketsCommunication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public partial class ServerSockets : Form
    {
        public ServerSockets()
        {
            InitializeComponent();
        }
        _SocketsCommunication socket = null;
        private void button1_Click(object sender, EventArgs e)
        {
            socket.ServerConnect("127.0.0.1", 4455);
            MessageBox.Show("开启监听成功"); 
            
        }

        private void ClientSocket_Load(object sender, EventArgs e)
        {
            Thread th = new Thread( new ParameterizedThreadStart((t) => {
                //注册通知事件
                _SocketsCommunication.PushClient += new CustomEventHandler._CustomEventHandler.TellCustomEventHandler<ClientSocket>(Recv);
                socket = new _SocketsCommunication();               
            }));
            th.Start();
          
        }

        private void Recv(ClientSocket soc)
        {
            this.Invoke(new ThreadStart(() =>
            {
                if (soc != null)
                {
                    if (soc.ClientDispose)
                    {
                        RefeshTextBox("客户端IP:" + soc.Ip + "下线", textBox1);
                    }
                    if (soc.NewClientFlag)
                    {
                        RefeshTextBox("客户端IP:" + soc.Ip + "上线", textBox1);
                    }

                }
            }));
        }

        private void RefeshTextBox(string msg, TextBox t)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, TextBox>(RefeshTextBox), msg, t);
                return;
            }
            t.AppendText(msg + Environment.NewLine);
        }
    }
}
