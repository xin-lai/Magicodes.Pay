using Abp.Timing;
using Magicodes.Admin.Application.App.Payment;
using Magicodes.Admin.Application.App.Payment.Dto;
using Magicodes.Admin.Core.Storage;
using Shouldly;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Magicodes.Admin.Tests.Custom.App_Payment
{
    /// <summary>
    /// 
    /// </summary>
    public class Payment_Tests : TestBase
    {
        private readonly IPaymentAppService _paymentAppService;
        private readonly ITempFileCacheManager _tempFileCacheManager;


        public Payment_Tests()
        {
            LoginAsHostAdmin();
            _paymentAppService = Resolve<IPaymentAppService>();
            _tempFileCacheManager = Resolve<ITempFileCacheManager>();
            UsingDbContext(context =>
            {
                AppTest_DataBuild.ChargeProjectBuild(context);
                AppTest_DataBuild.ReductionProjectBuild(context);
                AppTest_DataBuild.StudentChargeStandardBuild(context);
                AppTest_DataBuild.SCSReductionProjectBuild(context);
                AppTest_DataBuild.SCSChargeProjectBuild(context);
                AppTest_DataBuild.PaymentLogBuild(context);
            });
        }

        [Fact(Skip = "待实现")]
        public async Task Should_Payment() => await _paymentAppService.Payment(null);

        [Fact]
        public async Task Should_GetChargeProject()
        {
            var result = await _paymentAppService.GetChargeProjects();
            result.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Should_GetPaymentRecord()
        {
            var result = await _paymentAppService.GetPaymentRecord(new GetPaymentRecordInput()
            {
                OpenId = "owWF25zT2BnOeQ68myWuQian7qHq"
            });

            result.Items.Count.ShouldBe(1);
        }

        [Fact(DisplayName = "小程序端电子收据导出")]
        public async Task Should_GetReceipt()
        {
            var result = await _paymentAppService.GetReceipt(new Abp.Application.Services.Dto.EntityDto<long>(1));

            _tempFileCacheManager.GetFile(result.FileToken).Length.ShouldBeGreaterThan(0);
            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Clock.Now.ToString("yyyyMMdd-HHmmss") + "小程序端电子收据导出.pdf"), _tempFileCacheManager.GetFile(result.FileToken));
        }
    }
}
