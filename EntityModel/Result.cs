
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    /****************************************************************************
    *Copyright (c) 2018  All Rights Reserved.
    *CLR版本： 4.0.30319.42000
    *机器名称：WIN-OCE2SQ21FJO
    *命名空间：EntityModel
    *文件名：  Result
    *版本号：  V1.0.0.0
    *唯一标识：c9e3f549-3db9-42e1-a886-97681115b3ee
    *当前的用户域：WIN-OCE2SQ21FJO
    *创建人：  LeftYux
    *创建时间：2018-10-31 10:50:01

    *描述：操作结果的泛型类
    *
    *=====================================================================*/
    public class Result
    {
        /// <summary>
        /// 本次访问是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 具体的错误代码
        /// </summary>
        public int ErrorCode { get; set; }


        #region 创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息的静态方法

        public static Result CreateSuccessResult()
        {
            return new Result()
            {
                IsSuccess = true,
                ErrorCode = 0,
                Message = "可用Elements为null",
            };
        }
        /// <summary>
        /// 创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Result<T> CreateFailedResult<T>(Result result)
        {
            return new Result<T>()
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message,
            };
        }
        public static Result<T> CreateSuccessResult<T>(T value)
        {
            return new Result<T>()
            {
                IsSuccess = true,
                ErrorCode = 0,
                Message = "长度为0",
                Content = value
            };
        }
        /// <summary>
        /// 创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息
        /// </summary>
        /// <typeparam name="T1">目标数据类型一</typeparam>
        /// <typeparam name="T2">目标数据类型二</typeparam>
        /// <param name="result">之前的结果对象</param>
        /// <returns>带默认泛型对象的失败结果类</returns>
        public static Result<T1, T2> CreateFailedResult<T1, T2>(Result result)
        {
            return new Result<T1, T2>()
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message,
            };
        }


        /// <summary>
        /// 创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息
        /// </summary>
        /// <typeparam name="T1">目标数据类型一</typeparam>
        /// <typeparam name="T2">目标数据类型二</typeparam>
        /// <typeparam name="T3">目标数据类型三</typeparam>
        /// <param name="result">之前的结果对象</param>
        /// <returns>带默认泛型对象的失败结果类</returns>
        public static Result<T1, T2, T3> CreateFailedResult<T1, T2, T3>(Result result)
        {
            return new Result<T1, T2, T3>()
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message,
            };
        }


        /// <summary>
        /// 创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息
        /// </summary>
        /// <typeparam name="T1">目标数据类型一</typeparam>
        /// <typeparam name="T2">目标数据类型二</typeparam>
        /// <typeparam name="T3">目标数据类型三</typeparam>
        /// <typeparam name="T4">目标数据类型四</typeparam>
        /// <param name="result">之前的结果对象</param>
        /// <returns>带默认泛型对象的失败结果类</returns>
        public static Result<T1, T2, T3, T4> CreateFailedResult<T1, T2, T3, T4>(Result result)
        {
            return new Result<T1, T2, T3, T4>()
            {
                ErrorCode = result.ErrorCode,
                Message = result.Message,
            };
        }
        #endregion
    }

    public class Result<T> : Result
    {
        public Result()
            : base()
        {

        }
        /// <summary>
        ///未绑定的自定义泛型，需要用户构造
        /// </summary>
        public T Content { get; set; }
    }
    /*===================================================
   * 类名称: Result
   * 类描述:未绑定的操作结果泛型类，用户可以自带2个对象,构造该类型
   * 创建人: Yuxl
   * 创建时间: 2018/10/3 16:26:56
   * 修改人: 
   * 修改时间:
   * 版本： @version 1.0
   =====================================================*/
    public class Result<T1, T2> : Result
    {
        /// <summary>
        /// 未绑定的自定义泛型1，需要用户构造
        /// </summary>
        public T1 Content1 { get; set; }
        /// <summary>
        /// 未绑定的自定义泛型2，需要用户构造
        /// </summary>
        public T2 Content2 { get; set; }
    }
    /*===================================================
   * 类名称: Result
   * 类描述:未绑定的操作结果泛型类，用户可以自带3个对象,构造该类型
   * 创建人: Yuxl
   * 创建时间: 2018/10/3 16:26:56
   * 修改人: 
   * 修改时间:
   * 版本： @version 1.0
   =====================================================*/
    public class Result<T1, T2, T3> : Result
    {
        /// <summary>
        /// 自定义泛型1
        /// </summary>
        public T1 Content1 { get; set; }
        /// <summary>
        /// 自定义泛型2
        /// </summary>
        public T2 Content2 { get; set; }
        /// <summary>
        /// 自定义泛型3
        /// </summary>
        public T2 Content3 { get; set; }
    }
    /*===================================================
   * 类名称: Result
   * 类描述:未绑定的操作结果泛型类，用户可以自带4个对象,构造该类型
   * 创建人: Yuxl
   * 创建时间: 2018/10/3 16:26:56
   * 修改人: 
   * 修改时间:
   * 版本： @version 1.0
   =====================================================*/
    public class Result<T1, T2, T3, T4> : Result
    {
        public T1 Content1 { get; set; }
        public T2 Content2 { get; set; }
        public T2 Content3 { get; set; }
        public T4 Content4 { get; set; }
    }
}
