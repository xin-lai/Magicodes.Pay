// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : Currency.cs
//           description :金额
//   
//           created by 雪雁 at  2018-08-21 17:37
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Abp.Domain.Values;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Pay.Volo.Abp.TransactionLogs
{
    /// <summary>
    ///     金额（支持多种货币类型）
    /// </summary>
    [Owned]
    [Description("金额")]
    public class Currency
    {
        /// <summary>
        /// 货币符号
        /// </summary>
        [MaxLength(5)]
        public string Symbol { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">货币符号</param>
        /// <param name="currencyValue"></param>
        public Currency(decimal currencyValue, string symbol = "CNY")
        {
            Symbol = symbol;
            CurrencyValue = currencyValue;
        }

        /// <summary>
        /// 值
        /// </summary>
        public decimal CurrencyValue { get; internal set; }

        /// <summary>
        /// 是否为NULL
        /// </summary>
        [NotMapped]
        public bool IsNull => string.IsNullOrEmpty(Symbol);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => IsNull ? "0" : $"{CurrencyValue} {Symbol}";

        /// <summary>
        /// 转换金额
        /// </summary>
        /// <param name="currencyStr"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static Currency Parse(string currencyStr, string symbol = "CNY")
        {
            if (string.IsNullOrWhiteSpace(currencyStr))
            {
                return new Currency(0, null);
            }

            var digitPos = -1;
            var stringValue = currencyStr;

            while (digitPos < stringValue.Length
                   && !char.IsDigit(stringValue, ++digitPos))
            {
            }

            return digitPos < stringValue.Length ? new Currency(decimal.Parse(stringValue.Substring(digitPos)), symbol) : new Currency(0, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => IsNull ? 0 : ToString().GetHashCode();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var propertiesForCompare = GetPropertiesForCompare();
            return !((IEnumerable<PropertyInfo>)propertiesForCompare).Any() || ((IEnumerable<PropertyInfo>)propertiesForCompare).All(property => Equals(property.GetValue(this, null), property.GetValue(obj, null)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(Currency x, Currency y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(Currency x, Currency y) => !(x == y);

        private PropertyInfo[] GetPropertiesForCompare() => ((IEnumerable<PropertyInfo>)GetType().GetTypeInfo().GetProperties()).Where<PropertyInfo>(t => GetSingleAttributeOrDefault<IgnoreOnCompareAttribute>(t, null, true) == null).ToArray<PropertyInfo>();

        private static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default, bool inherit = true)
            where TAttribute : Attribute => memberInfo.IsDefined(typeof(TAttribute), inherit) ? memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First() : defaultValue;
    }
}