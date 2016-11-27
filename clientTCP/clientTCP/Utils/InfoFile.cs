using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace clientTCP.classes
{
    class InfoFile
    {
        private DateTime modified;
        private Byte[] md5String;

        public InfoFile(DateTime d)
        {
            this.modified = d;
            this.md5String = null;
        }

        public void ComputeHash(String filePath)
        {
            using (var md5 = MD5.Create())
            {
                this.md5String = md5.ComputeHash(File.ReadAllBytes(filePath));
            }
        }

        public DateTime Mod
        {
            get
            {
                return modified;
            }
            set
            {
                modified = value;
            }
        } 

        public Byte[] Md5
        {
            get
            {
                return md5String;
            }
            set
            {
                md5String = value;
            }

        }

       

    }
}

