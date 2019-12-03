using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

/// <summary>
/// 进行xml文件的读写操作
/// </summary>
public static class XMLManager
{
    private static string volumeControlPath;

    private static float volume;
    public static void Init()
    {
        volumeControlPath = Application.dataPath + "/Resources/XML/AudioVolume.xml";
        ParseVolumeControl();
        Audio.SetVolume(volume);
    }
    
    /// <summary>
    /// 读取xml数据
    /// </summary>
    private static void ParseVolumeControl()
    {
        if (File.Exists(volumeControlPath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(volumeControlPath);
            XmlNode node = xmlDoc.SelectSingleNode("AudioVolume");
            try
            {
                volume = float.Parse(node.InnerText);
            }
            catch
            {
                throw new Exception("读取xml失败");
            }
        }
    }

    public static float Volume
    {
        get { return volume; }
        set
        {
            volume = value;
            if (File.Exists(volumeControlPath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(volumeControlPath);
                XmlNode node = xmlDoc.SelectSingleNode("AudioVolume");
                node.InnerText = value.ToString();
                xmlDoc.Save(volumeControlPath);
            }
        }
    }

}