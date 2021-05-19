using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MvcApp01.Tools
{
  public class DataOperator
  {
    internal static string ByteToBase64String(byte[] src) => Convert.ToBase64String(src);
    internal static string CompressAndToBase64(string src)
    // internal static byte[] Compress(string src)
    {
      using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(src)))
      {
        using (var compressedStream = new MemoryStream())
        {
          {
            using (var compressor = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
            {
              uncompressedStream.CopyTo(compressor);
            }
            // return Encoding.UTF8.GetString(compressedStream.ToArray());
            return Convert.ToBase64String(compressedStream.ToArray())
            // .Replace('+', '-')        //「+」⇒「-」
            // .Replace('/', '_');       //「/」⇒「_」
            ;
            // return compressedStream.ToArray();
          }
        }
      }
    }
    internal static string FromBase64(string src) => Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64End(src)));
    static string PadBase64End(string src)
    {
      //FromBase64Stringでは、4の倍数の文字数しか受け付けない
      // なので、文字数が4の倍数になるようにPaddingする
      var main = src.TrimEnd('=')
      // .Replace('-', '+')                                //「-」⇒「+」
      // .Replace('_', '/')                            //「_」⇒「/」
      ;

      return main.PadRight(main.Length + 4 - (main.Length % 4), '=')
      ;
      // Console.WriteLine(pad);
      // return pad;
    }
    internal static string FromBase64AndDeCompress(string src)
    // internal static byte[] Compress(string src)
    {


      using (var compressedStream = new MemoryStream(Convert.FromBase64String(PadBase64End(src))))
      {
        using (var decompressor = new DeflateStream(compressedStream, CompressionMode.Decompress))
        {
          using (var decompressedStream = new MemoryStream())
          {
            {
              decompressor.CopyTo(decompressedStream);
            }
            // return Encoding.UTF8.GetString(compressedStream.ToArray());
            return Encoding.UTF8.GetString(decompressedStream.ToArray());
            // return compressedStream.ToArray();
          }
        }
      }
    }
    internal static string xdocToString(XDocument xdoc)
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