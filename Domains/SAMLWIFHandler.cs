using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
// using static MvcApp01.Domains.SAMLOwinHandler;

namespace MvcApp01.Domains
{
  public class SAMLWIFHandler
  {

    public static TokenData GetTokenObject()
    {
      var token = new TokenData();
      //現在のスレッドに対して割り当てられているプリンシパルを取得する
      // （プリンシパル＝権限主体とロールを結びつけたもの）
      var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
      token.IsAuthenticated = principal.Identity.IsAuthenticated;
      return token;
    }

    public class TokenData
    {
      public string NameIdentifier { get; set; }
      public string Naem { get; set; }
      public string Surname { get; set; }
      public string Role { get; set; }
      public string Email { get; set; }
      public string PetNmae { get; set; }
      public bool IsAuthenticated { get; set; }
    }
  }
}