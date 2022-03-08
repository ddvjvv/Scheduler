using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Scheduler.Application;
using Scheduler.Application.Interfaces;
using Scheduler.Data;
using Scheduler.Data.Repositories;
using Scheduler.Services;
using System;

namespace Scheduler.API
{
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
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SchedulerAPI", Version = "v1" });
            });

            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(
                    Configuration.GetConnectionString("SchedulerDb"),
                    new SqlServerStorageOptions()
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }
                    );
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "priority", "default" };
            });


            services.AddDbContext<SchedulerContext>(builder =>
            {
                builder.UseSqlServer(
                    Configuration.GetConnectionString("SchedulerDb"),
                    option => option.MigrationsAssembly(typeof(SchedulerContext).Assembly.FullName));
            });

            services.AddTransient<IJobService, JobService>();
            services.AddTransient<IGenericRepository<SchedulerContext>, GenericRepository<SchedulerContext>>();
            services.AddTransient<IJobDefinitionRepository, JobDefinitionsRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchedulerAPI v1"));
            }
            app.UseHangfireDashboard("/hangfire");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
