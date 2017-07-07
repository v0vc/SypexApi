// This file contains my intellectual property. Release of this file requires prior approval from me.
// 
// Copyright (c) 2015, v0v. All Rights Reserved

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SypexApi
{
    public class SypexGeo
    {
        #region Static and Readonly Fields

        private static readonly List<string> headers = new List<string>
        {
            "ver",
            "ts",
            "type",
            "charset",
            "b_idx_len",
            "m_idx_len",
            "range",
            "db_items",
            "id_len",
            "max_region",
            "max_city",
            "region_size",
            "city_size",
            "max_country",
            "country_size",
            "pack_size"
        };

        private readonly string[] countryCodes =
        {
            string.Empty, "AP", "EU", "AD", "AE", "AF", "AG", "AI", "AL", "AM", "CW", "AO", "AQ",
            "AR", "AS", "AT", "AU", "AW", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BM", "BN", "BO", "BR", "BS", "BT",
            "BV", "BW", "BY", "BZ", "CA", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "CR", "CU", "CV", "CX", "CY",
            "CZ", "DE", "DJ", "DK", "DM", "DO", "DZ", "EC", "EE", "EG", "EH", "ER", "ES", "ET", "FI", "FJ", "FK", "FM", "FO", "FR", "SX",
            "GA", "GB", "GD", "GE", "GF", "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY", "HK", "HM", "HN",
            "HR", "HT", "HU", "ID", "IE", "IL", "IN", "IO", "IQ", "IR", "IS", "IT", "JM", "JO", "JP", "KE", "KG", "KH", "KI", "KM", "KN",
            "KP", "KR", "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY", "MA", "MC", "MD", "MG", "MH",
            "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ", "NA", "NC", "NE", "NF", "NG",
            "NI", "NL", "NO", "NP", "NR", "NU", "NZ", "OM", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PS", "PT", "PW",
            "PY", "QA", "RE", "RO", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR",
            "ST", "SV", "SY", "SZ", "TC", "TD", "TF", "TG", "TH", "TJ", "TK", "TM", "TN", "TO", "TL", "TR", "TT", "TV", "TW", "TZ", "UA",
            "UG", "UM", "US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU", "WF", "WS", "YE", "YT", "RS", "ZA", "ZM", "ME", "ZW",
            "A1", "XK", "O1", "AX", "GG", "IM", "JE", "BL", "MF", "BQ", "SS"
        };

        private static int b_idx_len;
        private static string b_idx_str;
        private static int block_len;
        private static string cities_db;
        private static long city_size;
        private static long country_size;
        private static byte[] db;
        private static long db_begin;
        private static long db_items;
        private static long db_ts;
        private static int db_ver;
        private static int id_len;
        private static int m_idx_len;
        private static string m_idx_str;
        private static int max_city;
        private static int max_country;
        private static int max_region;
        private static string[] pack;
        private static int range;
        private static long region_size;
        private static string regions_db;

        #endregion

        #region Constructors

        public SypexGeo(string filePath)
        {
            FilePath = filePath;
        }

        #endregion

        #region Properties

        public string FilePath { get; set; } = string.Empty;

        #endregion

        #region Methods

        public Location GetLocationByIp(string ip)
        {
            var items = new Location();
            string res = string.Empty;
            var levels = new int[32];
            using (var binr = new BinaryReader(File.OpenRead(FilePath), Encoding.UTF8))
            {
                var conId = new string(binr.ReadChars(3));
                if (string.CompareOrdinal(conId, "SxG") == 0)
                {
                    var dict = new Dictionary<string, object>();
                    object[] info = StructConverter.Unpack(">BLBBBHHLBHHLLHLH", binr.ReadBytes(37)).ToArray();
                    for (var i = 0; i < headers.Count; i++)
                    {
                        dict.Add(headers[i], info[i]);
                        Console.WriteLine(info[i]);
                    }
                    b_idx_len = Convert.ToInt32(dict["b_idx_len"]);
                    m_idx_len = Convert.ToInt32(dict["m_idx_len"]);
                    db_items = Convert.ToInt64(dict["db_items"]);
                    range = Convert.ToInt32(dict["range"]);
                    id_len = Convert.ToInt32(dict["id_len"]);
                    db_ts = Convert.ToInt64(dict["ts"]);

                    if (b_idx_len * m_idx_len * range * db_items * id_len == 0)
                    {
                        Console.WriteLine("Wrong file format!");
                    }

                    block_len = id_len + 3;
                    max_region = Convert.ToInt32(dict["max_region"]);
                    max_city = Convert.ToInt32(dict["max_city"]);
                    max_country = Convert.ToInt32(dict["max_country"]);
                    country_size = Convert.ToInt64(dict["country_size"]);
                    db_ver = Convert.ToInt32(dict["ver"]);
                    region_size = Convert.ToInt64(dict["region_size"]);
                    city_size = Convert.ToInt64(dict["city_size"]);
                    int packSize = Convert.ToInt32(dict["pack_size"]);
                    pack = packSize > 0 ? new string(binr.ReadChars(packSize)).Split('\0') : new[] { string.Empty };
                    b_idx_str = new string(binr.ReadChars(b_idx_len * 4));
                    m_idx_str = new string(binr.ReadChars(m_idx_len * 4));
                    db_begin = binr.BaseStream.Position;
                }
                else
                {
                    Console.WriteLine("Can't open database file!");
                }
            }

            return items;
        }

        #endregion
    }
}
