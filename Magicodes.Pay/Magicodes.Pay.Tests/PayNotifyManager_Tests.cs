using Abp.Json;
using Abp.Timing;
using Magicodes.Admin.Application.Core.Payments.PaymentCallbacks;
using Magicodes.Admin.Core.Custom.Logs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Shouldly;
using Abp.Dependency;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Magicodes.Admin.Application.Core.Payments;
using System.Net.Http;
using NSubstitute;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using Magicodes.Admin.Core.Configuration;
using Magicodes.Admin.Tests.Configuration;
using Magicodes.Allinpay;

namespace Magicodes.Admin.Tests.Custom.Payments
{
    /// <summary>
    /// 支付回调通知测试
    /// </summary>
    public class PayNotifyManager_Tests : AppTestBase
    {
        private IPaymentCallbackManager paymentCallbackManager;
        private IPayNotifyManager payNotifyManager;
        private IAppConfigurationAccessor testAppConfigurationAccessor;
        private string outTradeNo = "8AFD62BF-EA24-4B92-B015-F8CB7A86C315";

        public PayNotifyManager_Tests()
        {
            paymentCallbackManager = Resolve<IPaymentCallbackManager>();
            payNotifyManager = Resolve<IPayNotifyManager>();
            testAppConfigurationAccessor = Resolve<IAppConfigurationAccessor>();

            UsingDbContext(context =>
            {
                context.TransactionLogs.Add(new Core.Custom.Logs.TransactionLog()
                {
                    ClientIpAddress = "192.168.1.1",
                    ClientName = "OS",
                    CreationTime = Clock.Now,
                    CustomData = new PaymentLog()
                    {
                        Name = "佩奇1号",
                        IdCard = "430122200010016014",
                        Phone = "18812340001",
                        GradeId = new Guid("00000000-0000-0000-0001-000000000001"),
                        MajorId = new Guid("00000000-0000-0000-0002-000000000001"),
                        SchoolId = new Guid("00000000-0000-0000-0003-000000000001"),
                        RecommendCode = "00001",
                        Amount = 100,
                        Code = "CD001",
                        ReceiptCodes = "RC001",
                        ChargeProjectId = 1,
                        CreationTime = new DateTime(2019, 10, 1),
                        OpenId = "owWF25zT2BnOeQ68myWuQian7qHq"
                    }.ToJsonString(),
                    OutTradeNo = outTradeNo,
                    Currency = new Currency(100),
                    Name = "学费",
                    PayChannel = PayChannels.AliPay,
                    Terminal = Terminals.Ipad,
                    TransactionState = TransactionStates.NotPay,
                    TenantId = GetCurrentTenant()?.Id
                });
            });


        }

        [Fact(DisplayName = "通联支付回调测试")]
        public async Task Allinpay_ExecPayNotifyAsync_Test()
        {
            //Mock HttpRequest
            var httpRequestMock = Substitute.For<HttpRequest>();
            //伪造支付参数
            var dic = new Dictionary<string, string>() {
                { "acct", "ouiSX5NVuuNgcwRchQf - q4cK_vG4" },
                { "appid", testAppConfigurationAccessor.Configuration["Allinpay:AppId"] },
                { "chnlid", "213186760" },
                { "chnltrxid", "4200000447201910244661192735" },
                { "cmid", "305235533" },
                { "cusid", testAppConfigurationAccessor.Configuration["Allinpay:CusId"] },
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
                { "trxamt", "100" },
                { "trxcode", "VSP501" },
                { "trxdate", "20191024" },
                { "trxid", "121964420000012121" },
                { "trxreserved", "缴费支付" },
                { "trxstatus", "0000" },
            };

            //获取签名
            var sign = AllinpayUtil.SignParam(dic, testAppConfigurationAccessor.Configuration["Allinpay:AppKey"]);
            dic.Add("sign", sign);
            dic.Remove("key");

            //Mock HttpRequest 的表单参数
            var formDic = dic.ToDictionary(item => item.Key, item => new Microsoft.Extensions.Primitives.StringValues(item.Value));
            httpRequestMock.Form.Returns(new FormCollection(formDic, null));

            //执行支付回调
            await payNotifyManager.ExecPayNotifyAsync(new PayNotify.Models.PayNotifyInput()
            {
                Provider = "allinpay",
                Request = httpRequestMock
            });

            //验证交易日志
            UsingDbContext(context =>
            {
                var log = context.PaymentLogs.FirstOrDefault(p => p.OutTradeNo == outTradeNo);
                log.ShouldNotBeNull();

                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).TransactionState.ShouldBe(TransactionStates.Success);
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).PayTime.HasValue.ShouldBeTrue();
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).Exception.ShouldBeNull();
            });
        }

    }
}
