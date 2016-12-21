using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace clientTCP
{

    public class Trayicon : System.Windows.Forms.Form
    {
 
        static private Trayicon instance;
        private NotifyIcon trayicon;
        private ContextMenu Content;
        private MenuItem exitItem;
        private MenuItem syncItem;
        private MenuItem cpItem;
        private MenuItem messageItem;
        private NotifyIcon ShareBox;
        private System.ComponentModel.IContainer components;
        private Network.Client client = null;

        private Trayicon()
        {
            this.components = new System.ComponentModel.Container();
            this.Content = new ContextMenu();
            this.exitItem = new MenuItem();
            this.cpItem = new MenuItem();
            this.syncItem = new MenuItem();
            this.messageItem = new MenuItem();

            // Initialize contextMenu1
            this.Content.MenuItems.Add( this.exitItem );
            this.Content.MenuItems.Add( this.cpItem );
            this.Content.MenuItems.Add( this.syncItem );
            this.Content.MenuItems.Add("-");
            this.Content.MenuItems.Add(this.messageItem );

            // Initialize menuItem1
            this.exitItem.Index = 3;
            this.exitItem.Text = "E&xit";
            this.exitItem.Click += new EventHandler(this.exit_Click);


            this.cpItem.Index = 2;
            this.cpItem.Text = "Control Panel";
            this.cpItem.Click += new EventHandler(this.controlPanel_Click);


            this.syncItem.Index = 1;
            this.syncItem.Text = "Start Sync";
            this.syncItem.Name = "StartSync";
            this.syncItem.Click += new EventHandler(this.startSync_Click);

            this.messageItem.Index = 0;
            this.messageItem.Text = "";
            this.messageItem.Enabled = false;
 
             // Set up how the form should be displayed.
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Text = "Notify Icon Example";

            // Create the NotifyIcon.
            this.trayicon = new NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
       
            trayicon.Icon = new Icon("Icon\\applicationIcon.ico");
            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            trayicon.ContextMenu = this.Content;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            trayicon.Text = "Sharebox";
            trayicon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            trayicon.DoubleClick += new System.EventHandler(this.trayicon_DoubleClick);

        }

        public static Trayicon Instance()
        {
              if (instance == null)
              {
                  instance = new Trayicon();
              }
              return instance;
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            // Clean up any components being used.
            if (disposing)
                if (components != null)
                    components.Dispose();

            base.Dispose(disposing);
        }

        private void trayicon_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.

            // Set the WindowState to normal if the form is minimized.
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            // Activate the form.
            this.Activate();
        }

        private void exit_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            trayicon.Visible = false;
            this.Close();
            Environment.Exit(0);
        }

        private void controlPanel_Click(object Sender, EventArgs e)
        {
         // CONNESSIONE/ACCESSO A FOLDER BACKUP
        }
        private void UpdateStatus(string status){
            this.messageItem.Text = status;
        }
        private void startSync_Click(object Sender, EventArgs e)
        {
                 // SINCRONIZZARE CARTELLA
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Trayicon));
            this.ShareBox = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // ShareBox
            // 
            this.ShareBox.Text = "Sharebox";
            this.ShareBox.Visible = true;
            // 
            // Trayicon
            // 
            this.AccessibleName = "ShareBox";
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Trayicon";
            this.Text = "Sharebox";
            this.Load += new System.EventHandler(this.Trayicon_Load);
            this.ResumeLayout(false);

        }

        private void Trayicon_Load(object sender, EventArgs e)
        {

        }
       

    }
    }