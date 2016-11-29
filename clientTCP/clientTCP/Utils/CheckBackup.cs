using System;
using System.Collections.Generic;
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

        public CheckBackup(String name, String version, String time)
        {
            this.name = name;
            this.version = version;
            this.time = time;
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
