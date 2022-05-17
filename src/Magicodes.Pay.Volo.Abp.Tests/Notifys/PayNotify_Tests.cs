using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Json;
using Abp.Timing;
using Magicodes.Pay.Abp;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.TransactionLogs;
using Magicodes.Pay.Allinpay;
using Magicodes.Pay.Notify.Models;
using Magicodes.Pay.Tests.Callback;
using Magicodes.Pay.Volo.Abp.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Magicodes.Pay.Volo.Abp.Tests.Notifys
{
    /// <summary>
    /// 支付回调通知测试
    /// </summary>
    public class PayNotify_Tests : TestBase
    {
        private IPaymentManager paymentManager;
        private IConfiguration configuration;
        private string outTradeNo = "8AFD62BF-EA24-4B92-B015-F8CB7A86C315";

        public PayNotify_Tests()
        {
            paymentManager = Resolve<IPaymentManager>();
            configuration = Resolve<IConfiguration>();

            UsingDbContext(context => context.TransactionLogs.Add(new TransactionLog()
            {
                ClientIpAddress = "192.168.1.1",
                ClientName = "OS",
                CreationTime = Clock.Now,
                CustomData = new
                {
                    Name = "佩奇1号",
                    IdCard = "430122200010016014",
                    Phone = "18812340001",
                    RecommendCode = "00001",
                    Code = "CD001",
                    ReceiptCodes = "RC001",
                    ChargeProjectId = 1,
                    CreationTime = new DateTime(2019, 10, 1),
                    OpenId = "owWF25zT2BnOeQ68myWuQian7qHq"
                }.ToJsonString(),
                OutTradeNo = outTradeNo,
                Currency = new Currency(0.01m),
                Name = "学费",
                PayChannel = PayChannels.AliAppPay,
                Terminal = Terminals.Ipad,
                TransactionState = TransactionStates.NotPay,
                TenantId = null
            }));


        }

        [Fact(DisplayName = "通联支付回调测试")]
        public async Task Allinpay_ExecPayNotifyAsync_Test()
        {
            //Mock HttpRequest
            var httpRequestMock = Substitute.For<HttpRequest>();
            //伪造支付参数
            var dic = new Dictionary<string, string>() {
                { "acct", "ouiSX5NVuuNgcwRchQf - q4cK_vG4" },
                { "appid", configuration["Allinpay:AppId"] },
                { "chnlid", "213186760" },
                { "chnltrxid", "4200000447201910244661192735" },
                { "cmid", "305235533" },
                { "cusid", configuration["Allinpay:CusId"] },
                { "cusorderid", "ouiSX5NVuuNgcwRchQf - q4cK_vG4" },
                { "fee", "0" },
                //外部交易单号
                { "outtrxid", outTradeNo },
                { "paytime", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "signtype", "MD5" },
                { "termauthno", "CFT" },
                { "termrefnum", "4200000447201910244661192735" },
                { "termtraceno", "0" },
                //金额
                { "trxamt", "1" },
                { "trxcode", "VSP501" },
                { "trxdate", "20191024" },
                { "trxid", "121964420000012121" },
                { "trxreserved", "缴费支付" },
                { "trxstatus", "0000" },
            };

            //制造签名
            var sign = AllinpayUtil.SignParam(dic, configuration["Allinpay:AppKey"]);
            dic.Add("sign", sign);
            dic.Remove("key");

            //Mock HttpRequest 的表单参数
            var formDic = dic.ToDictionary(item => item.Key, item => new Microsoft.Extensions.Primitives.StringValues(item.Value));
            httpRequestMock.Form.Returns(new FormCollection(formDic, null));

            //执行支付回调
            await paymentManager.ExecPayNotifyAsync(new PayNotifyInput()
            {
                Provider = "allinpay",
                Request = httpRequestMock
            });

            //验证交易日志
            UsingDbContext(context =>
            {
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).TransactionState.ShouldBe(TransactionStates.Success);
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).PayTime.HasValue.ShouldBeTrue();
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).Exception.ShouldBeNull();
            });
        }

    }
}
