// This file contains my intellectual property. Release of this file requires prior approval from me.
// 
// Copyright (c) 2015, v0v. All Rights Reserved

namespace SypexApi.Model
{
    public class Region
    {
        #region Properties

        public short CountryId { get; set; }
        public short Id { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string NameEn { get; set; }
        public string NameRu { get; set; }
        public short RegionSeek { get; set; }

        #endregion
    }
}
