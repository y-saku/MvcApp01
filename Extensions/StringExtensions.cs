using System;
using System.Text;
using System.Web;

namespace MvcApp01.Extensions
{
  public static class StringExtensions
  {
    public static string UrlEncode(this string self, Encoding encoding)
    => HttpUtility.UrlEncode(self, encoding);

    public static string ToBase64(this string self, Encoding encoding)
    => Convert.ToBase64String(encoding.GetBytes(self))
              .Replace('+', '_')
              .Replace('/', '-')
              ;

    public static string FromBase64(this string self, Encoding encoding)
    => encoding.GetString(
        // Convert.FromBase64String(self)
        Convert.FromBase64String(self.PadBase64End()
                                     .Replace('_', '+')                                //「-」⇒「+」
                                     .Replace('-', '/')                            //「_」⇒「/」
        )
        );
    public static string PadBase64End(this string self)
    {
      //FromBase64Stringでは、4の倍数の文字数しか受け付けない
      // なので、文字数が4の倍数になるようにPaddingする
      var main = self.TrimEnd('=');
      return  (self.Length % 4 == 0)  
              ? self 
              : main.PadRight(main.Length + 4 - (main.Length % 4), '=')
      ;
    }
  }
}