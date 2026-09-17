using System.Text;
using System.Xml.Serialization;

namespace iTender.Integrator.Infrastructure.Integrations.CSD
{
    public static class CsdXmlSerializer
    {
        public static string Serialize<T>(T value)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new Utf8StringWriter();
            serializer.Serialize(writer, value);
            return writer.ToString();
        }

        public static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }

        private sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
