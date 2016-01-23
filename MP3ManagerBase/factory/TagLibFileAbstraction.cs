using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.factory
{
    internal class TagLibFileAbstraction : TagLib.File.IFileAbstraction
    {
        private string name;

        public TagLibFileAbstraction(string file)
        {
            name = file;
        }

        public string Name
        {
            get { return name; }
        }

        public System.IO.Stream ReadStream
        {
            get { return new FileStream(Name, System.IO.FileMode.Open); }
        }

        public System.IO.Stream WriteStream
        {
            get { return new FileStream(Name, System.IO.FileMode.Open); }
        }

        public void CloseStream(System.IO.Stream stream)
        {
            stream.Close();
        }
    }
}
