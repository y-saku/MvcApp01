using System.Text;
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
      public static string ToSerialString(this XDocument xdoc)
    {
      var container = new StringBuilder();
      var setting = new XmlWriterSettings();
      using (var writer = XmlWriter.Create(container, setting))
      {
        xdoc.Save(writer);
      };
      return container.ToString();

    }
  }
}