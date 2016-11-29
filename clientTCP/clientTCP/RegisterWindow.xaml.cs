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

        public RegisterWindow()
        {
          
            InitializeComponent();
      
            _isRunning = true;
          
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = this.usernameTxtBox.Text;
            string password = this.passwordTxtBox.Password;

            if (username.Equals(""))
            {
                MessageBox.Show(this, "Username cannot be empty!", "Missing username", MessageBoxButton.OK);
                return;
            }

            if (password.Equals(""))
            {
                MessageBox.Show(this, "Password cannot be empty!", "Missing password", MessageBoxButton.OK);
                return;
            }

            TcpClient tmp = new TcpClient();
            // ip e porta del server fissati 
            tmp.Connect(classes.Function.checkIPAddress("127.0.0.1"), Int16.Parse("1500"));
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
///////////////////////////////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            //CONTROLLARE SE TUTTO E' ANDATO A BUON FINE
                MessageBox.Show(this, "Registration succeed. You can log in now.", "Registration succeed!", MessageBoxButton.OK);
                this.Hide();
                this.parent.Activate();
                this.parent.Show();
            ///////////////////////////////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA         
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
    }
}
