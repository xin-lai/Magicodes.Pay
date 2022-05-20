using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Magicodes.Pay.Volo.Abp.Tests.EF
{
    //[ConnectionStringName("Default")]
    public class TestDbContext : AbpDbContext<TestDbContext>
    {
        /// <summary>Constructor.</summary>
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<TransactionLog> TransactionLogs { get; set; }
    }

    public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            //ShopEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new TestDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Magicodes.Pay.Volo.Abp.Tests/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
