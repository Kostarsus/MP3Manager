using MP3ManagerBase.manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MP3ManagerTest
{
    public class AlbumAdditionalReaderTest
    {
        [Fact]
        public void ReadWebserviceTest_TheMissionChildren_Success()
        {
            AlbumAdditionalReader testObject = new AlbumAdditionalReader();
            var test = testObject.ReadTitlesFor("The Mission", "Children");
            test.Wait();
            var testResult = test.Result;
            Assert.Equal(testResult.Count, 8);
        }

    }
}
