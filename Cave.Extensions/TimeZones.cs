using System;

namespace Cave
{
    /// <summary>
    /// Provides global standartized time zone informations
    /// </summary>
    public static class TimeZones
    {
        #region predefined time zones

        /// <summary>
        /// Gets the TimeZone settings for the Greenwich Mean Time
        /// </summary>
        public static TimeZoneData GMT => new TimeZoneData("Greenwich Mean Time", "GMT", new TimeSpan(0, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Central European Time
        /// </summary>
        public static TimeZoneData CET => new TimeZoneData("Central European Time", "CET", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the French Winter Time (France)
        /// </summary>
        public static TimeZoneData FWT => new TimeZoneData("French Winter Time (France)", "FWT", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Middle European Time
        /// </summary>
        public static TimeZoneData MET => new TimeZoneData("Middle European Time", "MET", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Middle European Winter Time
        /// </summary>
        public static TimeZoneData MEWT => new TimeZoneData("Middle European Winter Time", "MEWT", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Swedish Winter Time (Sweden)
        /// </summary>
        public static TimeZoneData SWT => new TimeZoneData("Swedish Winter Time (Sweden)", "SWT", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Eastern European Time, USSR Zone 1
        /// </summary>
        public static TimeZoneData EET => new TimeZoneData("Eastern European Time, USSR Zone 1", "EET", new TimeSpan(2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Baghdad Time, USSR Zone 2
        /// </summary>
        public static TimeZoneData BT => new TimeZoneData("Baghdad Time, USSR Zone 2", "BT", new TimeSpan(3, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the USSR Zone 3
        /// </summary>
        public static TimeZoneData ZP4 => new TimeZoneData("USSR Zone 3", "ZP4", new TimeSpan(4, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the USSR Zone 4.
        /// </summary>
        public static TimeZoneData ZP5 => new TimeZoneData("USSR Zone 4.", "ZP5", new TimeSpan(5, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the USSR Zone 5.
        /// </summary>
        public static TimeZoneData ZP6 => new TimeZoneData("USSR Zone 5.", "ZP6", new TimeSpan(6, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Christmas Island Time (Australia)
        /// </summary>
        public static TimeZoneData CXT => new TimeZoneData("Christmas Island Time (Australia)", "CXT", new TimeSpan(7, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the China Coast Time, USSR Zone 7
        /// </summary>
        public static TimeZoneData CCT => new TimeZoneData("China Coast Time, USSR Zone 7", "CCT", new TimeSpan(8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Australian Western Standard Time
        /// </summary>
        public static TimeZoneData AWST => new TimeZoneData("Australian Western Standard Time", "AWST", new TimeSpan(8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Western Standard Time (Australia)
        /// </summary>
        public static TimeZoneData WST => new TimeZoneData("Western Standard Time (Australia)", "WST", new TimeSpan(8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Japan Standard Time, USSR Zone 8
        /// </summary>
        public static TimeZoneData JST => new TimeZoneData("Japan Standard Time, USSR Zone 8", "JST", new TimeSpan(9, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the East Australian Standard Time
        /// </summary>
        public static TimeZoneData EAST => new TimeZoneData("East Australian Standard Time", "EAST", new TimeSpan(10, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Guam Standard Time, USSR Zone 9
        /// </summary>
        public static TimeZoneData GST => new TimeZoneData("Guam Standard Time, USSR Zone 9", "GST", new TimeSpan(10, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the International Date Line East
        /// </summary>
        public static TimeZoneData IDLE => new TimeZoneData("International Date Line East", "IDLE", new TimeSpan(12, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the New Zealand Standard Time
        /// </summary>
        public static TimeZoneData NZST => new TimeZoneData("New Zealand Standard Time", "NZST", new TimeSpan(12, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the New Zealand Time
        /// </summary>
        public static TimeZoneData NZT => new TimeZoneData("New Zealand Time", "NZT", new TimeSpan(12, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the West Africa Time
        /// </summary>
        public static TimeZoneData WAT => new TimeZoneData("West Africa Time", "WAT", new TimeSpan(-1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Azores Time
        /// </summary>
        public static TimeZoneData AT => new TimeZoneData("Azores Time", "AT", new TimeSpan(-2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Atlantic Standard Time (Canada)
        /// </summary>
        public static TimeZoneData AST => new TimeZoneData("Atlantic Standard Time (Canada)", "AST", new TimeSpan(-4, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Eastern Standard Time (USA &amp; Canada)
        /// </summary>
        public static TimeZoneData EST => new TimeZoneData("Eastern Standard Time (USA & Canada)", "EST", new TimeSpan(-5, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Central Standard Time (USA &amp; Canada)
        /// </summary>
        public static TimeZoneData CST => new TimeZoneData("Central Standard Time (USA & Canada)", "CST", new TimeSpan(-6, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Mountain Standard Time (USA &amp; Canada)
        /// </summary>
        public static TimeZoneData MST => new TimeZoneData("Mountain Standard Time (USA & Canada)", "MST", new TimeSpan(-7, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Pacific Standard Time (USA &amp; Canada)
        /// </summary>
        public static TimeZoneData PST => new TimeZoneData("Pacific Standard Time (USA & Canada)", "PST", new TimeSpan(-8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Alaska Standard Time (USA)
        /// </summary>
        public static TimeZoneData AKST => new TimeZoneData("Alaska Standard Time (USA)", "AKST", new TimeSpan(-9, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Yukon Standard Time (Canada)
        /// </summary>
        public static TimeZoneData YST => new TimeZoneData("Yukon Standard Time (Canada)", "YST", new TimeSpan(-9, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Hawaii Standard Time
        /// </summary>
        public static TimeZoneData HST => new TimeZoneData("Hawaii Standard Time", "HST", new TimeSpan(-10, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Hawaii-Aleutian Standard Time (USA)
        /// </summary>
        public static TimeZoneData HAST => new TimeZoneData("Hawaii-Aleutian Standard Time (USA)", "HAST", new TimeSpan(-10, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Central Alaska Time
        /// </summary>
        public static TimeZoneData CAT => new TimeZoneData("Central Alaska Time", "CAT", new TimeSpan(-10, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Nome Time
        /// </summary>
        public static TimeZoneData NT => new TimeZoneData("Nome Time", "NT", new TimeSpan(-11, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the International Date Line West
        /// </summary>
        public static TimeZoneData IDLW => new TimeZoneData("International Date Line West", "IDLW", new TimeSpan(-11, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Atlantic Daylight Time (Canada)
        /// </summary>
        public static TimeZoneData ADT => new TimeZoneData("Atlantic Daylight Time (Canada)", "ADT", new TimeSpan(-3, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Eastern Daylight Time (USA &amp; Canada)
        /// </summary>
        public static TimeZoneData EDT => new TimeZoneData("Eastern Daylight Time (USA & Canada)", "EDT", new TimeSpan(-4, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Central Daylight Time (USA &amp; Canada)
        /// </summary>
        public static TimeZoneData CDT => new TimeZoneData("Central Daylight Time (USA & Canada)", "CDT", new TimeSpan(-5, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Mountain Daylight Time (USA &amp; Canada)
        /// </summary>
        public static TimeZoneData MDT => new TimeZoneData("Mountain Daylight Time (USA & Canada)", "MDT", new TimeSpan(-6, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Pacific Daylight Time (USA &amp; Canada)
        /// </summary>
        public static TimeZoneData PDT => new TimeZoneData("Pacific Daylight Time (USA & Canada)", "PDT", new TimeSpan(-7, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Alaska Daylight Time(USA)
        /// </summary>
        public static TimeZoneData AKDT => new TimeZoneData("Alaska Daylight Time (USA)", "AKDT", new TimeSpan(-8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Yukon Daylight Time (Canada)
        /// </summary>
        public static TimeZoneData YDT => new TimeZoneData("Yukon Daylight Time (Canada)", "YDT", new TimeSpan(-8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Hawaii-Aleutian Daylight Time (USA)
        /// </summary>
        public static TimeZoneData HADT => new TimeZoneData("Hawaii-Aleutian Daylight Time (USA)", "HADT", new TimeSpan(-8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Hawaii Daylight Time (USA)
        /// </summary>
        public static TimeZoneData HDT => new TimeZoneData("Hawaii Daylight Time (USA)", "HDT", new TimeSpan(-9, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the British Summer Time (UK)
        /// </summary>
        public static TimeZoneData BST => new TimeZoneData("British Summer Time (UK)", "BST", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Irish Summer Time (Ireland)
        /// </summary>
        public static TimeZoneData IST => new TimeZoneData("Irish Summer Time (Ireland)", "IST", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Western European Daylight Time
        /// </summary>
        public static TimeZoneData WEDT => new TimeZoneData("Western European Daylight Time", "WEDT", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Western European Summer Time
        /// </summary>
        public static TimeZoneData WEST => new TimeZoneData("Western European Summer Time", "WEST", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Central European Daylight Time
        /// </summary>
        public static TimeZoneData CEDT => new TimeZoneData("Central European Daylight Time", "CEDT", new TimeSpan(2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Central European Summer Time
        /// </summary>
        public static TimeZoneData CEST => new TimeZoneData("Central European Summer Time", "CEST", new TimeSpan(2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Middle European Summer Time
        /// </summary>
        public static TimeZoneData MEST => new TimeZoneData("Middle European Summer Time", "MEST", new TimeSpan(2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Mitteleuropäische Sommerzeit
        /// </summary>
        public static TimeZoneData MESZ => new TimeZoneData("Mitteleuropäische Sommerzeit", "MESZ", new TimeSpan(2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Swedish Summer Time (Sweden)
        /// </summary>
        public static TimeZoneData SST => new TimeZoneData("Swedish Summer Time (Sweden)", "SST", new TimeSpan(2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the French Summer Time (France)
        /// </summary>
        public static TimeZoneData FST => new TimeZoneData("French Summer Time (France)", "FST", new TimeSpan(2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Eastern European Daylight Time
        /// </summary>
        public static TimeZoneData EEDT => new TimeZoneData("Eastern European Daylight Time", "EEDT", new TimeSpan(3, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Eastern European Summer Time
        /// </summary>
        public static TimeZoneData EEST => new TimeZoneData("Eastern European Summer Time", "EEST", new TimeSpan(3, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the West Australian Daylight Time
        /// </summary>
        public static TimeZoneData WADT => new TimeZoneData("West Australian Daylight Time", "WADT", new TimeSpan(8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Eastern Australian Daylight Time
        /// </summary>
        public static TimeZoneData EADT => new TimeZoneData("Eastern Australian Daylight Time", "EADT", new TimeSpan(11, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the New Zealand Daylight Time
        /// </summary>
        public static TimeZoneData NZDT => new TimeZoneData("New Zealand Daylight Time", "NZDT", new TimeSpan(13, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Mitteleuropäische Zeit
        /// </summary>
        public static TimeZoneData MEZ => new TimeZoneData("Mitteleuropäische Zeit", "MEZ", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Australian Central Daylight Time
        /// </summary>
        public static TimeZoneData ACDT => new TimeZoneData("Australian Central Daylight Time", "ACDT", new TimeSpan(10, 30, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Australian Central Standard Time
        /// </summary>
        public static TimeZoneData ACST => new TimeZoneData("Australian Central Standard Time", "ACST", new TimeSpan(9, 30, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Newfoundland Daylight Time
        /// </summary>
        public static TimeZoneData NDT => new TimeZoneData("Newfoundland Daylight Time", "NDT", new TimeSpan(-2, -30, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Norfolk (Island) Time
        /// </summary>
        public static TimeZoneData NFT => new TimeZoneData("Norfolk (Island) Time", "NFT", new TimeSpan(11, 30, 0));

        /// <summary>
        /// Gets the TimeZone settings for the Newfoundland Standard Time
        /// </summary>
        public static TimeZoneData NST => new TimeZoneData("Newfoundland Standard Time", "NST", new TimeSpan(-3, -30, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Alfa
        /// </summary>
        public static TimeZoneData A => new TimeZoneData("Alfa", "A", new TimeSpan(1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Bravo
        /// </summary>
        public static TimeZoneData B => new TimeZoneData("Bravo", "B", new TimeSpan(2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Charlie
        /// </summary>
        public static TimeZoneData C => new TimeZoneData("Charlie", "C", new TimeSpan(3, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Delta
        /// </summary>
        public static TimeZoneData D => new TimeZoneData("Delta", "D", new TimeSpan(4, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Echo
        /// </summary>
        public static TimeZoneData E => new TimeZoneData("Echo", "E", new TimeSpan(5, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Foxtrot
        /// </summary>
        public static TimeZoneData F => new TimeZoneData("Foxtrot", "F", new TimeSpan(6, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Golf
        /// </summary>
        public static TimeZoneData G => new TimeZoneData("Golf", "G", new TimeSpan(7, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Hotel
        /// </summary>
        public static TimeZoneData H => new TimeZoneData("Hotel", "H", new TimeSpan(8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone India
        /// </summary>
        public static TimeZoneData I => new TimeZoneData("India", "I", new TimeSpan(9, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Kilo
        /// </summary>
        public static TimeZoneData K => new TimeZoneData("Kilo", "K", new TimeSpan(10, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Lima
        /// </summary>
        public static TimeZoneData L => new TimeZoneData("Lima", "L", new TimeSpan(11, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Mike
        /// </summary>
        public static TimeZoneData M => new TimeZoneData("Mike", "M", new TimeSpan(12, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone November
        /// </summary>
        public static TimeZoneData N => new TimeZoneData("November", "N", new TimeSpan(-1, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Oscar
        /// </summary>
        public static TimeZoneData O => new TimeZoneData("Oscar", "O", new TimeSpan(-2, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Papa
        /// </summary>
        public static TimeZoneData P => new TimeZoneData("Papa", "P", new TimeSpan(-3, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Quebec
        /// </summary>
        public static TimeZoneData Q => new TimeZoneData("Quebec", "Q", new TimeSpan(-4, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Romeo
        /// </summary>
        public static TimeZoneData R => new TimeZoneData("Romeo", "R", new TimeSpan(-5, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Sierra
        /// </summary>
        public static TimeZoneData S => new TimeZoneData("Sierra", "S", new TimeSpan(-6, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Tango
        /// </summary>
        public static TimeZoneData T => new TimeZoneData("Tango", "T", new TimeSpan(-7, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Uniform
        /// </summary>
        public static TimeZoneData U => new TimeZoneData("Uniform", "U", new TimeSpan(-8, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Victor
        /// </summary>
        public static TimeZoneData V => new TimeZoneData("Victor", "V", new TimeSpan(-9, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Whiskey
        /// </summary>
        public static TimeZoneData W => new TimeZoneData("Whiskey", "W", new TimeSpan(-10, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Xray
        /// </summary>
        public static TimeZoneData X => new TimeZoneData("Xray", "X", new TimeSpan(-11, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Yankee
        /// </summary>
        public static TimeZoneData Y => new TimeZoneData("Yankee", "Y", new TimeSpan(-12, 0, 0));

        /// <summary>
        /// Gets the TimeZone settings for the military zone Zulu
        /// </summary>
        public static TimeZoneData Z => new TimeZoneData("Zulu", "Z", TimeSpan.Zero);

        #endregion

        #region GetList()

        /// <summary>
        /// Gets a list of all TimeZones
        /// </summary>
        /// <returns>Returns an array of TimeZoneData instances.</returns>
        public static TimeZoneData[] GetList()
        {
            return new TimeZoneData[]
            {
                GMT,
                CET,
                FWT,
                MET,
                MEWT,
                SWT,
                EET,
                BT,
                ZP4,
                ZP5,
                ZP6,
                CXT,
                CCT,
                AWST,
                WST,
                JST,
                EAST,
                GST,
                IDLE,
                NZST,
                NZT,
                WAT,
                AT,
                AST,
                EST,
                CST,
                MST,
                PST,
                AKST,
                YST,
                HST,
                HAST,
                CAT,
                NT,
                IDLW,
                ADT,
                EDT,
                CDT,
                MDT,
                PDT,
                AKDT,
                YDT,
                HADT,
                HDT,
                BST,
                IST,
                WEDT,
                WEST,
                CEDT,
                CEST,
                MEST,
                MESZ,
                SST,
                FST,
                EEDT,
                EEST,
                WADT,
                EADT,
                NZDT,
                MEZ,
                ACDT,
                ACST,
                NDT,
                NFT,
                NST,
                A,
                B,
                C,
                D,
                E,
                F,
                G,
                H,
                I,
                K,
                L,
                M,
                N,
                O,
                P,
                Q,
                R,
                S,
                T,
                U,
                V,
                W,
                X,
                Y,
                Z,
            };
        }
        #endregion

        #region GetNames()

        /// <summary>
        /// Gets a list of all TimeZones
        /// </summary>
        /// <returns>Returns an array of time zone names.</returns>
        public static string[] GetNames()
        {
            return new string[]
            {
                "GMT",
                "CET",
                "FWT",
                "MET",
                "MEWT",
                "SWT",
                "EET",
                "BT",
                "ZP4",
                "ZP5",
                "ZP6",
                "CXT",
                "CCT",
                "AWST",
                "WST",
                "JST",
                "EAST",
                "GST",
                "IDLE",
                "NZST",
                "NZT",
                "WAT",
                "AT",
                "AST",
                "EST",
                "CST",
                "MST",
                "PST",
                "AKST",
                "YST",
                "HST",
                "HAST",
                "CAT",
                "NT",
                "IDLW",
                "ADT",
                "EDT",
                "CDT",
                "MDT",
                "PDT",
                "AKDT",
                "YDT",
                "HADT",
                "HDT",
                "BST",
                "IST",
                "WEDT",
                "WEST",
                "CEDT",
                "CEST",
                "MEST",
                "MESZ",
                "SST",
                "FST",
                "EEDT",
                "EEST",
                "WADT",
                "EADT",
                "NZDT",
                "MEZ",
                "ACDT",
                "ACST",
                "NDT",
                "NFT",
                "NST",
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G",
                "H",
                "I",
                "K",
                "L",
                "M",
                "N",
                "O",
                "P",
                "Q",
                "R",
                "S",
                "T",
                "U",
                "V",
                "W",
                "X",
                "Y",
                "Z",
            };
        }

        #endregion
    }
}
