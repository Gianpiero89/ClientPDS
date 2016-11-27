using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientTCP.Utils
{
    class CheckBackup
    {
        private String name;    // Nome del backup
        private String version;    // Versione del backup
        private String time;  // Data del backup
        private Boolean _isChecked; // Utilizzato nella ListView per vedere quale dei backup ripristinare 

        public CheckBackup(string name, string version, string time)
        {
            this.name = name;
            this.version = version;
            this.time = time;
            this._isChecked = false;
        }


        public String NAME
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public String VERSION
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }


        public String TIME
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        public Boolean CHECK
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                
            }
        }

     


    }
}
