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
using System.Xml.XPath;
namespace MvcApp01.Domains
{
  public class SAML
  {
    public XDocument XDoc { get; }
    public string XmlStr { get => this.XDoc?.ToSerialString(); }
    public string Id { get => this.XDoc.Root.Attribute("ID").ToString(); }

    public XElement XElSignature { get => this.XDoc.Descendants(NameSpace.Signature + "Signature").Single(); }
    SAML()
    {

    }
    SAML(XDocument xdoc)
    {
      this.XDoc = xdoc;
    }



    internal static class NameSpace
    {

      internal static XNamespace Protocol { get; } = "urn:oasis:names:tc:SAML:2.0:protocol";
      internal static XNamespace Assersion { get; } = "urn:oasis:names:tc:SAML:2.0:assertion";
      internal static XNamespace Signature { get; } = "http://www.w3.org/2000/09/xmldsig#";
    }

    public class Handler
    {
      const string timestamp = "2013-03-18T03:28:54.1839884Z";
      public SAML saml { get; }
      // Azure AD はこの属性を使用して、返される応答の InResponseTo 属性を設定します。 
      // ID の 1 文字目に数字を使用することはできないので、一般的な方法としては、GUID の文字列表現の前に "id" のような文字列を付加します。 
      // たとえば、 id6c1c178c166d486687be4aaf5e482730 は有効な ID です。      
      public string Id { get; private set; } = string.Empty;
      public string Issuer { get; private set; } = string.Empty;
      public string Timestamp { get; private set; } = string.Empty;
      public string XmlStr { get; private set; } = string.Empty;
      public Handler()
      {
        this.saml = new SAML();
      }

      public Handler AddIssuer(string issuer) { this.Issuer = issuer; return this; }
      public Handler AddXmlStr(string xmlStr) { this.XmlStr = xmlStr; return this; }
      public Handler SetIdFromGUID() { this.Id = "id" + Guid.NewGuid().ToString("N"); return this; }

      public Handler SetTimestamp() { this.Timestamp = timestamp; return this; }
      public SAML BuildLoginRequest() => new SAML(xdocLogin(this.Id, this.Issuer, this.Timestamp));
      public SAML Parse() => new SAML(XDocument.Parse(this.XmlStr));
      public static SAML LoginRequest(string issuer, string timestamp) => new SAML(xdocLogin("id" + Guid.NewGuid().ToString("N"), issuer, timestamp));
      // public static SAML LoginRequest(string path, string issuer, string timestamp) => new SAML(path, issuer, timestamp);
      public static SAML Parse(string xmlStr) => new SAML(XDocument.Parse(xmlStr));



      static XDocument xdocLogin(string id, string issuer, string timestamp)
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
      static XDocument xdocLogin2(string path, string id, string issuer, string timestamp)
      {
        var body = String.Format(File.ReadAllText(path, Encoding.UTF8), new string[] { id, timestamp, issuer });
        Console.WriteLine(body);
        return XDocument.Parse(body);
      }
      static XDocument xdocLogin3(string id, string issuer, string timestamp)
      {
        var template
        = @$"<samlp:AuthnRequest xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" ID=""{id}"" Version=""2.0"" IssueInstant=""{timestamp}""
            xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
            <Issuer xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"">{issuer}</Issuer>
          </samlp:AuthnRequest>";
        Console.WriteLine(template);
        return XDocument.Parse(template);
      }
    }
  }
}