using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Reflection;
using System.IO;
using MP3ManagerBase.factory;

namespace MP3ManagerTest
{
    public class MP3FileHandlerTest
    {
        private Stream ReadRessource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream result = new MemoryStream();
            using (Stream stream = assembly.GetManifestResourceStream(filename))
            {
                stream.CopyTo(result);
            }
            return result;
        }
    }
}
