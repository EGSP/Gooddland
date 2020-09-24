using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gasanov.Utils.SaveUtilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PropertyFileProxyTests
    {
        public static string FilesPath = "D:\\UProject\\Gooddland\\Assets\\Tests\\Data\\";

        [SetUp]
        public void CreateDeafaultFile()
        {
            var fs = new FileStream(FilesPath +"propsTest.txt", FileMode.Create, FileAccess.ReadWrite);
            var sw = new StreamWriter(fs);
            
            sw.WriteLine("seed: 123;");
            sw.WriteLine("wqwq 4343");
            sw.WriteLine("peed: peedString;");
            sw.WriteLine("seedF: 123.56;");
            sw.WriteLine("seedFC: 123,56;");
            sw.Close();
            fs.Close();
        }
        
        
        [Test]
        public void Constructor_FSNull_NullRefException()
        {
            Assert.Throws<NullReferenceException>(() => new PropertyFileProxy(null));
        }

        [Test]
        public void MatchAnyProperty_Correct_KeySeedValue123kK()
        {
            string key;
            string value;
            PropertyFileProxy.MatchAnyProperty("Seed: 123kK;", out key, out value);

            Assert.AreEqual("Seed123kK", key + value);
        }
        
        [Test]
        public void MatchAnyProperty_Incorrect_WithoutSpecialSymbols()
        {
            string key;
            string value;
            PropertyFileProxy.MatchAnyProperty("Seed: 123kK", out key, out value);

            Assert.AreNotEqual("Seed123kK", key + value);
        }
        
        [Test]
        public void GetInt_seed_123()
        {
            var fs = new FileStream(FilesPath+"propsTest.txt", FileMode.Open, FileAccess.ReadWrite);
            
            var pfp = new PropertyFileProxy(fs, NotExistProperty.Exception, IncorrectPropertyType.Exception);

            int value = -1;
            pfp.GetInt("seed", ref value);
            Assert.AreEqual(123,value);
        }

        [Test]
        public void GetFloat_seedF_123dot56()
        {
            var fs = new FileStream(FilesPath+"propsTest.txt", FileMode.Open, FileAccess.ReadWrite);
            
            var pfp = new PropertyFileProxy(fs, NotExistProperty.Exception, IncorrectPropertyType.Exception);

            float value = -1;
            pfp.GetFloat("seedF", ref value);
            Assert.AreEqual(123.56f,value);
        }
        
        [Test]
        public void GetFloat_seedFC_123comma56()
        {
            var fs = new FileStream(FilesPath+"propsTest.txt", FileMode.Open, FileAccess.ReadWrite);
            
            var pfp = new PropertyFileProxy(fs, NotExistProperty.Exception, IncorrectPropertyType.Exception);

            float value = -1;
            pfp.GetFloat("seedFC", ref value);
            Assert.AreEqual(123.56f,value);
        }

        [Test]
        public void GetInt_peed_RewriteDefault()
        {
            var fs = new FileStream(FilesPath+"propsTest.txt", FileMode.Open, FileAccess.ReadWrite);
            
            var pfp = new PropertyFileProxy(fs, NotExistProperty.Exception, IncorrectPropertyType.RewriteDefault);

            int value = -1;
            pfp.GetInt("peed", ref value);
            Assert.AreEqual(0,value);
        }

        [Test]
        public void SetInt_newProp_600()
        {
            var fs = new FileStream(FilesPath+"propsTest.txt", FileMode.Open, FileAccess.ReadWrite);
            
            var pfp = new PropertyFileProxy(fs, NotExistProperty.Exception, IncorrectPropertyType.Exception);

            pfp.SetInt("newProperty", 600);
            int value = -1;
            pfp.GetInt("newProperty", ref value);
            Assert.AreEqual(600,value);
        }
        
        [Test]
        public void SetInt_seed_658()
        {
            var fs = new FileStream(FilesPath+"propsTest.txt", FileMode.Open, FileAccess.ReadWrite);
            
            var pfp = new PropertyFileProxy(fs, NotExistProperty.Exception, IncorrectPropertyType.Exception);

            pfp.SetInt("seed", 658);
            int value = -1;
            pfp.GetInt("seed", ref value);
            Assert.AreEqual(658,value);
        }

        [Test]
        public void Apply_PFP_PFP()
        {
            var fs = new FileStream(FilesPath+"propsTest.txt", FileMode.Open, FileAccess.ReadWrite);
            var pfp = new PropertyFileProxy(fs, NotExistProperty.Exception, IncorrectPropertyType.Exception);

            pfp.SetInt("seed", 658);
            pfp.SetBool("boolean", true);
            pfp.Apply();
            
            var fs2 = new FileStream(FilesPath+"propsTest.txt", FileMode.Open, FileAccess.ReadWrite);
            var pfp2 = new PropertyFileProxy(fs2, NotExistProperty.Exception, IncorrectPropertyType.Exception);

            int value1 = -1;
            bool value2 = false;
            pfp2.GetInt("seed", ref value1);
            pfp2.GetBool("boolean", ref value2);
            Assert.AreEqual("658True", value1.ToString() + value2.ToString());

            pfp2.SetInt("seed", 123);
            pfp2.Apply();
        }

        [TearDown]
        public void DeleteAllFiles()
        {
            File.Delete(FilesPath+"propsTest.txt");
        }
    }
}
