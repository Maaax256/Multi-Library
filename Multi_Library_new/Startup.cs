using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.Configuration;
using Multi_Library.Models;
using Multi_Library.Interfaces;
using Multi_Library.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Multi_Library
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 104857600; // 100 MB, наприклад
            });
            services.AddMvc();
            services.AddTransient<IAlbum, MockAlbum>();
            services.AddTransient<IAuthorSong, MockAuthorSong>();
            //services.AddTransient<ICompilation, MockCompilation>();
            services.AddTransient<ICover, MockCover>();
            //services.AddTransient<ICoverCoverGallery, MockCoverCoverGallery>();
            services.AddTransient<ISong, MockSong>();
            //services.AddTransient<ISongSongPlaylist, MockSongSongPlaylist>();
            //services.AddTransient<IUserCompilation, MockUserCompilation>();
            services.AddTransient<IUserTable, MockUserTable>();
            services.AddTransient<IVideoClip, MockVideoClip>();
            //services.AddTransient<IVideoClipClipPlaylist, MockVideoClipClipPlaylist>();

            services.AddDbContext<Mul_Lib_Context>(); //(options =>
            //options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            //MvcOptions.EnableEndpoinRouting = false;
            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //.AddEntityFrameworkStores<mul_libContext>()
            //.AddDefaultTokenProviders();
            services.AddHttpContextAccessor();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                       options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Logout");
                       options.ExpireTimeSpan = System.TimeSpan.FromHours(10);
                   });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=LoginPage}/{id?}");
            });
            //app.UseMvcWithDefaultRoute();
            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });*/
        }
    }
}
