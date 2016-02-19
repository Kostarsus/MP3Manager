using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MP3ManagerBase.manager
{
    public class WDuplicateEntry : WTitleShort
    {
        public int TitleId { get; set; }

        public string Path { get; set; }

        public string Filename { get; set; }

        public int Bitrate { get; set; }

        public string Length { get; set; }

        public long? Bytes { get; set; }


        public override string ToString()
        {
            StringBuilder retValue = new StringBuilder();
            retValue.Append("TitleId= " + TitleId);
            retValue.Append(", Path= " + Path);
            retValue.Append(", Filename= " + Filename);
            retValue.Append(", Bitrate= " + Bitrate);
            retValue.Append(", Length= " + Length);
            retValue.Append(", Bytes= " + Bytes);
            return retValue.ToString();
        }

    }
}
