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

namespace Magicodes.Admin.Tests.Custom.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentCallbackManager_Tests : AppTestBase
    {
        private IPaymentCallbackManager paymentCallbackManager;
        public PaymentCallbackManager_Tests()
        {
            paymentCallbackManager = Resolve<IPaymentCallbackManager>();
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
                        OutTradeNo = "AAAAAAAAAAAAAAAAAAAAAAA",
                        CreationTime = new DateTime(2019, 10, 1),
                        OpenId = "owWF25zT2BnOeQ68myWuQian7qHq"
                    }.ToJsonString(),
                    OutTradeNo = "AAAAAAAAAAAAAAAAAAAAAAA",
                    Currency = new Currency(100),
                    Name = "学费",
                    PayChannel = PayChannels.AliPay,
                    Terminal = Terminals.Ipad,
                    TransactionState = TransactionStates.NotPay,
                    TenantId = GetCurrentTenant()?.Id
                });
            });


        }

        /// <summary>
        /// 支付回调逻辑正常验证
        /// </summary>
        /// <returns></returns>
        [Fact()]
        public async Task ExecuteCallback_Tests()
        {
            var no = "AAAAAAAAAAAAAAAAAAAAAAA";
            await paymentCallbackManager.ExecuteCallback("缴费支付", no, "1111111", 10000);
            UsingDbContext(context =>
            {
                //var list = context.TransactionLogs.ToList();
                //var logs = context.PaymentLogs.ToList();
                var log = context.PaymentLogs.FirstOrDefault(p => p.OutTradeNo == no);
                log.ShouldNotBeNull();

                context.TransactionLogs.First(p => p.OutTradeNo == no).TransactionState.ShouldBe(TransactionStates.Success);
                context.TransactionLogs.First(p => p.OutTradeNo == no).PayTime.HasValue.ShouldBeTrue();
                context.TransactionLogs.First(p => p.OutTradeNo == no).Exception.ShouldBeNull();
            });
        }

        /// <summary>
        /// 支付回调逻辑异常验证
        /// </summary>
        /// <returns></returns>
        [Fact()]
        public async Task ExecuteCallbackError_Tests()
        {

            var container = LocalIocManager.IocContainer;
            container.Register(Component.For<IPaymentCallbackManager>().Named("TestPaymentCallbackManager").ImplementedBy<TestPaymentCallbackManager>().LifestyleTransient());
            paymentCallbackManager = container.Resolve<IPaymentCallbackManager>("TestPaymentCallbackManager");

            var no = "AAAAAAAAAAAAAAAAAAAAAAA";
            var exception = await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await paymentCallbackManager.ExecuteCallback("缴费支付", no, "111111", 10000);
            });

            UsingDbContext(context =>
            {
                var list = context.TransactionLogs.ToList();
                context.TransactionLogs.First(p => p.OutTradeNo == no).TransactionState.ShouldBe(TransactionStates.PayError);
                context.TransactionLogs.First(p => p.OutTradeNo == no).Exception.ShouldNotBeNullOrEmpty();
            });
        }
    }
}
