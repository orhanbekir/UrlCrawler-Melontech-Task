using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrawlerLib;

namespace WebCrawlerLibTest
{
    [TestClass]
    public class WebCrawlerLibTest
    {
        [TestMethod]
        public void IsFile_Rar()
        {
            Crawler cr = new Crawler(TestConstant.URL);
            PrivateObject obj = new PrivateObject(cr);
            object[] parameters = { TestConstant.RAR_FILE_EXTENSION };
            var retVal = obj.Invoke("IsFile", parameters);
            Assert.AreEqual(true, retVal);
        }

        [TestMethod]
        public void CheckUrl_False()
        {
            Crawler cr = new Crawler(TestConstant.URL);
            PrivateObject obj = new PrivateObject(cr);
            object[] parameters = { TestConstant.ALTERNATIVE_URL};
            var retVal = obj.Invoke("CheckUrl", parameters);
            Assert.AreEqual(false, retVal);
        }

        [TestMethod]
        public void DownloadHtmlString_StringEmpty()
        {
            Crawler cr = new Crawler(TestConstant.URL);
            PrivateObject obj = new PrivateObject(cr);
            object[] parameters = { TestConstant.ALTERNATIVE_URL };
            var retVal = obj.Invoke("DownloadHtmlString", parameters);
            Assert.AreEqual(String.Empty, retVal);
        }//BuildUrl

        [TestMethod]
        public void BuildUrl_True()
        {
            var retVal = UrlUtils.BuildUrl(TestConstant.URL, TestConstant.URL_PATH);
            Assert.AreEqual(TestConstant.URL_WITH_PATH, retVal);
        }
    }
}
