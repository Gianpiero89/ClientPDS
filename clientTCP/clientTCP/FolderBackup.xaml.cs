﻿using System;
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
        private volatile Boolean Shutdown = false;


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (client != null)
            {
                try
                {
                    client.sendCommand("++CLOSE", client.CLIENT.GetStream());
                    _isRunning = false;
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message);
                    _isRunning = false;
                   // MessageBox.Show("Errore di rete, Server offline");
                    //this.Hide();
                    this.Hide();
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
                tmp.Connect(classes.Function.checkIPAddress("192.168.137.111"), Int16.Parse("3000"));
                //tmp.Connect(classes.Function.checkIPAddress("127.0.0.1"), Int16.Parse("3000"));
                client = new Network.Client(tmp);
            
                string ccc = client.reciveComand(client.CLIENT.GetStream());
                if (ccc.Equals("+++OPEN"))
                {
                    Thread th = new Thread(CheckCon);
                    th.Start(client.CLIENT);
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


        private void CheckCon(object o)
        {
            TcpClient c = (TcpClient)o;
            MessageBox.Show("partito!");
            while (c.Connected);
            MessageBox.Show("Errore di rete, Server offline!");
            Shutdown = true;

         
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

            try
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
                    // MessageBox.Show(xml);
                    Thread thread = new Thread(new ThreadStart(watch));
                    thread.IsBackground = true;
                    thread.Start();
                    clientSend("+BACKUP");

                }

            }
            catch (Exception e1)
            {
                MessageBox.Show("Errore di rete");
                Application.Current.Shutdown();
            }

           
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                clientSend("RESTORE"); 
            }
            catch(Exception e1)
            {
                MessageBox.Show("Errore di rete");
                Application.Current.Shutdown();
            }

        }

        private void clientSend(string text)
        {
            if (client.isConnected())
                client.sendCommand(text, client.CLIENT.GetStream());
            else
            {
                MessageBox.Show("Errore di rete");
                Application.Current.Shutdown(); 
            }
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
               

                    while (_isRunning  && !Shutdown)
                    {
                       
                        try
                        {
                            cmd = client.reciveComand(ns);
                            Console.WriteLine(cmd);
                            if (cmd == null)
                            {
                                _isRunning = false;
                                return;
                            }
                            if (cmd.Equals("++CLOSE"))
                            {
                                client.close();
                                _isRunning = false;
                                Application.Current.Shutdown();
                                return;
                            }
                            if (cmd == null){_isRunning = false; return;}
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
                            if (cmd == null) { _isRunning = false; return; }
                            if (cmd.Equals("+BACKUP"))
                            {
                                dir.Dispatcher.Invoke(new Action(() =>
                                {
                                    dir.Text += "Sono nel backup\n";
                                }), DispatcherPriority.ContextIdle);
                                client.sendFileDimension(xml.Length, ns);
                                //   MessageBox.Show(""+xml.Length);
                                Console.WriteLine("Crash");
                                cmd = client.reciveComand(ns);
                                if (cmd == null) { _isRunning = false; return; }
                                if (cmd.Equals("+++++OK"))
                                {
                                    Console.WriteLine("Crash dopo OK");
                                    dir.Dispatcher.Invoke(new Action(() =>
                                    {
                                        dir.Text += "Ok \n";
                                    }), DispatcherPriority.ContextIdle);

                                    client.sendData(xml, ns);
                                    Console.WriteLine("Crash Send Data");
                                    dir.Dispatcher.Invoke(new Action(() =>
                                    {
                                        dir.Text += "Send XML \n";

                                    }), DispatcherPriority.ContextIdle);

                                
                                    cmd = client.reciveComand(ns);
                                    if (cmd == null) { _isRunning = false; return; }
                                    Console.WriteLine("Crash Send ok");
                                    if (cmd.Equals("+++++OK"))
                                    {
                                        try
                                        {
                                            cmd = client.reciveComand(ns);
                                        }
                                        catch (IOException ioe)
                                        {
                                            
                                            Application.Current.Shutdown();
                                        }
                                        if (cmd == null) { _isRunning = false; return; }
                                        if (cmd.Equals("+UPLOAD"))
                                        {
                                            Console.WriteLine("Crash Upload ok");
                                            try
                                            {
                                                dir.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    dir.Text += "Inizio l'upload\n";
                                                }), DispatcherPriority.ContextIdle);
                                                client.sendData("+++++OK", ns);
                                                int totale = lstFilesFound.Count;
                                                int i = 0;
                                                foreach (String path in lstFilesFound)
                                                {
                                                    pbStatus.Dispatcher.Invoke(() => { pbStatus.Value = (i * 100) / totale; count.Text = (i * 100) / totale + "%"; }, DispatcherPriority.Background);
                                                    cmd = client.reciveComand(ns);
                                                    if (cmd == null) { _isRunning = false; return; }
                                                    if (cmd.Equals("+++FILE"))
                                                    {
                                                        Console.WriteLine("Crash File");
                                                        dir.Dispatcher.Invoke(new Action(() =>
                                                        {
                                                            dir.Text += "Invio il File " + i + "\n";
                                                        }), DispatcherPriority.ContextIdle);

                                                        client.sendFile(path, ns);
                                                        i++;
                                                        if (client.reciveComand(ns) == "+++++OK")
                                                            client.sendCommand("+++++OK", ns);

                                                    }
                                                }
                                                pbStatus.Dispatcher.Invoke(() => { pbStatus.Value = (totale * 100) / totale; count.Text = (totale * 100) / totale + "%"; }, DispatcherPriority.Background);
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
                            }
                            if (cmd == null) { _isRunning = false; return; }
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
                                        if (cmd == null) { _isRunning = false; return; }
                                        if (mycmd.Equals("+++++OK"))
                                        {
                                            client.sendData(toRestore, client.CLIENT.GetStream());
                                            mycmd = client.reciveComand(client.CLIENT.GetStream());
                                            Console.WriteLine(mycmd);
                                            if (cmd == null) { _isRunning = false; return; }
                                            if (mycmd.Equals("+++++OK"))
                                            {
                                                int i = 0;
                                                Boolean Next = true;
                                                while (Next)
                                                {
                                                    mycmd = client.reciveComand(client.CLIENT.GetStream());
                                                    Console.WriteLine(mycmd);
                                                    if (mycmd == null) { _isRunning = false; return; }
                                                    if (mycmd.Equals("++++END")) { Console.WriteLine("Finito"); break; };

                                                    if (mycmd.Equals("+++FILE"))
                                                    {

                                                        Console.WriteLine("Numero : " + i);
                                                        i++;
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
                                                Console.WriteLine("Restore Completato!");
                                                MessageBox.Show("Restore Completato");
                                                Console.WriteLine(cmd);
                                                cmd = "";

                                            }
                                            
                                        }
                                    }
                                }
                            }
                        } catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
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
            watcher.Deleted += new FileSystemEventHandler(OnChangedDelete);
            watcher.EnableRaisingEvents = true;
            if (!_isRunning) return;
        }

        public void OnChanged(object source, FileSystemEventArgs e)
        {
            watcher.EnableRaisingEvents = false;
            this.lstFilesFound.Clear();
            this.info.Clear();
            this.files.Clear();
          //  MessageBox.Show(e.FullPath);

            while (IsFileLocked(e.FullPath))
            {
                Console.WriteLine("Attendo che la copia sia completata");
            }
           // MessageBox.Show("fuori");

            DirSearch(absolutePath);
            classes.Function.createXmlToSend(this.lstFilesFound, this.info, this.files);
            xml = classes.Function.DictToXml(this.files, relativePath);

            MessageBox.Show("Rilevata modifica dei files presenti nella cartella selezionata\n Aggiornamento versione Backup");
            clientSend("+BACKUP");
            watcher.EnableRaisingEvents = true;

        }

        public void OnChangedDelete(object source, FileSystemEventArgs e)
        {
   
            this.lstFilesFound.Clear();
            this.info.Clear();
            this.files.Clear();
         //   MessageBox.Show(e.FullPath);
        

            DirSearch(absolutePath);
            classes.Function.createXmlToSend(this.lstFilesFound, this.info, this.files);
            xml = classes.Function.DictToXml(this.files, relativePath);

            MessageBox.Show("Rilevata modifica dei files presenti nella cartella selezionata\n Aggiornamento versione Backup");
            clientSend("+BACKUP");
           

        }

        private bool IsFileLocked(string FullPath)
        {
            FileInfo file = new FileInfo(FullPath);
            FileStream stream = null;

            try
            {
               
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                if (stream.Length == 0)
                    return true;
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.sendCommand("++CLOSE", client.CLIENT.GetStream());
                _isRunning = false;
                this.Close();
                parent.Activate();
                parent.Show();
            }catch(Exception e1)
            {
                Console.WriteLine(e1.Message);
                _isRunning = false;
                MessageBox.Show("Errore di rete, Server offline");
                client.close();
                //this.Hide();
                this.Close();
                Application.Current.Shutdown();
            }

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var item = cb.DataContext;
            box.SelectedItem = item;
        }

    }
}
