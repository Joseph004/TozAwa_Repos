using System.IO;
using System.Xml.Linq;

namespace Tozawa.Language.Svc.XliffConverter
{
    public interface IXliffConverter
    {
        XDocument StreamToXDocument(byte[] xliffdata);
        XDocument StreamToXDocument(Stream xliffdata);
    }

    public class XliffConverter : IXliffConverter
    {
        public XDocument StreamToXDocument(byte[] xliffdata)
        {
            XDocument document;
            using (var stream = new MemoryStream(xliffdata))
            {
                using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8, true))
                {
                    document = XDocument.Load(reader);
                }
            }
            return document;
        }
        public XDocument StreamToXDocument(Stream xliffdata)
        {
            XDocument document;

            using (var reader = new StreamReader(xliffdata, System.Text.Encoding.UTF8, true))
            {
                document = XDocument.Load(reader);
            }
            return document;
        }
    }
}