using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using Magicodes.Pay.Abp.TransactionLogs;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Pay.Tests.EF
{
    public class TestDbContext : AbpDbContext
    {
        /// <summary>Constructor.</summary>
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<TransactionLog> TransactionLogs { get; set; }
    }
}
