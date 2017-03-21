using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace clientTCP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FolderBackup : Window
    {
        public Window parent { get; set; }
        //private TcpClient client;
        private Network.Client client;
        private List<String> lstFilesFound;
        private List<Utils.FileInfomation> info;
        private Dictionary<String, Utils.FileInfomation> files;
        private Thread t;
        private string auth = "";
        private static string backupFolder = "Backup";
        private volatile Boolean _isRunning = false;
        private String absolutePath;
        private String relativePath;
        private String xml;
        private List<Utils.CheckBackup> backupList;
        FileSystemWatcher watcher;


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (client != null)
            {
                try
                {
                    client.sendCommand("++CLOSE", client.CLIENT.GetStream());
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message);
                    _isRunning = false;
                    MessageBox.Show("Errore di rete, Server offline");
                    //this.Hide();
                    this.Close();
                }
            }

            base.OnClosing(e);
        }

        public FolderBackup(string cnt)
        {
            this.auth = cnt;
            lstFilesFound = new List<string>();
            files = new Dictionary<string, Utils.FileInfomation>();
            info = new List<Utils.FileInfomation>();
            backupList = new List<Utils.CheckBackup>();
            
            InitializeComponent();

            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            try
            {
                // ip e porta del server fissati 

                TcpClient tmp = new TcpClient();
                //TcpClient control = new TcpClient();
                _isRunning = true;
                // ip e porta del server fissati 
                tmp.Connect(classes.Function.checkIPAddress("127.0.0.1"), Int16.Parse("3000"));
                client = new Network.Client(tmp);
                string ccc = client.reciveComand(client.CLIENT.GetStream());
                if (ccc.Equals("+++OPEN"))
                { 
                    clientSend("+++AUTH");
                    string cmd = client.reciveComand(client.CLIENT.GetStream());
                    Console.WriteLine(cmd);
                    if (cmd.Equals("+++++OK"))
                    {
                        client.sendFileDimension(this.auth.Length, client.CLIENT.GetStream());
                        cmd = client.reciveComand(client.CLIENT.GetStream());
                        if (cmd.Equals("+++++OK"))
                        {
                            Console.WriteLine(cmd);
                            client.sendData(this.auth, client.CLIENT.GetStream());
                        }
                    }
                    
                    clientSend(String.Format("+++LIST"));
                    dir.Dispatcher.Invoke(new Action(() =>
                    {
                        dir.Text += "Client connected to server!\n";
                    }), DispatcherPriority.ContextIdle);
                    clientRecive();
                  
                }

            }
            catch (SocketException e1)
            {
                MessageBox.Show("Server non ragiungibile!");
                this.client = null;
                return;

            }
            catch (FormatException e2)
            {
                MessageBox.Show("Porta errata!");
                this.client = null;
                return;

            }
            catch (InvalidOperationException e3)
            {
                MessageBox.Show("Indirizzo IP errato!");
                this.client = null;
                return;

            }
            catch (Exception e)
            {
                MessageBox.Show("Connessione col server persa!");
                return;
            }

        }

        public void DirSearch(String path)
        {
            try
            {
                //MessageBox.Show("Sono qui");
                foreach (string f in Directory.GetFiles(path, "*"))
                {
                    //MessageBox.Show(relativePath + " : " + f.LastIndexOf(relativePath).ToString() + "  last \\" + f.LastIndexOf(@"\").ToString());
                    lstFilesFound.Add(f);
                    int resto = Math.Abs(f.LastIndexOf(@"\") - f.LastIndexOf(relativePath));
                    
                    String tmp = f.Substring(f.LastIndexOf(relativePath) - 1, resto + 1);
                    
                    info.Add(new Utils.FileInfomation(tmp, f.Substring(f.LastIndexOf(@"\") + 1)));
                }
                foreach (string d in Directory.GetDirectories(path))
                {
                    //MessageBox.Show(d);
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
                return;
            }
            return;
        }


        private void backup_Click(object sender, RoutedEventArgs e)
        {


            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = true;

            if (dlg.ShowDialog() == true)
            {
                absolutePath = dlg.SelectedPath;
                relativePath = absolutePath.Substring(absolutePath.LastIndexOf(@"\") + 1);
                //MessageBox.Show(relativePath);
                Choose_Folder.Content = absolutePath;
                this.lstFilesFound.Clear();
                this.info.Clear();
                this.files.Clear();

                //MessageBox.Show(absolutePath);
                DirSearch(absolutePath);
                classes.Function.createXmlToSend(this.lstFilesFound, this.info, this.files);
                xml = classes.Function.DictToXml(this.files, relativePath);
                Thread thread = new Thread(new ThreadStart(watch));
                thread.IsBackground = true;
                thread.Start();
                clientSend("+BACKUP");
                
            }

           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            clientSend("RESTORE");

        }

        private void clientSend(string text)
        {
            if (client.isConnected())
                client.sendCommand(text, client.CLIENT.GetStream());
        }



        private void clientRecive()
        {
            if (client.isConnected())
            {
                new Thread(() =>
                {
                    string cmd;
                    NetworkStream ns = client.CLIENT.GetStream();
                    // Send the message to the connected TcpServer.
                    while (_isRunning )
                    {
                        try
                        {
                            cmd = client.reciveComand(ns);
                            if (cmd.Equals("++CLOSE"))
                            {
                                client.close();
                                _isRunning = false;
                                return;
                            }
                            if (cmd.Equals("+++LIST"))
                            {
                                int dim = client.reciveDimension(ns);
                                Console.WriteLine(dim);
                                clientSend("+++++OK");
                                String version = client.reciveVersion(dim, ns);
                                Console.WriteLine(version);
                                String[] list = version.Split(';');
                                backupList = new List<Utils.CheckBackup>();
                                for (int i = 0; i < list.Length - 1; i++)
                                {
                                    String[] backup = list[i].Split('%');
                                    backupList.Add(new Utils.CheckBackup(backup[0], backup[1], backup[2]));
                                }

                                box.Dispatcher.Invoke(new Action(() =>
                                {
                                    box.ItemsSource = backupList;
                                }), DispatcherPriority.ContextIdle);
                                cmd = "";


                            }
                            dir.Dispatcher.Invoke(new Action(() =>
                            {
                                dir.Text += "Wait to command\n";
                            }), DispatcherPriority.ContextIdle);

                            if (cmd.Equals("+BACKUP"))
                            {
                                client.sendFileDimension(xml.Length, ns);
                                cmd = client.reciveComand(ns);
                                if (cmd.Equals("+++++OK"))
                                {
                                    client.sendData(xml, ns);
                                    cmd = client.reciveComand(ns);
                                    if (cmd.Equals("+UPLOAD"))
                                    {
                                        try
                                        {
                                            client.sendData("+++++OK", ns);
                                            int totale = lstFilesFound.Count;
                                            int i = 0;
                                            foreach (String path in lstFilesFound)
                                            {
                                                pbStatus.Dispatcher.Invoke(() => pbStatus.Value = (i * 100) / totale, DispatcherPriority.Background);
                                                cmd = client.reciveComand(ns);
                                                if (cmd.Equals("+++FILE"))
                                                {
                                                    /*dir.Dispatcher.Invoke(new Action(() =>
                                                    {
                                                        dir.Text += "Invio il File\n";
                                                    }), DispatcherPriority.ContextIdle);*/
                                                    client.sendFile(path, ns);
                                                    i++;
                                                    if (client.reciveComand(ns) == "+++++OK")
                                                        client.sendCommand("+++++OK", ns);

                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            MessageBox.Show("Connesione Persa!");
                                            return;
                                        }
                                    }
                                    clientSend("+++LIST");
                                    cmd = "";
                                }
                            }

                            if (cmd.Equals("RESTORE"))
                            {
                                dir.Dispatcher.Invoke(new Action(() =>
                                {
                                    dir.Text += "Restoring\n";
                                }), DispatcherPriority.ContextIdle);

                                foreach (Utils.CheckBackup cb in backupList)
                                {
                                    if (cb.CHECK)
                                    {
                                        string toRestore = cb.NAME + @"\" + cb.VERSION;
                                        MessageBox.Show(toRestore);

                                        if (!Directory.Exists(backupFolder + @"\" + cb.NAME + @"\" + cb.VERSION))
                                        {
                                            Directory.CreateDirectory(backupFolder + @"\" + cb.NAME + @"\" + cb.VERSION);
                                        }
                                        client.sendFileDimension(toRestore.Length, client.CLIENT.GetStream());
                                        string mycmd = client.reciveComand(client.CLIENT.GetStream());
                                        Console.WriteLine(mycmd);
                                        if (mycmd.Equals("+++++OK"))
                                        {
                                            client.sendData(toRestore, client.CLIENT.GetStream());
                                            mycmd = client.reciveComand(client.CLIENT.GetStream());
                                            Console.WriteLine(mycmd);
                                            if (mycmd.Equals("+++++OK"))
                                            {
                                                while (true)
                                                {
                                                    mycmd = client.reciveComand(client.CLIENT.GetStream());
                                                    if (mycmd.Equals("++++END")) break;

                                                    if (mycmd.Equals("+++FILE") || mycmd.Equals("+++NEXT"))
                                                    {

                                                        int dim = client.reciveDimension(client.CLIENT.GetStream());
                                                        client.sendCommand("+++++OK", client.CLIENT.GetStream());
                                                        string relative = client.reciveVersion(dim, client.CLIENT.GetStream());
                                                        Console.WriteLine("path : " + relative);
                                                        client.sendCommand("+++++OK", client.CLIENT.GetStream());
                                                        dim = client.reciveDimension(client.CLIENT.GetStream());
                                                        client.sendCommand("+++++OK", client.CLIENT.GetStream());
                                                        string name = client.reciveVersion(dim, client.CLIENT.GetStream());
                                                        Console.WriteLine("fileName : " + name);
                                                        client.sendCommand("+++++OK", client.CLIENT.GetStream());
                                                        dim = client.reciveDimension(client.CLIENT.GetStream());
                                                        client.sendCommand("+++++OK", client.CLIENT.GetStream());
                                                        client.ReciveFile(Directory.GetCurrentDirectory() + @"\" + backupFolder + @"\" + cb.NAME + @"\" + cb.VERSION + @"\", relative, name, dim, client.CLIENT.GetStream());
                                                        Console.WriteLine("finish\n");
                                                        clientSend("+++++OK");

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        } catch (Exception e)
                        {
                            MessageBox.Show("Connessione persa!");
                            return;
                        }
                }
             return;
            }).Start();
             return;
            }
            return;
        }


        public void watch()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = absolutePath;
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        public void OnChanged(object source, FileSystemEventArgs e)
        {
            this.lstFilesFound.Clear();
            this.info.Clear();
            this.files.Clear();

            DirSearch(absolutePath);
            classes.Function.createXmlToSend(this.lstFilesFound, this.info, this.files);
            xml = classes.Function.DictToXml(this.files, relativePath);
            
            MessageBox.Show("Rilevata modifica dei files presenti nella cartella selezionata\n Aggiornamento versione Backup");
            clientSend("+BACKUP"); 

        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.sendCommand("++CLOSE", client.CLIENT.GetStream());
                this.Close();
                parent.Activate();
                parent.Show();
            }catch(Exception e1)
            {
                Console.WriteLine(e1.Message);
                _isRunning = false;
                MessageBox.Show("Errore di rete, Server offline");
                //this.Hide();
                this.Close();
            }

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var item = cb.DataContext;
            box.SelectedItem = item;
        }

        public static void SetTcpKeepAlive(Socket socket, uint keepaliveTime, uint keepaliveInterval)
        {
            /* the native structure
          struct tcp_keepalive {
           ULONG onoff;
          ULONG keepalivetime;
           ULONG keepaliveinterval;
          };
          */

            // marshal the equivalent of the native structure into a byte array
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(keepaliveTime)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)keepaliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)keepaliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

            // write SIO_VALS to Socket IOControl
            socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }


    }
}
