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
    public partial class MainWindow : Window
    {
        public Window parent { get; set; }

        private Network.Client client = null;
        private Thread t;
        private volatile Boolean _isRunning = false;
        private String username;
        private String password;
        public Boolean Registrazione = false;

        
        public MainWindow()
        {
            Trayicon.Instance();
            InitializeComponent();
        }

        //LOGIN
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string t = ConnectServer();
            Console.WriteLine(t);

            if (t != null)
            {
                FolderBackup rw = new FolderBackup(t);
                rw.parent = this;
                rw.Show();
                rw.Activate();
                this.Hide();
               
            }

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (client != null)
            {
                client.sendCommand("++CLOSE", client.CLIENT.GetStream());
                client.close();
            }

            base.OnClosing(e);
        }


        private string ConnectServer()
        {
            if (this.client == null)
            {
                try
                {
                    TcpClient tmp = new TcpClient();
                    _isRunning = true;
                    // ip e porta del server fissati 
                    tmp.Connect(classes.Function.checkIPAddress("192.168.43.143"), Int16.Parse("3000"));
                    //tmp.Connect(classes.Function.checkIPAddress("127.0.0.1"), Int16.Parse("3000"));
                    client = new Network.Client(tmp);
                  
                    string str =  login();
                    client.close();
                    client = null;
                    return str;
                    //t = new Thread(new ParameterizedThreadStart(threadProc));
                    //
                    //t.Start(client);
                    //command = "++LOGIN";

                }
                catch (SocketException e1)
                {
                    MessageBox.Show("Server non ragiungibile!");
                    this.client = null;
                    return null;

                }
                catch (FormatException e2)
                {
                    MessageBox.Show("Porta errata!");
                    this.client = null;
                    return null;

                }
                catch (InvalidOperationException e3)
                {
                    MessageBox.Show("Indirizzo IP errato!");
                    this.client = null;
                    return null;

                }

            }
            else
            {
                return null;
            }
            
        }


        public string login()
        {

            string cmd;
            Network.Client myClient = client;
            NetworkStream ns = myClient.CLIENT.GetStream();
            errorBox_Username.Visibility = System.Windows.Visibility.Hidden;
            errorBox_Password.Visibility = System.Windows.Visibility.Hidden;
            cmd = myClient.reciveComand(ns);
      
            if (cmd.Equals("+++OPEN"))
            {
                Connect.Dispatcher.Invoke(new Action(() =>
                {
                    username = usernameTxtBox.Text;
                    password = paswordTxtBox.Password;
                }), DispatcherPriority.ContextIdle);

            
                if (username.Equals(""))
                {
                    errorBox_Username.Visibility = System.Windows.Visibility.Visible;
                   
                }
                    

                if (password.Equals(""))
                {
                    errorBox_Password.Visibility = System.Windows.Visibility.Visible;
           
                }

                myClient.sendCommand("++LOGIN", ns);
                cmd = myClient.reciveComand(ns);
                if (cmd.Equals("+++++OK"))
                {
                    
                  
                    myClient.sendFileDimension(username.Length + password.Length + 1, ns);
                    cmd = myClient.reciveComand(ns);
                    if (cmd.Equals("+++++OK"))
                    {
                        string user = username + ":" + password;

                        myClient.sendData(user, ns);
                        string tmp = myClient.reciveComand(ns);
                            
                        if (tmp.Equals("++CLOSE"))
                        {
                            myClient.close();
                            return username;
                        }
                        else
                        {
                            errorBox.Visibility = System.Windows.Visibility.Visible; 
                            myClient.close();
                            return null;
                        }

                    }
                    
                }
            }

            return null;
        }


        //REGISTRAZIONE
        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            Color c = (Color)ColorConverter.ConvertFromString("#FF024FFF");
            this.registerLabel.Foreground = new SolidColorBrush(c);
        }

        private void registerLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            Color c = (Color)ColorConverter.ConvertFromString("#FF000000");
            this.registerLabel.Foreground = new SolidColorBrush(c);
        }

        private void registerLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow();
            rw.parent = this;
            rw.Show();
            rw.Activate();
            this.Hide();
            this.errorBox.Visibility = System.Windows.Visibility.Hidden;
            this.errorBox_Username.Visibility = System.Windows.Visibility.Hidden;
            this.errorBox_Password.Visibility = System.Windows.Visibility.Hidden;
            this.usernameTxtBox.Text = String.Empty;
            this.paswordTxtBox.Password = String.Empty;
        }

       
    }
   

}


