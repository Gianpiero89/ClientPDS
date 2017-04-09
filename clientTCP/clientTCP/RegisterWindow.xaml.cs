using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Threading;


namespace clientTCP
{
    /// <summary>
    /// Logica di interazione per RegisterWindow.xaml
    /// </summary>

    public partial class RegisterWindow : Window
    {


        private volatile Boolean _isRunning = false;
        public Window parent { get; set; }
        private Network.Client client;
        private Boolean Errore = false;

        public RegisterWindow()
        {

            InitializeComponent();

            _isRunning = true;

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = this.usernameTxtBox.Text;
            string password = this.passwordTxtBox.Password;
            errorBox_Username.Visibility = System.Windows.Visibility.Hidden;
            errorBox_Password.Visibility = System.Windows.Visibility.Hidden;

            /////////////////////////////////////////////////////////////////////
            if (username.Equals(""))
            {
                errorBox_Username.Visibility = System.Windows.Visibility.Visible;
                Errore = true; 
            }


            if (password.Equals(""))
            {
                errorBox_Password.Visibility = System.Windows.Visibility.Visible;
                Errore = true;
            }


            if (Errore) return;
            ////////////////////////////////////////////////////////////////

            TcpClient tmp = new TcpClient();
            // ip e porta del server fissati 
            tmp.Connect(classes.Function.checkIPAddress("172.29.146.235"), Int16.Parse("3000"));
            client = new Network.Client(tmp);
            string ccc = client.reciveComand(client.CLIENT.GetStream());

            if (ccc.Equals("+++OPEN"))
            {
             
                client.sendCommand("++++REG", client.CLIENT.GetStream());
                client.sendFileDimension(username.Length, client.CLIENT.GetStream());

                ccc = client.reciveComand(client.CLIENT.GetStream());
                if (ccc.Equals("+++++OK"))
                    client.sendData(username, client.CLIENT.GetStream());

                ccc = client.reciveComand(client.CLIENT.GetStream());
                if (ccc.Equals("+++++OK"))
                    client.sendFileDimension(password.Length, client.CLIENT.GetStream());

                ccc = client.reciveComand(client.CLIENT.GetStream());
                if (ccc.Equals("+++++OK"))
                    client.sendData(password, client.CLIENT.GetStream());

            }


            errorBox.Content = "Registration succeed";
            errorBox.Visibility = System.Windows.Visibility.Visible;
            errorBox.Foreground = Brushes.Green;
      
            System.Threading.Thread.Sleep(1000);
            this.Hide();
            this.parent.Activate();
            this.parent.Show();
              
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.parent.Show();
            this.parent.Activate();
        }

        private void clientSend(string text)
        {
            if (client.isConnected())
                client.sendCommand(text, client.CLIENT.GetStream());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
                this.Hide();
                this.parent.Show();
                this.parent.Activate();

        }
    }
}
