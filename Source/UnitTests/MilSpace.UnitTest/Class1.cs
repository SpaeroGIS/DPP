using Microsoft.VisualStudio.TestTools.UnitTesting;
using MilSpace.Core.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.UnitTest
{
    [TestClass]
    public class MilSpaceUnitTestCmpression
    {

        [TestMethod]
        public void GetZipEntryExisting1()
        {
            Assert.AreEqual(true, false);
        }
        [TestMethod]
        public void GetZipEntryExisting()
        {

            using (ZipManager zipMgr = new ZipManager(@"E:\Data\S1\SRC\S1A_IW_SLC__1SDV_20190711T034707_20190711T034737_028064_032B5E_9546.zip"))
            {

                zipMgr.GoThrogh();
            }
            Assert.AreEqual(true, true);
        }
    }
}
