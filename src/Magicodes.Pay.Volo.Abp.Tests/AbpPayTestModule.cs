using Magicodes.Pay.Volo.Abp.Allinpay;
using Magicodes.Pay.Volo.Abp.Icbcpay;
using Magicodes.Pay.Volo.Abp.Tests.EF;
using Magicodes.Pay.Volo.Abp.Wxpay;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
//using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Json;
//using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace Magicodes.Pay.Volo.Abp.Tests;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(AbpAllinpayModule),
    typeof(AbpWxpayModule),
    typeof(AbpIcbcpayModule)
    )]
public class AbpPayTestModule : AbpModule
{
    private SqliteConnection _sqliteConnection;
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        //PreConfigure<AbpIdentityServerBuilderOptions>(options =>
        //{
        //    options.AddDeveloperSigningCredential = false;
        //});

        //PreConfigure<IIdentityServerBuilder>(identityServerBuilder =>
        //{
        //    identityServerBuilder.AddDeveloperSigningCredential(false, System.Guid.NewGuid().ToString());
        //});

        var accessorMock = Substitute.For<IHttpContextAccessor>();
        accessorMock.HttpContext= Substitute.For<HttpContext>();
        accessorMock.HttpContext.Connection.RemoteIpAddress = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
        accessorMock.HttpContext.Request.Headers["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.64 Safari/537.36 Edg/101.0.1210.47";
        context.Services.AddTransient<IHttpContextAccessor>((sp)=> accessorMock);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //Configure<AbpMultiTenancyOptions>(options =>
        //{
        //    options.IsEnabled = MultiTenancyConsts.IsEnabled;
        //});

        
        context.Services.AddAlwaysAllowAuthorization();
        context.Services.AddAbpDbContext<TestDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        ConfigureInMemorySqlite(context.Services);
    }

    private void ConfigureInMemorySqlite(IServiceCollection services)
    {
        _sqliteConnection = CreateDatabaseAndGetConnection();

        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(context =>
            {
                context.DbContextOptions.UseSqlite(_sqliteConnection);
            });
        });
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _sqliteConnection.Dispose();
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new TestDbContext(options))
        {
            context.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        return connection;
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}
