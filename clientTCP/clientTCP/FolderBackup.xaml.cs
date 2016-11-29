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
        private String command = "";
        private List<Utils.CheckBackup> backupList;





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
                _isRunning = true;
                // ip e porta del server fissati 
                tmp.Connect(classes.Function.checkIPAddress("127.0.0.1"), Int16.Parse("1500"));
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

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _isRunning = false;
        }

        public void DirSearch(String path)
        {

            try

            {
                foreach (string f in Directory.GetFiles(path, "*"))
                {
                    lstFilesFound.Add(f);
                    int resto = f.LastIndexOf(@"\") - f.LastIndexOf(relativePath);
                    String tmp = f.Substring(f.LastIndexOf(relativePath) - 1, resto + 1);
                    info.Add(new Utils.FileInfomation(tmp, f.Substring(f.LastIndexOf(@"\") + 1)));
                }
                foreach (string d in Directory.GetDirectories(path))
                {
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

                this.lstFilesFound.Clear();
                this.info.Clear();
                this.files.Clear();

                DirSearch(absolutePath);
                classes.Function.createXmlToSend(this.lstFilesFound, this.info, this.files);
                xml = classes.Function.DictToXml(this.files, relativePath);
                //Thread thread = new Thread(new ParameterizedThreadStart(checkFileInDirectory));
                Utils.WrapItem wp = new Utils.WrapItem(absolutePath, this.info);
                //thread.IsBackground = true;
                //thread.Start(wp);
                clientSend("+BACKUP");
            }

           
        }

        public void checkFileInDirectory(object wrap)
        {
            while (_isRunning)
            {
                Thread.Sleep(60000);
                Utils.WrapItem wp = (Utils.WrapItem)wrap;
                String path = wp.PATH;
                List<Utils.FileInfomation> checkFiles = new List<Utils.FileInfomation>(wp.FILES);
                this.info.Clear();
                lstFilesFound.Clear();
                DirSearch(path);
                lock (this.files)
                {
                    this.files.Clear();
                    classes.Function.createXmlToSend(this.lstFilesFound, checkFiles, this.files);
                    xml = classes.Function.DictToXml(this.files, relativePath);
                    if (!classes.Function.areEquals(this.info, checkFiles))
                    {
                        lock (command)
                        {
                            MessageBox.Show("Aggiorno");
                            clientSend("+BACKUP");
                        }
                    }
                }
            }
            return;

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
                    while (_isRunning)
                    {
                        cmd = client.reciveComand(ns);
                        if (cmd.Equals("+++LIST"))
                        {
                            int dim = client.reciveDimension(ns);
                            Console.WriteLine(dim);
                            clientSend("+++++OK");
                            String version = client.reciveVersion(dim, ns);
                            Console.WriteLine(version);
                            String[] list = version.Split(';');
                            for (int i = 0; i < list.Length - 1; i++)
                            {
                                String[] backup = list[i].Split('-');
                                backupList.Add(new Utils.CheckBackup(backup[0], backup[1], backup[2]));
                            }

                            box.Dispatcher.Invoke(new Action(() =>
                            {
                                box.ItemsSource = backupList;
                            }), DispatcherPriority.ContextIdle);
                            command = "";


                        }
                        dir.Dispatcher.Invoke(new Action(() =>
                        {
                            dir.Text += "Wait to command\n";
                        }), DispatcherPriority.ContextIdle);
                        
                        if (cmd.Equals("+BACKUP"))
                        {
                            client.sendFileDimension(xml.Length, ns);
                            client.sendData(xml, ns);
                            cmd = client.reciveComand(ns);
                            if (cmd.Equals("+UPLOAD"))
                                {
                                    foreach (String path in lstFilesFound)
                                    {
                                        cmd = client.reciveComand(ns);
                                        if (cmd.Equals("+++FILE"))
                                        {
                                            dir.Dispatcher.Invoke(new Action(() =>
                                            {
                                                dir.Text += "Invio il File\n";
                                            }), DispatcherPriority.ContextIdle);
                                            client.sendFile(path, ns);
                                            client.sendCommand("+++++OK", ns);
                                        }
                                    }
                                command = "";
                            }
                         }

                    if (command.Equals("RESTORE"))
                    {
                        foreach (Utils.CheckBackup cb in backupList)
                        {
                            if (cb.CHECK)
                            {
                                client.sendCommand("RESTORE", ns);
                                int dim = client.reciveDimension(ns);
                                String xml = client.ReciveXMLData(dim, ns);
                            }
                        }

                    }

                }


            }).Start();
            }
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
