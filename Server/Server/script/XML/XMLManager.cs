using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

/// <summary>
/// 进行xml文件的读写操作
/// </summary>
public static class XMLManager
{

    public static List<XMLMajor> majorInfo; 
    /// <summary>
    /// 用来读取XML文件中商品信息，在服务器启动时读取
    /// </summary>
    public static void ReadXML()
    {
        majorInfo = new List<XMLMajor>();

        XmlDocument doc = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        XmlReader reader = XmlReader.Create(@"../../script/XML/Data.xml",settings);
        doc.Load(reader);
        XmlNode xn = doc.SelectSingleNode("shop");
        XmlNodeList xnl = xn.ChildNodes;
        foreach (XmlNode xmlNode in xnl)
        {
            try
            {
                XMLMajor major = new XMLMajor();
                XmlNodeList xnl0 = xmlNode.ChildNodes;
                major.name = xnl0.Item(0).InnerText;
                int priceNoMajor = int.Parse(xnl0.Item(1).InnerText);
                major.priceNoMajor = priceNoMajor;
                int priceHaveMajor = int.Parse(xnl0.Item(2).InnerText);
                major.priceHaveMajor = priceHaveMajor;
                majorInfo.Add(major);
            }
            catch
            {
                throw new Exception("xml转换错误");
            }

            reader.Close();
        }
    }

}