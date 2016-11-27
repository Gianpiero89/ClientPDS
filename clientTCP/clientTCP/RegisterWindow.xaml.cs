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
        private Network.Client client;
        private volatile Boolean _isRunning = false;
        public Window parent { get; set; }

        public RegisterWindow()
        {
          
            InitializeComponent();
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
            _isRunning = true;
            // ip e porta del server fissati 
            tmp.Connect(classes.Function.checkIPAddress("127.0.0.1"), Int16.Parse("1500"));
           
            client = new Network.Client(tmp);
      /*  t = new Thread(new ParameterizedThreadStart(threadProc));
            t.Start(client);
            command = "++LOGIN";

            BackupServiceClient server = new BackupServiceClient();  
          //  string salt = server.registerStep1(username)
                
                ;
            if (salt == null)
            {
                MessageBox.Show(this, "Username already choosen! Try another!", "Registration problem", MessageBoxButton.OK);
                return;

            }
            if (server.registerStep2(username, AuthenticationPrimitives.hashPassword(password, salt), salt))
            {
                MessageBox.Show(this, "Registration succeed. You can log in now.", "Registration succeed!", MessageBoxButton.OK);
                this.Hide();
                this.parent.Activate();
                this.parent.Show();
            }
            else
            {
                MessageBox.Show(this, "Registration procedure failed!", "Error", MessageBoxButton.OK);
            }*/
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.parent.Show();
            this.parent.Activate();
        }


    }
}
