using CloverleafTrack.Data;
using CloverleafTrack.Managers;
using CloverleafTrack.Options;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CloverleafTrack
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
            //services.AddMiniProfiler(options =>
            //{
            //    options.ColorScheme = StackExchange.Profiling.ColorScheme.Dark;
            //}).AddEntityFramework();

            services.Configure<CurrentSeasonOptions>(Configuration.GetSection(CurrentSeasonOptions.Name));

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddDbContext<CloverleafTrackDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CloverleafTrackDataContext")));

            services.AddTransient<IAthleteManager, AthleteManager>();
            services.AddTransient<IMeetManager, MeetManager>();
            services.AddTransient<IPerformanceManager, PerformanceManager>();
            services.AddTransient<ISeasonManager, SeasonManager>();
            services.AddTransient<ITrackEventManager, TrackEventManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseMiniProfiler();
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllers();
            });
        }
    }
}
