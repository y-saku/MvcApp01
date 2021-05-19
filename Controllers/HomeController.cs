using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcApp01.Models;
using System.Security.Claims;
// using System.Web.Mvc;
//ASPCore用のライブラリを使用
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;
using System.Text;
using System.Xml;
using MvcApp01.Domains;
using System.Threading;
using Microsoft.Extensions.Primitives;
using System.IO;
using MvcApp01.Extensions;
// 文字列の場合
// SetStringとGetStringの拡張メソッドを使うのに必要
using Microsoft.AspNetCore.Http;
namespace MvcApp01.Controllers
{
  public class HomeController : Controller
  {
    const string id = "id6c1c178c166d486687be4aaf5e482730";
    // private const string issuer = @"https://sts.windows.net/005f8506-fa58-46c9-b7cc-6254a21fa596/";
    const string issuer = @"http://adapplicationregistry.onmicrosoft.com/customappsso/primary";

    const string timestamp = "2013-03-18T03:28:54.1839884Z";
    const string loginUrl = @"https://login.microsoftonline.com/005f8506-fa58-46c9-b7cc-6254a21fa596/saml2";
    const string loginReqTemplate = @"/app/MvcApp01/Datas/SAMLRequest.xml";
    const string certPath = @"/app/MvcApp01/SAML_TEST01_2.cer";

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }


    [AllowAnonymous]
    public IActionResult Index()
    {
      var saml = SAML.Factory
                               .LoginRequest(loginReqTemplate, issuer, timestamp)
                               //  .Create(id, issuer, timestamp)
                               .CompressAndToBase64() //圧縮とURLエンコードがあるのはクエリパラメータだから
                               .UrlEncode();
      //RelayStateにIdPから戻ってくる際のリダイレクトURLを設定
      //https://help.liferay.com/hc/ja/articles/360033738332-SAML%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%97%E3%81%9F%E8%AA%8D%E8%A8%BC
      var nextUrl = @"https://localhost:44377/Home/Privacy".ToBase64(Encoding.UTF8).UrlEncode(Encoding.UTF8);
      var url = $@"{loginUrl}?SAMLRequest={saml.XmlStr}&RelayState={nextUrl}";
      ViewBag.Message = url;
      // HttpContext.Session.SetString("AuthRequestID", saml.Id);
      TempData["AuthRequestID"] = saml.Id;

      // var cookie = new responseCookie("AuthRequestID", saml.Id.ToBase64(Encoding.UTF8).UrlEncode(Encoding.UTF8));
      // cookie.Secure = true;
      // cookie.HttpOnly = true;
      // Response.Cookies.Append(cookie);
      return Redirect(url);
    }
    [HttpPost]
    public IActionResult Account(string SAMLResponse, string RelayState)
    {
      VerifySAMLResponse(SAMLResponse);
      // Console.WriteLine(HttpContext.Session.GetString("AuthRequestID"));
      Console.WriteLine("Account:"+TempData["AuthRequestID"] as string);
      HttpContext.Session.Keys.ToList().ForEach(val => Console.WriteLine(val));
      // return View("Privacy");
      //★リダイレクト前にセッションにSAMLアサーションを追加
      // 初回ログイン後同一ドメイン内でしか遷移しないのでセッションでいい
      return Redirect(RelayState?.FromBase64(Encoding.UTF8));
      // return View();
      // return RedirectToAction("Privacy","Home");
      ;
    }

    [HttpPost]
    [HttpGet]
    public IActionResult Privacy()
    {
      // Console.WriteLine(HttpContext.Session.GetString("AuthRequestID"));
      // HttpContext.Session.Keys.ToList().ForEach(val => Console.WriteLine(val));
      //★セッションの検証を入れる
      // VerifySAMLResponse(SAMLResponse);
      Console.WriteLine("Privacy:"+TempData["AuthRequestID"] as string );
      return View();

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    void VerifySAMLResponse(string SAMLResponse)
    {

      if (String.IsNullOrEmpty(SAMLResponse)) return;
      // Console.WriteLine(SAMLResponse);
      Console.WriteLine("===============================================================");
      foreach (var item in Request.Headers) Console.WriteLine($"{item.Key}:{item.Value}");
      Console.WriteLine("===============================================================");
      foreach (var item in Request.Query) Console.WriteLine($"{item.Key}:{item.Value}");
      Console.WriteLine("===============================================================");
      using (var reader = new StreamReader(Request.Body))
        Console.WriteLine(reader.ReadToEnd());
      Console.WriteLine("===============================================================");

      var saml = SAML.Factory
                         .Create(SAMLResponse ?? string.Empty)
                         .FromBase64()
                         .Parse()
                         ;
      // Console.WriteLine(saml.XElSignature);
      // var principal = Thread.CurrentPrincipal;
      // Console.WriteLine(principal);

      //証明書の管理はおいおい考える

      var cert = new Verifier(certPath);
      // .FromBase64()
      // .Load();
      //★改ざん検証
      saml.XDoc?.Descendants(SAML.NameSpace.Assersion + "Assertion").Single().Add(new XElement("Add", new XText("改ざん検証")));
      ViewData["verify"] = $"電子署名検証：{cert.VerifySignature(saml.XDoc)}";
      ViewData["samlresponse"] = saml.XDoc;
      Console.WriteLine(saml.XDoc);


    }
  }
}
