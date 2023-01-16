using System.Linq;
using System.Threading.Tasks;
using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Volo.Abp.Services.Dto;
using Magicodes.Pay.Volo.Abp.Tests;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Magicodes.Pay.Wxpay.Helper;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Magicodes.Pay.Volo.Abp.Tests.Services
{
    public class PayServices_Tests : AbpTestBase
    {
        public readonly IPayAppService payAppService;
        private IRepository<TransactionLogs.TransactionLog, long> transactionLogsRepository;

        public PayServices_Tests()
        {
            payAppService = GetRequiredService<IPayAppService>();
            transactionLogsRepository = GetRequiredService<IRepository<TransactionLogs.TransactionLog, long>>();
        }

        [Fact]
        public async Task Pay_AllinJsApiPay_Test()
        {
            
            //请配置正确的支付参数后在移除异常校验
            await Assert.ThrowsAsync<BusinessException>(async () =>
             {
                 var input = new PayInputBase()
                 {
                     Body = "缴费支付",
                     CustomData = "{\"Name\":\"张6\",\"IdCard\":\"430626199811111111\",\"Phone\":\"18975061111\",\"Amount\":0.01,\"Remark\":\"\",\"OpenId\":\"ouiSX5OJ0OX-5W_1g4du5QZx-wsE\",\"PayChannel\":7}",
                     Key = "缴费支付",
                     OpenId = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                     OutTradeNo = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                     PayChannel = Abp.TransactionLogs.PayChannels.AllinJsApiPay,
                     Subject = "缴费",
                     TotalAmount = 0.01m
                 };

                 //var data = XmlHelper.SerializeObjectWithoutNamespace(input);
                 await payAppService.Pay(input);

                 await WithUnitOfWorkAsync(async () =>
                 {
                     //验证状态
                     var result = await transactionLogsRepository.AnyAsync(p => p.Amount == 88 && p.PayChannel == PayChannels.AllinWeChatMiniPay && p.TransactionState == TransactionStates.NotPay && p.OutTradeNo == input.OutTradeNo);
                     result.ShouldBeTrue();
                 });

             });
        }

        [Fact]
        public async Task Pay_WeChatMiniProgram_Test()
        {
            var input = new PayInputBase()
            {
                Body = "缴费支付",
                CustomData = "{\"Name\":\"张6\",\"IdCard\":\"430626199811111111\",\"Phone\":\"18975061111\",\"Amount\":0.01,\"Remark\":\"\",\"OpenId\":\"ouiSX5OJ0OX-5W_1g4du5QZx-wsE\",\"PayChannel\":7}",
                Key = "缴费支付",
                OpenId = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                OutTradeNo = "ouiSX5OJ0OX-5W_1g4du5QZx-wsE",
                PayChannel = Abp.TransactionLogs.PayChannels.WeChatMiniProgram,
                Subject = "缴费",
                TotalAmount = 0.01m
            };
            await payAppService.Pay(input);

            await WithUnitOfWorkAsync(async () =>
            {
                //验证状态
                var result = await transactionLogsRepository.AnyAsync(p => p.Amount == 88 && p.PayChannel == PayChannels.WeChatMiniProgram && p.TransactionState == TransactionStates.NotPay && p.OutTradeNo == input.OutTradeNo);
                result.ShouldBeTrue();
            });

        }
    }
}
