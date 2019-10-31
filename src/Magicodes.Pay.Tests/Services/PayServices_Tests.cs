using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Json;
using Abp.UI;
using Magicodes.Pay.Abp.Services;
using Magicodes.Pay.Abp.Services.Dto;
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
                     CustomData = "{\"Name\":\"张6\",\"IdCard\":\"430626199811111111\",\"Phone\":\"18975061111\",\"Amount\":88,\"Remark\":\"\",\"OpenId\":\"ouiSX5OJ0OX-5W_1g4du5QZx-wsE\",\"PayChannel\":5}",
                     Key = "缴费支付",
                     OpenId = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                     OutTradeNo = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                     PayChannel = Abp.TransactionLogs.PayChannels.AllinWeChatMiniPay,
                     Subject = "缴费",
                     TotalAmount = 88
                 };
                 await payAppService.Pay(input);

                //交易日志校验
                UsingDbContext(context => context.TransactionLogs.Any(p => p.Currency.CurrencyValue == 88 && p.PayChannel == Pay.Abp.TransactionLogs.PayChannels.AllinWeChatMiniPay && p.TransactionState == Pay.Abp.TransactionLogs.TransactionStates.NotPay && p.OutTradeNo == input.OutTradeNo).ShouldBeTrue());

                //TODO:模拟回调并进一步测试
             });
        }
    }
}
