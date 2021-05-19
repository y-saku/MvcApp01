using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using MvcApp01.Tools;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using MvcApp01.Extensions;
namespace MvcApp01.Domains
{
  public class Verifier
  {

    public string X509CertificateStr { get; private set; } = string.Empty;
    public string FilePath { get; } = string.Empty;
    public X509Certificate2 Certificate { get; private set; }
    public Verifier(string filePath)
    {
      this.FilePath = filePath;
      this.Certificate = new X509Certificate2(FilePath);
    }
    public Verifier FromBase64()
    {
      this.X509CertificateStr = DataOperator.FromBase64(this.X509CertificateStr);
      return this;
    }

    // public Certificate Load()
    // {
    //   // this.X509Certificate = new X509Certificate2(Encoding.UTF8.GetBytes(this.X509CertificateStr));
    //   this.X509Certificate = new X509Certificate2(this.FilePath);
    //   return this;
    // }
    public bool VerifySignature(XDocument xdoc)
    {

      // var xmlDoc = xdoc.ToXmlDocument();
      // var signedXml = new SignedXml(xmlDoc);

      // var nodeList = xmlDoc.GetElementsByTagName("Signature");
      // signedXml.LoadXml((XmlElement)nodeList[0]);
      var signedXml = new SignedXml(xdoc.ToXmlDocument());
      signedXml.LoadXml(xdoc.Descendants(SAML.NameSpace.Signature+"Signature").Single().ToXmlElement());

      // var signedXml = new SignedXml(xdoc.Element("Assertion").Element("Signature").ToXmlElement());
      return signedXml.CheckSignature(this.Certificate, true);
    }
    // public bool VerifySignature(XElement xel)
    // {
    //   var signedXml = new SignedXml();
    //   signedXml.LoadXml(xel.ToXmlElement());
    //   // var signedXml = new SignedXml(xdoc.Element("Assertion").Element("Signature").ToXmlElement());
    //   return signedXml.CheckSignature(this.Certificate, true);
    // }


  }
}