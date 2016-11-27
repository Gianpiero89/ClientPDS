using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientTCP.Utils
{
    class WrapItem
    {
        private String path;
        private List<FileInfomation> files;

        public WrapItem(String path, List<FileInfomation> info)
        {
            this.path = path;
            this.files = info;
        }

        public String PATH
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        public List<FileInfomation> FILES
        {
            get
            {
                return files;
            }
            set
            {
                files = value;
            }
        }
    }
}
