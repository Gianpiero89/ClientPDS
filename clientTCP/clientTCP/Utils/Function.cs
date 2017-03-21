using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Xml.Linq;
using System.Windows;

namespace clientTCP.classes
{
    class Function
    {
        public static void createXmlToSend(List<String> lstFilesFound, List<Utils.FileInfomation> list, Dictionary<String, Utils.FileInfomation> files)
        {
            int i = 0;
            foreach(String path in lstFilesFound)
            {
                
                list.ElementAt(i).Mod = File.GetLastWriteTime(@path);
                list.ElementAt(i).DIMENSION = new FileInfo(@path).Length;
                list.ElementAt(i).ComputeHash(@path);   
                files.Add(path, list.ElementAt(i));
                i += 1;
            }
        } 

        public static IPAddress checkIPAddress(string IPAdd)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(IPAdd, out ip))
                throw new InvalidOperationException("InvalidIPAddress");
            else
                return IPAddress.Parse(IPAdd);
        }

        public static String DictToXml(Dictionary<String, Utils.FileInfomation> files, String name)
        {
            XElement outElem = new XElement("Directory");
            outElem.Add(new XAttribute("backup_name", name));
            Dictionary<String, Utils.FileInfomation>.KeyCollection keys = files.Keys;          

            foreach (String key in keys)
            {
                XElement inner = new XElement("File");
                inner.Add(new XElement("Filename", files[key].FILENAME));
                inner.Add(new XElement("Path", files[key].PATH));
                inner.Add(new XElement("Dimension", files[key].DIMENSION));
                // formatto la data
                inner.Add(new XElement("Last_Modify",files[key].Mod.ToString(@"dd/MM/yyyy HH:mm:ss")));
                inner.Add(new XElement("MD5", files[key].Md5));
                outElem.Add(inner);
            }


            return XmlToString(outElem);          
        }

        private static String XmlToString(XElement root)
        {
            var reader = root.CreateReader();
            reader.MoveToContent();
            return reader.ReadOuterXml();
        }
      
        private static string encode(string path)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(path));
        }

        private static string decode(string path)
        {
            return System.Text.Encoding.ASCII.GetString(System.Convert.FromBase64String(path));
        }

        public static Boolean areEquals(List<Utils.FileInfomation> newFiles, List<Utils.FileInfomation> oldFiles)
        {
            Boolean choice = false;
            if (newFiles.Count == oldFiles.Count)
            {
                /*
                for (int i = 0; i < newFiles.Count; i++)
                {
                    if (newFiles[i].FILENAME != oldFiles[i].FILENAME 
                        || newFiles[i].DIMENSION != oldFiles[i].DIMENSION 
                        || System.Text.Encoding.ASCII.GetString(newFiles[i].Md5) != System.Text.Encoding.ASCII.GetString(oldFiles[i].Md5) 
                        || newFiles[i].Mod != oldFiles[i].Mod
                        || newFiles[i].PATH != oldFiles[i].PATH)
                    {
                        choice = false;
                        return choice;
                    } 
                 */
                return newFiles.Except(oldFiles).Any();
                //}
            }
            return choice;
        }


    }
}
