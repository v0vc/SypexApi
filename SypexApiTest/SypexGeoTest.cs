// This file contains my intellectual property. Release of this file requires prior approval from me.
// 
// Copyright (c) 2015, v0v. All Rights Reserved

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SypexApi;

namespace SypexApiTest
{
    [TestClass]
    public class SypexGeoTest
    {
        #region Constants

        private const string baseIp = "77.88.21.3";
        private const string dbFile = "SxGeoCity.dat";

        #endregion

        #region Static and Readonly Fields

        private readonly string fullDb;

        #endregion

        #region Constructors

        public SypexGeoTest()
        {
            DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            if (directoryInfo == null)
            {
                return;
            }
            var fn = new FileInfo(Path.Combine(directoryInfo.FullName, "db", dbFile));
            fullDb = fn.FullName;
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestGeoIpLocator()
        {
            var api = new SypexGeo(fullDb);
            Location location = api.GetLocationByIp(baseIp);
            Assert.AreEqual(524894, location.Region.Id);
        }

        #endregion
    }
}
