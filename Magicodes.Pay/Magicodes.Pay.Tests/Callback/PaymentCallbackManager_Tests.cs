using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Json;
using Abp.Timing;
using Abp.UI;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.TransactionLogs;
using Shouldly;
using Xunit;

namespace Magicodes.Pay.Tests.Callback
{
    public class PaymentCallbackManager_Tests : TestBase
    {
        private IPaymentCallbackManager paymentCallbackManager;
        private string outTradeNo = "AAAAAAAAAAAAAAAAAAAAAAA";

        public PaymentCallbackManager_Tests()
        {
            paymentCallbackManager = Resolve<IPaymentCallbackManager>();

            UsingDbContext(context => context.TransactionLogs.Add(new TransactionLog()
            {
                ClientIpAddress = "192.168.1.1",
                ClientName = "OS",
                CreationTime = Clock.Now,
                CustomData = new
                {
                    Name = "佩奇",
                    IdCard = "430122200010016014",
                    Phone = "18812340001",
                    RecommendCode = "00001",
                    CreationTime = new DateTime(2019, 10, 1),
                    OpenId = "owWF25zT2BnOeQ68myWuQian7qHq"
                }.ToJsonString(),
                OutTradeNo = outTradeNo,
                Currency = new Currency(100),
                Name = "学费",
                PayChannel = PayChannels.AliPay,
                Terminal = Terminals.Ipad,
                TransactionState = TransactionStates.NotPay,
                TenantId = null
            }));
        }

        [Fact()]
        public async Task ExecuteCallback_Tests()
        {
            await paymentCallbackManager.ExecuteCallback("缴费支付", outTradeNo, "aaaa", 100);

            UsingDbContext(context =>
            {
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).TransactionState.ShouldBe(TransactionStates.Success);
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).PayTime.HasValue.ShouldBeTrue();
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).Exception.ShouldBeNull();
            });
        }

        [Fact()]
        public async Task ExecuteCallbackError_Tests()
        {
            await Assert.ThrowsAsync<UserFriendlyException>(async () => await paymentCallbackManager.ExecuteCallback("缴费支付异常测试", outTradeNo, "aaaa", 100));

            UsingDbContext(context =>
            {
                //验证状态
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).TransactionState.ShouldBe(TransactionStates.PayError);
                
                //验证异常日志
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).Exception.ShouldNotBeNullOrEmpty();
                context.TransactionLogs.First(p => p.OutTradeNo == outTradeNo).Exception.ShouldContain("支付报错");
            });
        }

    }
}
