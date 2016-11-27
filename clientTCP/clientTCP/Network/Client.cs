using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace clientTCP.Network
{
    public class Client
    {
        private TcpClient client;

        public Client(TcpClient client)
        {
            this.client = client;
            // this.ns = client.GetStream();
        }


        public Boolean isConnected()
        {
            return this.client.Connected;
        }

        public void close()
        {
            //this.ns.Close();
            this.client.Close();
        }


        public void sendCommand(string command, NetworkStream ns)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }

        public void sendFileDimension(int dim, NetworkStream ns)
        {
            byte[] buf = BitConverter.GetBytes(dim);
            ns.Write(buf, 0, buf.Length);
            ns.Flush();

        }

        public void sendFile(String path, NetworkStream ns)
        {
            Byte[] data = File.ReadAllBytes(@path);
            ns.Write(data, 0, data.Length);
            ns.Flush();

        }


        public void sendData(string file, NetworkStream ns)
        {

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(file);
            ns.Write(data, 0, data.Length);
            ns.Flush();

        }

        public string reciveComand(NetworkStream ns)
        {
            Byte[] data = new Byte[7];
            String s;
            ns.Read(data, 0, 7);
            ns.Flush();
            return s = System.Text.Encoding.ASCII.GetString(data);
        }

        public int reciveDimension(NetworkStream ns)
        {
            byte[] buf = new byte[client.ReceiveBufferSize];  
            int len = ns.Read(buf, 0, client.ReceiveBufferSize);
            ns.Flush();
            return BitConverter.ToInt32(buf, 0);
        }

        public String reciveVersion(int dim, NetworkStream ns)
        {
            Byte[] bytes = new Byte[dim];

            ns.Read(bytes, 0, dim);
            return System.Text.Encoding.ASCII.GetString(bytes, 0, dim);
        }


        public String ReciveXMLData(int dim, NetworkStream ns)
        {

            // Buffer for reading data
            Byte[] bytes = new Byte[1024];
            int lenght = 1024;
            //StringBuilder sb = new StringBuilder();
            int i;
            Int32 numberOfTotalBytes = dim;
            Int32 byteRecivied = 0;
            String xml = "";


            if (dim < lenght)
            {
                ns.Read(bytes, 0, dim);
                return System.Text.Encoding.ASCII.GetString(bytes, 0, dim);
            }
            else
            {
                while ((i = ns.Read(bytes, 0, lenght)) != 0)
                {
                    xml += System.Text.Encoding.ASCII.GetString(bytes, 0, lenght);
                    byteRecivied += lenght;
                    if (numberOfTotalBytes - byteRecivied < lenght)
                    {
                        lenght = numberOfTotalBytes - byteRecivied;
                        ns.Read(bytes, 0, lenght);
                        xml += System.Text.Encoding.ASCII.GetString(bytes, 0, lenght);
                        return xml;
                        
                    }

                }
                
            }

            return "";
        }

        public void ReciveFile(String path, String name, int dim, NetworkStream ns)
        {

            // Buffer for reading data
            Byte[] bytes = new Byte[1024];
            int lenght = 1024;
            int i;
            Int32 numberOfTotalBytes = dim;
            Int32 byteRecivied = 0;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FileStream fs = new FileStream(path + @"\" + name, FileMode.OpenOrCreate);


            if (dim < lenght)
            {
                ns.Read(bytes, 0, dim);
                fs.Write(bytes, 0, dim);
                fs.Close();
                return;
            }
            else
            {
                while ((i = ns.Read(bytes, 0, lenght)) != 0)
                {
                    fs.Write(bytes, 0, lenght);
                    byteRecivied += lenght;
                    if (numberOfTotalBytes - byteRecivied < lenght)
                    {
                        lenght = numberOfTotalBytes - byteRecivied;
                        ns.Read(bytes, 0, lenght);
                        fs.Write(bytes, 0, lenght);
                        break;
                    }

                }
                fs.Close();
            }

            return;
        }



        public TcpClient CLIENT
        {
            get
            {
                return this.client;
            }
            set
            {
                this.client = value;
            }
        }
        
    }
}
