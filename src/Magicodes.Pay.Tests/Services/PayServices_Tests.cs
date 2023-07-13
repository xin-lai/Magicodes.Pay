using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.UI;
using Magicodes.Pay.Abp.Services;
using Magicodes.Pay.Abp.Services.Dto;
using Magicodes.Pay.Abp.TransactionLogs;
using Shouldly;
using Xunit;

namespace Magicodes.Pay.Tests.Services
{
    public class PayServices_Tests : TestBase
    {
        public readonly IPayAppService payAppService;

        public PayServices_Tests()
        {
            this.payAppService = Resolve<IPayAppService>();
        }

        [Fact]
        public async Task Pay_Test()
        {
            //请配置正确的支付参数后在移除异常校验
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
             {
                 var input = new PayInputBase()
                 {
                     Body = "缴费支付",
                     CustomData = "{\"Name\":\"张6\",\"IdCard\":\"430626199811111111\",\"Phone\":\"18975061111\",\"Amount\":0.01,\"Remark\":\"\",\"OpenId\":\"ouiSX5OJ0OX-5W_1g4du5QZx-wsE\",\"PayChannel\":7}",
                     Key = "缴费支付",
                     OpenId = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                     OutTradeNo = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                     PayChannel = PayChannels.AllinJsApiPay,
                     Subject = "缴费",
                     TotalAmount = 0.01m
                 };
                 await payAppService.Pay(input);

                 //交易日志校验
                 UsingDbContext(context => context.TransactionLogs.Any(p => p.Currency.CurrencyValue == 88 && p.PayChannel == PayChannels.AllinWeChatMiniPay && p.TransactionState == TransactionStates.NotPay && p.OutTradeNo == input.OutTradeNo).ShouldBeTrue());

             });
        }

        [Fact]
        public async Task Pay_icbc_Test()
        {
            //请配置正确的支付参数后在移除异常校验
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                 var input = new PayInputBase()
                {
                    Body = "张三-买菜",
                    CustomData = "{\"OutTradeNo\":\"c72cccce9af44f8cbef4bceb7900704d\",\"StudentId\":\"1161d8cc-260a-4510-970c-001347fc9c6e\",\"StudentName\":\"122\",\"SchoolId\":\"42bb8f05-7cb6-45af-aaa7-dee526ed8e7c\",\"Amount\":1.0,\"ProjectName\":\"test\",\"PayChannel\":9,\"OpenId\":\"ouiSX5OJ0OX-5W_1g4du5QZx-wsE\",\"PayerId\":null,\"PayerType\":null,\"RechargeType\":0}",
                    Key = "一脸通充值",
                    OpenId = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                    OutTradeNo = Guid.NewGuid().ToString(),
                    PayChannel = PayChannels.IcbcPay,
                    Subject = "买菜",
                    TotalAmount = 1
                };
                await payAppService.Pay(input);

                //交易日志校验
                //UsingDbContext(context => context.TransactionLogs.Any(p => p.Currency.CurrencyValue == 88 && p.PayChannel == PayChannels.AllinWeChatMiniPay && p.TransactionState == TransactionStates.NotPay && p.OutTradeNo == input.OutTradeNo).ShouldBeTrue());

            });
        }
    }
}
