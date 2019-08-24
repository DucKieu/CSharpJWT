﻿namespace OAuthServer
{
    using CSharpJWT.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using CSharpJWT.Domain;
    using Microsoft.EntityFrameworkCore.Design;
    using System.IO;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddDbContext<CSharpJWTContext>(options => options.UseInMemoryDatabase("CSharpJWT"));

            services.AddCSharpJWTIdentity<CSharpJWTContext>();

            services.AddDbContext<CSharpJWTContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("CSharpJWTDB"),
               sql => sql.MigrationsAssembly("OAuthServer")));

            services.AddCSharpJWTAuthentication();

            services.AddTransient<SeedData>();

            services.AddCSharpJWTDistributedSqlServerCache(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("DistCache_ConnectionString");
                options.SchemaName = "dbo";
                options.TableName = "TestCache";
            });

            //services.AddCSharpJWTDistributedMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SeedData seedData)
        {
            seedData.SeedUser();

            //seedData.SeedClient();

            //seedData.SeedRole();

            //seedData.SeedUserRole();

            app.UseCSharpJWTServer();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}