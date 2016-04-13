using MP3ManagerBase.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3ManagerBase.Views
{
    public class MissingAlben : WSongInformation
    {
        public string InterpretMBId { get; set; }

        public string AlbumMBId { get; set; }

        public string TitleMBId { get; set; }

    }
}
