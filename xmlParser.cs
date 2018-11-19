using System.Xml;
namespace test_selenium
{
    struct Xmlelement
    {
        public int count;
        public string link;
        public Xmlelement(string link, int count)
        {
            this.count = count;
            this.link = link;
        }
    }

    class xmlParser
    {
        public xmlParser(string path)
        {
            textFile = path;
        }
        private string textFile = null;
        public Xmlelement GetLink(string attr)
        {
                Xmlelement element;
                XmlDocument linksDoc = new XmlDocument();
                linksDoc.Load(textFile);
                XmlElement xmlElement = linksDoc.DocumentElement;
                foreach (XmlNode nodeName in xmlElement)
                {
                    if (nodeName.Attributes.Count > 0 && nodeName.Attributes[0].Value == attr)
                    {
                        string link = nodeName["link"].InnerText;
                        int count = int.Parse(nodeName["count"].InnerText);
                        element = new Xmlelement(link, count);
                        return element;
                    }
                }
                element = new Xmlelement("", 0);
                return element;
        }
    }

}

