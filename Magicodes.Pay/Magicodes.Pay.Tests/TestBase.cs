using System;
using Abp.TestBase;
using Magicodes.Pay.Tests.EF;

namespace Magicodes.Pay.Tests
{
    public class TestBase : AbpIntegratedTestBase<TestModule>
    {
        protected void UsingDbContext(Action<TestDbContext> action)
        {
            using var context = LocalIocManager.Resolve<TestDbContext>();
            action(context);
            context.SaveChanges();
        }
    }
}