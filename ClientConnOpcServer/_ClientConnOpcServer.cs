
using EntityModel;
using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ClientConnOpcServer
{
    /****************************************************************************
    *Copyright (c) 2018  All Rights Reserved.
    *CLR版本： 4.0.30319.42000
    *机器名称：WIN-OCE2SQ21FJO
    *命名空间：ClientConnOpcServer
    *文件名：  ClientConnOpcServer
    *版本号：  V1.0.0.0
    *唯一标识：54c43575-c679-432f-b2e2-e6a132542f74
    *当前的用户域：WIN-OCE2SQ21FJO
    *创建人：  LeftYux
    *创建时间：2018-10-31 10:24:46

    *描述：利用Interop.OPCAutomation DLL连接opc服务器
    *
    *=====================================================================*/

    public delegate void PullOpcData();
    public  class _ClientConnOpcServer
    {
        private OPCItems opcItems;//客户端的id+键
        private OPCItem opcItem;
        private OPCGroups opcGroups;
        private OPCGroup opcGroup;
        private List<string> nodeName = new List<string>();
        public Dictionary<string, string> nodeValues = new Dictionary<string, string>();
        private List<int> itemHandleClient = new List<int>();
        private List<int> itemHandleServer = new List<int>();//服务端id
        public event PullOpcData Pull;

        /// <summary>
        /// 构造函数
        /// </summary>      
        public _ClientConnOpcServer() { }

        /// <summary>
        /// 创建连接Opc,默认连接超时5秒钟
        /// </summary>
        /// <param name="OpcHostName">服务器的主机名</param>
        /// <param name="OpcHostIP">服务器的Ip</param>
        /// <returns>自定义返回集合</returns>
        public Result<OPCServer> CreateConnection(string OpcHostName, string OpcHostIP)
        {
            return CreateConnection(OpcHostName, OpcHostIP,5000);
        }
            
        /// <summary>
        /// 创建连接Opc
        /// </summary>
        /// <param name="OpcHostName">服务器的主机名</param>
        /// <param name="OpcHostIP">服务器的ip</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>自定义返回集合</returns>
        public Result<OPCServer> CreateConnection(string OpcHostName, string OpcHostIP, int timeOut)
        {
            Result<OPCServer> result = new Result<OPCServer>();
            ManualResetEvent connectDone = new ManualResetEvent(false);
            StateObject state = new StateObject();
            OPCServer opc = null;
            try
            {
               opc = new OPCServer();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message +"初始化Opc服务器异常!";
                return result;
            }
            OperTimeOut tempTimeOut = new OperTimeOut()
            {
                WorkOPc = opc,
                DelayTime =timeOut
            };
            //开启线程验证连接是否超时
            ThreadPool.QueueUserWorkItem(new WaitCallback(VaildateTimeOut),tempTimeOut);

            try
            {
                state.WorkOpc =opc;
                state.WaitDone =connectDone;
                state.Ip = OpcHostIP;
                state.Name = OpcHostName;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThradConnecedOpc), state);
            }
            catch (Exception ex)
            {
                tempTimeOut.IsSuccessful = true;
                opc.Disconnect();
                connectDone.Close();
                result.Message = ex.Message;
                return result;
            }
            connectDone.WaitOne();
            connectDone.Close();
            tempTimeOut.IsSuccessful = true;
            //如果开启线程发生了异常
            if (state.IsError) 
            { 
                result.Message = state.ErrerMsg;
                if (opc != null) { opc.Disconnect(); }
            }
            if (state.OpcStatus)
            {
                result.Content = opc;
                result.IsSuccess = true;
                state.Clear();
                state = null;
            }
            return result;
        }


        /// <summary>
        /// 添加采集的Opc配置点
        /// </summary>
        /// <param name="Elements">配置点元素</param>
        /// <param name="opc">当前实例的OPc对象</param>
        /// <param name="opc">刷新率</param>
        public Result AddConnectionPoint(IList<string> Elements, OPCServer opc, int refreshRate)
        {
            if (Elements == null) { return Result.CreateSuccessResult();}
            Result result = new Result();
            if (opc == null) { result.IsSuccess = false; result.Message = "当前Opc对象为null"; return result; }
            //创建组并注册事件
            Result temp = settingsGroupAndpushElements(Elements, opc, refreshRate);
            result.IsSuccess = temp.IsSuccess;
            result.Message = temp.Message;
           
            return result;
        }

        private Dictionary<string, string> ClientConnOpcServer_Pull()
        {
            return nodeValues;
        }

        void opcGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                nodeValues[nodeName[Convert.ToInt32(ClientHandles.GetValue(i)) - 1]] = ItemValues.GetValue(i).ToString();
            }
        }

      
        #region 辅助线程的回调函数和私有函数
        /// <summary>
        /// 线程池验证超时的回调函数
        /// </summary>
        /// <param name="ir">异步操作的对象</param>
        private void VaildateTimeOut(object obj)
        {
            OperTimeOut tempObj = obj as OperTimeOut;
            if (tempObj != null)
            {
                while (!tempObj.IsSuccessful)
                {
                    Thread.Sleep(100);
                    if ((DateTime.Now - tempObj.StartTime).TotalMilliseconds > tempObj.DelayTime)
                    {
                        if (!tempObj.IsSuccessful)
                        {
                            //如果检测超时直接关闭释放
                            tempObj.WorkOPc.Disconnect();
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 线程池连接Opc服务器的回调函数
        /// </summary>
        /// <param name="obj">异步操作的对象</param>
        private void ThradConnecedOpc(object obj)
        {
            StateObject st = obj as StateObject;
            if (st != null)
            {
                try
                {
                    var tempOpc = st.WorkOpc;
                    tempOpc.Connect(st.Name, st.Ip);
                    if (tempOpc.ServerState == (int)OPCServerState.OPCRunning)
                    {
                        st.OpcStatus = true;
                    }
                    st.WaitDone.Set();
                }
                catch (Exception ex)
                {
                    st.IsError = true;
                    st.ErrerMsg = ex.Message;
                    st.OpcStatus = false;
                    st.WaitDone.Set();
                }
            }
        }

        /// <summary>
        /// 开启一个线程来添加节点到服务器
        /// </summary>
        /// <param name="obj">异步操作的对象</param>
        private void ThreadAddNodeElements(object obj)
        {
            StateObject st = obj as StateObject;
            if (st != null)
            {
                try
                {
                    nodeName.Clear();
                    nodeValues.Clear();
                    //添加节点元素，并与键值对匹配
                    foreach (var item in st.tempList) { this.nodeName.Add(item); this.nodeValues.Add(item, ""); }
                    for (int i = 0; i < st.tempList.Count; i++)
                    {
                        itemHandleClient.Add(i + 1);//客户端id
                        opcItem = opcItems.AddItem(st.tempList[i], i + 1);
                        itemHandleServer.Add(opcItem.ServerHandle);
                    }
                    st.WaitDone.Set();
                }
                catch (Exception ex)
                {
                    st.IsError = true;
                    st.ErrerMsg = ex.Message;
                    st.WaitDone.Set();
                }
            }
        }

        /// <summary>
        /// 设置Opc组和添加元素
        /// </summary>
        /// <param name="opc">当前操作的opc对象</param>
        /// <param name="refreshRate">刷新率</param>
        private Result settingsGroupAndpushElements(IList<string> Elements, OPCServer opc, int refreshRate)
        {
            Result result = new Result();
            ManualResetEvent connectDone = new ManualResetEvent(false);
            StateObject state = new StateObject();
            try
            {
                opcGroups = opc.OPCGroups;
                opcGroup = opcGroups.Add("Default Group");
                opc.OPCGroups.DefaultGroupIsActive = true;
                opc.OPCGroups.DefaultGroupDeadband = 0;
                opcGroup.IsActive = true;
                opcGroup.DeadBand = 0;
                opcGroup.UpdateRate = refreshRate;//设置刷新率
                opcItems = opcGroup.OPCItems;
                opcGroup.DataChange += opcGroup_DataChange;//注册读取事件

                #region 添加节点
                state.WaitDone = connectDone;
                state.tempList = Elements;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadAddNodeElements), state);
                connectDone.WaitOne();
                connectDone.Close();
                #endregion
            }
            catch (Exception ex)
            {
                result.Message = ex.Message + ",创建组失败!";
                result.IsSuccess = false;
                return result;
            }

            if (state.IsError)
            {
                result.IsSuccess = false;
                result.Message = state.ErrerMsg + ",添加元素到服务器失败!";
                return result;
            }
            result.IsSuccess = true;
            result.Message = "创建组并且添加元素成功!";
            return result;
        }
        #endregion
    }
}
