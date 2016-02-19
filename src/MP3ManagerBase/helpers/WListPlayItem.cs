using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MP3ManagerBase.helpers
{
    public class WListPlayItem
    {
        public string Interpret { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
        public int Id { get; set; }

        public string TraceText()
        {
            return "Interpret=" + Interpret + ";Album=" + Album + ";Title=" + Title + ";Path=" + Path + ";Filename=" + Filename + ";ID=" + Id;
        }

    }
}
