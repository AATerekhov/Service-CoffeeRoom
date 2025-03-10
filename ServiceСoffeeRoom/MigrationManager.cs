using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace ServiceСoffeeRoom
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
        {
            var scope = host.Services.CreateScope();
            var appContext = scope.ServiceProvider.GetService<T>();
            appContext?.Database.Migrate();
            return host;
        }
        public static IServiceCollection AddNpgsqlSingleTon<TContext>(this IServiceCollection serviceCollection, string? connectionString, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null, Action<DbContextOptionsBuilder>? optionsAction = null) where TContext : DbContext
        {
            Action<DbContextOptionsBuilder> optionsAction2 = optionsAction;
            string connectionString2 = connectionString;
            Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction2 = npgsqlOptionsAction;
            return serviceCollection.AddDbContext<TContext>(delegate (IServiceProvider _, DbContextOptionsBuilder options)
            {
                optionsAction2?.Invoke(options);
                options.UseNpgsql(connectionString2, npgsqlOptionsAction2);
            }, optionsLifetime:ServiceLifetime.Singleton);
        }
    }
}
