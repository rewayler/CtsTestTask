using System.Xml;

namespace CtsTestTask
{
    public class SchemaValidator
    {
        public void Validate(string path, string xsd)
        {
            using (XmlTextReader xsdReader = new XmlTextReader(xsd))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(path))
                {
                    XmlValidatingReader validator = new XmlValidatingReader(xmlReader);
                    validator.ValidationType = ValidationType.Schema;
                    validator.Schemas.Add(null, xsdReader);
                    while (validator.Read()) ;
                    xmlReader.Close();
                }
                xsdReader.Close();
            }
        }
    }
}