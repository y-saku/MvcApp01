using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
// using System.Xml.Linq.XName;
using System.Web;
using MvcApp01.Tools;
using MvcApp01.Extensions;
namespace MvcApp01.Domains
{
  public class SAML
  {
    public string Id { get; }
    public string Issuer { get; }
    public string Timestamp { get; }
    public string LoginUrl { get; }
    public string XmlStr { get; private set; } = string.Empty;
    public XDocument XDoc { get; private set; }
    public XElement XElSignature { get => this.XDoc.Descendants(NameSpace.Signature + "Signature").Single(); }
    // public byte[] XmlCompressedByteArray { get; }
    // public string XmlBase64Str { get; }
    // public string XmlUrlEncodedStr { get; }
    // public string LoginUrlAndParam { get => $@"{this.LoginUrl}?SAMLRequest={this.XmlStr}"; }
    SAML() { }
    SAML(string xmlStr)
    {
      this.XmlStr = xmlStr;
    }
    SAML(string issuer, string timestamp)
    {
      // Azure AD はこの属性を使用して、返される応答の InResponseTo 属性を設定します。 
      // ID の 1 文字目に数字を使用することはできないので、一般的な方法としては、GUID の文字列表現の前に "id" のような文字列を付加します。 
      // たとえば、 id6c1c178c166d486687be4aaf5e482730 は有効な ID です。
      this.Id = "id"+Guid.NewGuid().ToString("N");
      this.Issuer = issuer;
      this.Timestamp = timestamp;
      // this.LoginUrl = loginUrl;
      this.XDoc = xdocLogin2(this.Id, issuer, timestamp);
      this.XmlStr = DataOperator.xdocToString(this.XDoc);
      // this.XmlCompressedByteArray = DataOperator.Compress(this.XmlStr);
      // this.XmlBase64Str = Convert.ToBase64String(this.XmlCompressedByteArray);
      // this.XmlUrlEncodedStr = HttpUtility.UrlEncode(this.XmlBase64Str, Encoding.UTF8);
      // this.LoginUrlAndParam = $@"{this.LoginUrl}?SAMLRequest={this.XmlUrlEncodedStr}";
    }
    SAML(string path,  string issuer, string timestamp)
    {
      this.Id = "id"+Guid.NewGuid().ToString("N");
      this.Issuer = issuer;
      this.Timestamp = timestamp;
      this.XDoc = xdoc(path,this.Id, issuer, timestamp);
      this.XmlStr = DataOperator.xdocToString(this.XDoc);
    }
    public SAML Parse()
    {
      this.XDoc = XDocument.Parse(this.XmlStr);
      return this;
    }
    public SAML CompressAndToBase64()
    {
      this.XmlStr = DataOperator.CompressAndToBase64(this.XmlStr);
      return this;
    }
    public SAML FromBase64AndDeCompress()
    {
      this.XmlStr = DataOperator.FromBase64AndDeCompress(this.XmlStr);
      return this;
    }
    public SAML ToBase64()
    {
      this.XmlStr = XmlStr.ToBase64(Encoding.UTF8);
      // this.XmlStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.XmlStr));
      return this;
    }
    public SAML FromBase64()
    {
      this.XmlStr = DataOperator.FromBase64(this.XmlStr);
      // this.XmlStr = this.XmlStr.FromBase64(Encoding.UTF8);
      return this;
    }
    public SAML UrlEncode()
    {
      this.XmlStr = this.XmlStr.UrlEncode(Encoding.UTF8);
      // this.XmlStr = HttpUtility.UrlEncode(this.XmlStr, Encoding.UTF8);
      return this;
    }
    // public SAML UrlDecode()
    // {
    //   this.XmlStr = HttpUtility.UrlDecode(this.XmlStr, Encoding.UTF8);
    //   return this;
    // }
    XDocument xdocLogin2(string id, string issuer, string timestamp)
    {
      return new XDocument(
          new XDeclaration("1.0", "utf-8", null)
          // , new XComment("SAML")
          , new XElement(
             // https://docs.microsoft.com/ja-jp/dotnet/standard/linq/control-namespace-prefixes
             // NSMetadata + "samlp:AuthnRequest"
             NameSpace.Protocol + "AuthnRequest"
              , new XAttribute("xmlns", "urn:oasis:names:tc:SAML:2.0:metadata") //メタデータ名前空間
              , new XAttribute("ID", id)
              , new XAttribute("Version", "2.0") //SAMLバージョン
              , new XAttribute("IssueInstant", timestamp)
              , new XAttribute(XNamespace.Xmlns + "samlp", NameSpace.Protocol.NamespaceName)//プロトコル名前空間

          , new XElement(
              NameSpace.Assersion + "Issuer"
              // , new XAttribute(XNamespace.Xmlns, @"urn:oasis:names:tc:SAML:2.0:assertion")//アサーション名前空間
              , new XText(issuer)
          )
          )
      );

    }
    XDocument xdoc(string path, string id, string issuer, string timestamp)
    {
      var body = String.Format(File.ReadAllText(path, Encoding.UTF8), new string[]{ id, timestamp, issuer });
      Console.WriteLine(body);
      return XDocument.Parse(body);
    }
    XDocument xdocLogin(string id, string issuer, string timestamp)
    {
      var template 
      = @$"<samlp:AuthnRequest xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" ID=""{id}"" Version=""2.0"" IssueInstant=""{timestamp}""
            xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
            <Issuer xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"">{issuer}</Issuer>
          </samlp:AuthnRequest>";
      Console.WriteLine(template);
      return XDocument.Parse(template);
    }


    internal static class NameSpace
    {

      internal static XNamespace Protocol { get; } = "urn:oasis:names:tc:SAML:2.0:protocol";
      internal static XNamespace Assersion { get; } = "urn:oasis:names:tc:SAML:2.0:assertion";
      internal static XNamespace Signature { get; } = "http://www.w3.org/2000/09/xmldsig#";
    }

    public static class Factory
    {
        // const string loginReqFile = @"/app/MvcApp01/Datas/SAMLRequest.xml";

      // const string id = "id6c1c178c166d486687be4aaf5e482730";
      // // private const string issuer = @"https://sts.windows.net/005f8506-fa58-46c9-b7cc-6254a21fa596/";
      // const string issuer = @"http://adapplicationregistry.onmicrosoft.com/customappsso/primary";

      // const string timestamp = "2013-03-18T03:28:54.1839884Z";
      // const string loginUrl = @"https://login.microsoftonline.com/005f8506-fa58-46c9-b7cc-6254a21fa596/saml2";
      // public static SAMLAuthRequest Create() => new SAMLAuthRequest(id, issuer, timestamp, loginUrl);

      public static SAML LoginRequest(string issuer, string timestamp) => new SAML( issuer, timestamp);
      public static SAML LoginRequest(string path, string issuer, string timestamp) => new SAML(path,issuer, timestamp);
      public static SAML Create(string xmlStr) => new SAML(xmlStr);
    }
  }
}