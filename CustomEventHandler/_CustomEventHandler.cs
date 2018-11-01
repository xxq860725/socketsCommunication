
using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomEventHandler
{
    /****************************************************************************
    *Copyright (c) 2018  All Rights Reserved.
    *CLR版本： 4.0.30319.42000
    *机器名称：WIN-OCE2SQ21FJO
    *命名空间：CustomEventHandler
    *文件名：  _CustomEventHandler
    *版本号：  V1.0.0.0
    *唯一标识：ff1f838b-8974-4905-ae47-91ce1ad866f4
    *当前的用户域：WIN-OCE2SQ21FJO
    *创建人：  LeftYux
    *创建时间：2018-11-01 21:15:02
    *描述：
    *
    *=====================================================================*/
    public class _CustomEventHandler
    {
        /// <summary>
        /// 推送到客户端的事件
        /// </summary>
        /// <param name="soc"></param>
         public delegate void TellServerIsConnecd(ClientSocket soc);

         /// <summary>
         /// 推送Opc事件
         /// </summary>
         /// <param name="resultList"></param>
         public delegate void TelegramRecievedEventHandler(Dictionary<string, string> resultList);

        /// <summary>
        /// 自定义泛型参数的事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
         public delegate void TellCustomEventHandler<T>(T t);
    }
}
