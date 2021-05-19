using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace MvcApp01
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
      services.AddControllersWithViews();
      //暫定的に許可
      // If using Kestrel:
      // Kestrel は、ASP.NET Core 向けのクロスプラットフォーム Web サーバーです。 Kestrel は、ASP.NET Core のプロジェクト テンプレートに既定で含まれ、有効になっている Web サーバーです。
      // https://docs.microsoft.com/ja-jp/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-5.0
      services.Configure<KestrelServerOptions>(options =>
      {
        options.AllowSynchronousIO = true;
      });
      // If using IIS:
      //https://stackoverflow.com/questions/47735133/asp-net-core-synchronous-operations-are-disallowed-call-writeasync-or-set-all
      services.Configure<IISServerOptions>(options =>
      {
        options.AllowSynchronousIO = true;
      });
      // // 分散キャッシュの指定(アプリのインスタンス内で有効)
      services.AddDistributedMemoryCache();
      // services.AddCookiePolicy();
      services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();
      services.AddRazorPages()
          .AddSessionStateTempDataProvider();
      services.AddSession(options =>
      {
        // options.IdleTimeout = TimeSpan.FromMinutes(60);
        // options.Cookie.IsEssential = true;
        // セッションクッキーの名前を変えるなら
        options.Cookie.Name = "session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;

      }
      );
      // services.AddMvc(options => options.EnableEndpointRouting = false);
      services.AddMvc(options => options.EnableEndpointRouting = false).AddSessionStateTempDataProvider();


      // Add the temp data provider
      // services.AddMvc();
      // services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
      //　cookieを使用
      // services.Configure<CookieTempDataProviderOptions>(options =>
      // {
      //     // TempDataのクッキーの名前を変えるなら
      //     options.Cookie.Name = "temp";
      // });

      // services.AddMvc(options => options.EnableEndpointRouting = false);
      // services.AddControllersWithViews();
      // services.AddRazorPages();
      // services.AddAuthentication().AddSchem;
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
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();


      // ADD
      // app.Run(async context =>
      // {
      //   // DefaultAuthenticateScheme causes User to be set
      //   var user = context.User;

      //   // This is what [Authorize] calls
      //   // var user = await context.AuthenticateAsync();

      //   // This is what [Authorize(ActiveAuthenticationSchemes = WsFederationDefaults.AuthenticationScheme)] calls
      //   // var user = await context.AuthenticateAsync(WsFederationDefaults.AuthenticationScheme);

      //   // Not authenticated
      //   if (user == null || !user.Identities.Any(identity => identity.IsAuthenticated))
      //   {
      //     // This is what [Authorize] calls
      //     await context.ChallengeAsync();

      //     // This is what [Authorize(ActiveAuthenticationSchemes = WsFederationDefaults.AuthenticationScheme)] calls
      //     // await context.ChallengeAsync(WsFederationDefaults.AuthenticationScheme);

      //     return;
      //   }
      // });
      // app.UseCookiePolicy(); //こいつが無いとセッション使えない！！
      app.UseSession();
      app.UseMvc();
      app.UseCookiePolicy(); //こいつが無いとセッション使えない！！
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });
      //       app.UseMvc(routes =>
      //  {
      //    routes.MapRoute(
      //               name: "default",
      //               template: "{controller=Home}/{action=Index}/{id?}");
      //  });
    }
  }
}
