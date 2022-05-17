// ======================================================================
// 
//           Copyright (C) 2019-2020 湖南心莱信息科技有限公司
//           All rights reserved
// 
//           filename : ServiceCollectionRegistrar.cs
//           description :
// 
//           created by 雪雁 at  2019-06-14 11:22
//           开发文档: docs.xin-lai.com
//           公众号教程：magiccodes
//           QQ群：85318032（编程交流）
//           Blog：http://www.cnblogs.com/codelove/
//           Home：http://xin-lai.com
// 
// ======================================================================

using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Castle.Windsor.MsDependencyInjection;
using Magicodes.Pay.Volo.Abp.Tests.EF;

namespace Magicodes.Pay.Volo.Abp.Tests.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();
            WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);

            var builder = new DbContextOptionsBuilder<TestDbContext>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<TestDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            new TestDbContext(builder.Options).Database.EnsureCreated();
        }
    }
}