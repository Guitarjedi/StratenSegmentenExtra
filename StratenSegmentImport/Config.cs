using System;
using System.Collections.Generic;
using System.Text;

namespace ImportFromFiles
{
    public static class Config
    {

        public static string TaalCode { get; set; } = "nl";
        //public static string Path { get; set; } = @"C:\Users\Matthijs\source\repos\StratenSegmentenReDo\StratenSegmentImport\TestData";
        public static string Path {get; set;} = @"C:\PR3.StratenSegmentenData\WRData2";
        public static string StoragePath { get; set; } = @"C:\Users\Matthijs\StratenSegmenten";
        public const string StraatIdGemeenteId = "WRGemeenteID.csv";
        public const string Straatnamen = "WRstraatnamen.csv";
        public const string ProvincieIds = "ProvincieIDsVlaanderen.csv";
        public const string ProvincieInfo = "ProvincieInfo.csv";
        public const string Gemeentenamen = "WRGemeentenaam.csv";
        public const string Data = "WRdata.csv";
    }
}
