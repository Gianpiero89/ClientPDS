using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows;

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
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
                ns.Write(data, 0, data.Length);
                ns.Flush();
               
            }
            catch( IOException ioe)
            {
                throw ioe;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                throw e;
            }
        }

        public void sendFileDimension(int dim, NetworkStream ns)
        {
            try
            {
                byte[] buf = new byte[15];
                BitConverter.GetBytes(dim).CopyTo(buf, 0);
                ns.Write(buf, 0, 15);
                ns.Flush();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                throw e;
            }

        }

        public void sendFile(String path, NetworkStream ns)
        {
            try
            {
                
                Byte[] data = File.ReadAllBytes(@path);
                ns.Write(data, 0, data.Length);
                ns.Flush();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                throw e;
            }

        }


        public void sendData(string file, NetworkStream ns)
        {
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(file);
                ns.Write(data, 0, data.Length);
                ns.Flush();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                throw e;
            }

        }

        public string reciveComand(NetworkStream ns)
        {
            try
            {
                if (client.Connected)
                {
                    Byte[] data = new Byte[7];
                    String s;
                    try
                    {

                        ns.Read(data, 0, 7);
                        ns.Flush();
                        return s = System.Text.Encoding.ASCII.GetString(data);

                    }
                    catch (IOException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            var sex = ex.InnerException as SocketException;
                            if (sex == null)
                            {
                                MessageBox.Show("An unknown exception occurred.");
                                return null;
                            }
                            else
                            {
                                switch (sex.SocketErrorCode)
                                {
                                    case SocketError.ConnectionReset:
                                        MessageBox.Show("A ConnectionReset SocketException occurred.");
                                        return null;
                                    default:
                                        MessageBox.Show("A SocketException occurred.");
                                        return null;

                                }
                                
                            }
                        }
                        else
                        {
                            MessageBox.Show("An IOException occurred.");
                            return null;
                           
                        }
                       
                       
                    }
                   
                }
                else
                {
                    throw new SocketException();
                }

            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                throw e;
            }
        }

        public int reciveDimension(NetworkStream ns)
        {
            try
            {
                byte[] buf = new byte[15];
                int len = ns.Read(buf, 0, 15);
                ns.Flush();
                return BitConverter.ToInt32(buf, 0);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                throw e;
            }
        }

        public String reciveVersion(int dim, NetworkStream ns)
        {
            try
            {
                Byte[] bytes = new Byte[dim];
                ns.Read(bytes, 0, dim);
                return System.Text.Encoding.ASCII.GetString(bytes, 0, dim);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
            
        }


        public String ReciveXMLData(int dim, NetworkStream ns)
        {
            try
            {
                // Buffer for reading data
                Byte[] bytes = new Byte[1];
                int lenght = 1;
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
                        Console.WriteLine("Sto leggendo");
                        if (numberOfTotalBytes - byteRecivied == 0)
                        { 
                            Console.WriteLine("Ho finito!");
                            return xml;
                        }
                        byteRecivied += i;
                        ns.Read(bytes, 0, 1);
                        xml += System.Text.Encoding.ASCII.GetString(bytes, 0, 1);
                    }

                    return xml;

                }
            }
             catch (Exception e)
            {
                Debug.Print(e.Message);
                throw e;
            }
        }

        public void ReciveFile(String path, String relative, String name, int dim, NetworkStream ns)
        {
            try
            {
                // Buffer for reading data
                Byte[] bytes = new Byte[512];
                int lenght = 512;
                int i;
                Int32 numberOfTotalBytes = dim;
                Int32 byteRecivied = 0;
                Console.WriteLine("Size : " + dim);
                if (!Directory.Exists(path + relative + @"\"))
                {
                    Directory.CreateDirectory(path + relative + @"\");
                }

                FileStream fs = new FileStream(path + relative + @"\" + name, FileMode.OpenOrCreate);

                if (dim < lenght)
                {
                    ns.Read(bytes, 0, dim);
                    fs.Write(bytes, 0, dim);
                    ns.Flush();
                    fs.Close();
                    return;
                }
                else
                {
                    //DataAvailable(ns, 5000);
                    while ((i = ns.Read(bytes, 0, lenght)) > 0)
                    {
                        fs.Write(bytes, 0, lenght);
                        byteRecivied += lenght;

                        if (numberOfTotalBytes - byteRecivied > 0 && numberOfTotalBytes - byteRecivied < lenght)
                        {
                            lenght = numberOfTotalBytes - byteRecivied;
                            Console.WriteLine("Lunghezza residua : " +lenght);
                            ns.Read(bytes, 0, lenght);
                            fs.Write(bytes, 0, lenght);
                            break;
                        }
                        if(numberOfTotalBytes - byteRecivied  == 0) break;
                    }
                    ns.Flush();
                    fs.Close();
                }

                return;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                throw e;
            }
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
