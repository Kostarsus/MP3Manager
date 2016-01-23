using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MP3ManagerBase.factory
{
    public class WFileInfo
    {
        public string Path { get; set; }
        public string Filename { get; set; }
        public long Bytes { get; set; }
        public DateTime EditDate { get; set; }
        public string TraceText()
        {
            return "Path=" + Path + ";Filename=" + Filename + ";Bytes=" + Bytes;
        }
    }
}

