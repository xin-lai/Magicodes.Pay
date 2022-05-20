using System;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.Pay.Volo.Abp.Tests;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Magicodes.Pay.Volo.Abp.Tests.Callback
{
    public class PaymentCallback_Tests : AbpTestBase
    {
        private IPaymentManager paymentManager;
        private string outTradeNo = "AAAAAAAAAAAAAAAAAAAAAAA";
        private IRepository<TransactionLog, long> transactionLogsRepository;

        public PaymentCallback_Tests()
        {
            paymentManager = GetRequiredService<IPaymentManager>();
            transactionLogsRepository = GetRequiredService<IRepository<TransactionLog, long>>();
        }

        [Fact()]
        public async Task ExecuteCallback_Tests()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await transactionLogsRepository.InsertAsync(new TransactionLog()
                {
                    ClientIpAddress = "192.168.1.1",
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
                    Amount = 100,
                    Name = "学费",
                    PayChannel = PayChannels.AliAppPay,
                    Terminal = Terminals.Ipad,
                    TransactionState = TransactionStates.NotPay,
                }, true);
            });

            await paymentManager.ExecuteCallback("缴费支付", outTradeNo, "aaaa", 100);

            await WithUnitOfWorkAsync(async () =>
            {
                var log = await transactionLogsRepository.FirstAsync(p => p.OutTradeNo == outTradeNo);
                log.TransactionState.ShouldBe(TransactionStates.Success);
                log.PayTime.HasValue.ShouldBeTrue();
                log.Exception.ShouldBeNull();
            });

        }

        [Fact()]
        public async Task ExecuteCallbackError_Tests()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await transactionLogsRepository.InsertAsync(new TransactionLog()
                {
                    ClientIpAddress = "192.168.1.1",
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
                    Amount = 100,
                    Name = "学费",
                    PayChannel = PayChannels.AliAppPay,
                    Terminal = Terminals.Ipad,
                    TransactionState = TransactionStates.NotPay,
                }, true);
            });

            await Assert.ThrowsAsync<BusinessException>(async () => await paymentManager.ExecuteCallback("缴费支付异常测试", outTradeNo, "aaaa", 100));

            await WithUnitOfWorkAsync(async () =>
            {
                //验证状态
                var log = await transactionLogsRepository.FirstAsync(p => p.OutTradeNo == outTradeNo);
                log.TransactionState.ShouldBe(TransactionStates.PayError);

                //验证异常日志
                log.Exception.ShouldNotBeNullOrEmpty();
                log.Exception.ShouldContain("支付");
            });
        }

    }
}
