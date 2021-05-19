using System.Xml;
using System.Xml.Linq;

namespace MvcApp01.Extensions
{
  public static class XmlExtensions
  {
    public static XmlDocument ToXmlDocument(this XDocument xdoc)
    {
      var xmlDoc = new XmlDocument();
      using (var reader = xdoc.CreateReader())
      {
        xmlDoc.Load(reader);
      }
      return xmlDoc;
    }

    public static XmlElement ToXmlElement(this XElement xel)
    {
      var xmlDoc = new XmlDocument();
      using (var reader = xel.CreateReader())
      {
        xmlDoc.Load(reader);
      }
      return xmlDoc.DocumentElement;
    }
  }
}