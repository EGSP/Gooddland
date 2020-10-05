using System.Collections.Generic;
using System.IO;
using Gasanov.Utils.SaveUtilities;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    [TestFixture]
    public class DataProviderTests
    {
        
        [Test]
        public void Constructor_ProfileDirectory_AppDataPathTest()
        {
            var profile = new DataProfile("test");
            
            var provider = new DataProvider(profile, Application.dataPath);
            
            Assert.AreEqual(Application.dataPath+"/test/", provider.RootFolder );
        }

        [Test]
        public void SaveNLoadEntities_3DataProfile_3DeserializedDataProfile()
        {
            var profiles = new List<DataProfile>()
            {
                new DataProfile("Test1"), new DataProfile("Test2"),
                new DataProfile("Test3")
            };
            
            var provider = new DataProvider(new DataProfile("TestProfile"),
                Application.dataPath+"/Tests/Data");
            
            provider.SaveEntities("Profiles/", profiles);

            var loadedProfiles = provider.LoadClassEntities<DataProfile>("Profiles/");
            
            Assert.AreEqual(profiles.Count,loadedProfiles.Count);
        }
    }
}