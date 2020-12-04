using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using PatagonianChallengeAPI.Dao;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Services;

namespace PatagonianChallengeAPI.Modules
{
    public static class SongsModule
    {
        public static void AddSongsManager(this IServiceCollection services, IWebHostEnvironment webHostingEnvironment)
        {
            services.AddSingleton(typeof(IConfiguration), service =>
            {
                var configPath = Path.Combine(webHostingEnvironment.ContentRootPath, "config");

                return new ConfigurationBuilder()
                    .SetBasePath(configPath)
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile("servicesettings.json", false, true)
                    .Build();
            });

            services.AddSingleton(typeof(ISessionFactory), service =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var config = serviceProvider.GetService<IConfiguration>();

                var connectionString = $"FullUri=file:{ Path.Combine(webHostingEnvironment.ContentRootPath, "database")}\\{config.GetValue<string>("databaseFileName")}";

                var _sessionFactory = Fluently.Configure()
                    .Database(SQLiteConfiguration.Standard
                        .ConnectionString(connectionString)
                        .ShowSql())
                    .Mappings(m => m
                        .FluentMappings.AddFromAssemblyOf<SongModel>())
                    .BuildSessionFactory();

                return _sessionFactory;
            });

            services.AddScoped(typeof(ISongService), typeof(SongService));
            services.AddScoped(typeof(ISongsDao), typeof(SongsDao));
        }
    }
}
