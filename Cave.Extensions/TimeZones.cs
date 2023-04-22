using System;

namespace Cave;

/// <summary>Gets global standartized time zone informations.</summary>
public static class TimeZones
{
    #region Static

    /// <summary>Gets a list of all TimeZones.</summary>
    /// <returns>Returns an array of TimeZoneData instances.</returns>
    public static TimeZoneData[] GetList() =>
        new[]
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
            Z
        };

    /// <summary>Gets a list of all TimeZones.</summary>
    /// <returns>Returns an array of time zone names.</returns>
    public static string[] GetNames() =>
        new[]
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
            "Z"
        };

    /// <summary>Gets the TimeZone settings for the military zone Alfa.</summary>
    public static TimeZoneData A => new("Alfa", "A", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Australian Central Daylight Time.</summary>
    public static TimeZoneData ACDT => new("Australian Central Daylight Time", "ACDT", new(10, 30, 0));

    /// <summary>Gets the TimeZone settings for the Australian Central Standard Time.</summary>
    public static TimeZoneData ACST => new("Australian Central Standard Time", "ACST", new(9, 30, 0));

    /// <summary>Gets the TimeZone settings for the Atlantic Daylight Time (Canada).</summary>
    public static TimeZoneData ADT => new("Atlantic Daylight Time (Canada)", "ADT", new(-3, 0, 0));

    /// <summary>Gets the TimeZone settings for the Alaska Daylight Time(USA).</summary>
    public static TimeZoneData AKDT => new("Alaska Daylight Time (USA)", "AKDT", new(-8, 0, 0));

    /// <summary>Gets the TimeZone settings for the Alaska Standard Time (USA).</summary>
    public static TimeZoneData AKST => new("Alaska Standard Time (USA)", "AKST", new(-9, 0, 0));

    /// <summary>Gets the TimeZone settings for the Atlantic Standard Time (Canada).</summary>
    public static TimeZoneData AST => new("Atlantic Standard Time (Canada)", "AST", new(-4, 0, 0));

    /// <summary>Gets the TimeZone settings for the Azores Time.</summary>
    public static TimeZoneData AT => new("Azores Time", "AT", new(-2, 0, 0));

    /// <summary>Gets the TimeZone settings for the Australian Western Standard Time.</summary>
    public static TimeZoneData AWST => new("Australian Western Standard Time", "AWST", new(8, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Bravo.</summary>
    public static TimeZoneData B => new("Bravo", "B", new(2, 0, 0));

    /// <summary>Gets the TimeZone settings for the British Summer Time (UK).</summary>
    public static TimeZoneData BST => new("British Summer Time (UK)", "BST", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Baghdad Time, USSR Zone 2.</summary>
    public static TimeZoneData BT => new("Baghdad Time, USSR Zone 2", "BT", new(3, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Charlie.</summary>
    public static TimeZoneData C => new("Charlie", "C", new(3, 0, 0));

    /// <summary>Gets the TimeZone settings for the Central Alaska Time.</summary>
    public static TimeZoneData CAT => new("Central Alaska Time", "CAT", new(-10, 0, 0));

    /// <summary>Gets the TimeZone settings for the China Coast Time, USSR Zone 7.</summary>
    public static TimeZoneData CCT => new("China Coast Time, USSR Zone 7", "CCT", new(8, 0, 0));

    /// <summary>Gets the TimeZone settings for the Central Daylight Time (USA &amp; Canada).</summary>
    public static TimeZoneData CDT => new("Central Daylight Time (USA & Canada)", "CDT", new(-5, 0, 0));

    /// <summary>Gets the TimeZone settings for the Central European Daylight Time.</summary>
    public static TimeZoneData CEDT => new("Central European Daylight Time", "CEDT", new(2, 0, 0));

    /// <summary>Gets the TimeZone settings for the Central European Summer Time.</summary>
    public static TimeZoneData CEST => new("Central European Summer Time", "CEST", new(2, 0, 0));

    /// <summary>Gets the TimeZone settings for the Central European Time.</summary>
    public static TimeZoneData CET => new("Central European Time", "CET", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Central Standard Time (USA &amp; Canada).</summary>
    public static TimeZoneData CST => new("Central Standard Time (USA & Canada)", "CST", new(-6, 0, 0));

    /// <summary>Gets the TimeZone settings for the Christmas Island Time (Australia).</summary>
    public static TimeZoneData CXT => new("Christmas Island Time (Australia)", "CXT", new(7, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Delta.</summary>
    public static TimeZoneData D => new("Delta", "D", new(4, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Echo.</summary>
    public static TimeZoneData E => new("Echo", "E", new(5, 0, 0));

    /// <summary>Gets the TimeZone settings for the Eastern Australian Daylight Time.</summary>
    public static TimeZoneData EADT => new("Eastern Australian Daylight Time", "EADT", new(11, 0, 0));

    /// <summary>Gets the TimeZone settings for the East Australian Standard Time.</summary>
    public static TimeZoneData EAST => new("East Australian Standard Time", "EAST", new(10, 0, 0));

    /// <summary>Gets the TimeZone settings for the Eastern Daylight Time (USA &amp; Canada).</summary>
    public static TimeZoneData EDT => new("Eastern Daylight Time (USA & Canada)", "EDT", new(-4, 0, 0));

    /// <summary>Gets the TimeZone settings for the Eastern European Daylight Time.</summary>
    public static TimeZoneData EEDT => new("Eastern European Daylight Time", "EEDT", new(3, 0, 0));

    /// <summary>Gets the TimeZone settings for the Eastern European Summer Time.</summary>
    public static TimeZoneData EEST => new("Eastern European Summer Time", "EEST", new(3, 0, 0));

    /// <summary>Gets the TimeZone settings for the Eastern European Time, USSR Zone 1.</summary>
    public static TimeZoneData EET => new("Eastern European Time, USSR Zone 1", "EET", new(2, 0, 0));

    /// <summary>Gets the TimeZone settings for the Eastern Standard Time (USA &amp; Canada).</summary>
    public static TimeZoneData EST => new("Eastern Standard Time (USA & Canada)", "EST", new(-5, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Foxtrot.</summary>
    public static TimeZoneData F => new("Foxtrot", "F", new(6, 0, 0));

    /// <summary>Gets the TimeZone settings for the French Summer Time (France).</summary>
    public static TimeZoneData FST => new("French Summer Time (France)", "FST", new(2, 0, 0));

    /// <summary>Gets the TimeZone settings for the French Winter Time (France).</summary>
    public static TimeZoneData FWT => new("French Winter Time (France)", "FWT", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Golf.</summary>
    public static TimeZoneData G => new("Golf", "G", new(7, 0, 0));

    /// <summary>Gets the TimeZone settings for the Greenwich Mean Time.</summary>
    public static TimeZoneData GMT => new("Greenwich Mean Time", "GMT", new(0, 0, 0));

    /// <summary>Gets the TimeZone settings for the Guam Standard Time, USSR Zone 9.</summary>
    public static TimeZoneData GST => new("Guam Standard Time, USSR Zone 9", "GST", new(10, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Hotel.</summary>
    public static TimeZoneData H => new("Hotel", "H", new(8, 0, 0));

    /// <summary>Gets the TimeZone settings for the Hawaii-Aleutian Daylight Time (USA).</summary>
    public static TimeZoneData HADT => new("Hawaii-Aleutian Daylight Time (USA)", "HADT", new(-8, 0, 0));

    /// <summary>Gets the TimeZone settings for the Hawaii-Aleutian Standard Time (USA).</summary>
    public static TimeZoneData HAST => new("Hawaii-Aleutian Standard Time (USA)", "HAST", new(-10, 0, 0));

    /// <summary>Gets the TimeZone settings for the Hawaii Daylight Time (USA).</summary>
    public static TimeZoneData HDT => new("Hawaii Daylight Time (USA)", "HDT", new(-9, 0, 0));

    /// <summary>Gets the TimeZone settings for the Hawaii Standard Time.</summary>
    public static TimeZoneData HST => new("Hawaii Standard Time", "HST", new(-10, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone India.</summary>
    public static TimeZoneData I => new("India", "I", new(9, 0, 0));

    /// <summary>Gets the TimeZone settings for the International Date Line East.</summary>
    public static TimeZoneData IDLE => new("International Date Line East", "IDLE", new(12, 0, 0));

    /// <summary>Gets the TimeZone settings for the International Date Line West.</summary>
    public static TimeZoneData IDLW => new("International Date Line West", "IDLW", new(-11, 0, 0));

    /// <summary>Gets the TimeZone settings for the Irish Summer Time (Ireland).</summary>
    public static TimeZoneData IST => new("Irish Summer Time (Ireland)", "IST", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Japan Standard Time, USSR Zone 8.</summary>
    public static TimeZoneData JST => new("Japan Standard Time, USSR Zone 8", "JST", new(9, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Kilo.</summary>
    public static TimeZoneData K => new("Kilo", "K", new(10, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Lima.</summary>
    public static TimeZoneData L => new("Lima", "L", new(11, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Mike.</summary>
    public static TimeZoneData M => new("Mike", "M", new(12, 0, 0));

    /// <summary>Gets the TimeZone settings for the Mountain Daylight Time (USA &amp; Canada).</summary>
    public static TimeZoneData MDT => new("Mountain Daylight Time (USA & Canada)", "MDT", new(-6, 0, 0));

    /// <summary>Gets the TimeZone settings for the Middle European Summer Time.</summary>
    public static TimeZoneData MEST => new("Middle European Summer Time", "MEST", new(2, 0, 0));

    /// <summary>Gets the TimeZone settings for the Mitteleuropäische Sommerzeit.</summary>
    public static TimeZoneData MESZ => new("Mitteleuropäische Sommerzeit", "MESZ", new(2, 0, 0));

    /// <summary>Gets the TimeZone settings for the Middle European Time.</summary>
    public static TimeZoneData MET => new("Middle European Time", "MET", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Middle European Winter Time.</summary>
    public static TimeZoneData MEWT => new("Middle European Winter Time", "MEWT", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Mitteleuropäische Zeit.</summary>
    public static TimeZoneData MEZ => new("Mitteleuropäische Zeit", "MEZ", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Mountain Standard Time (USA &amp; Canada).</summary>
    public static TimeZoneData MST => new("Mountain Standard Time (USA & Canada)", "MST", new(-7, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone November.</summary>
    public static TimeZoneData N => new("November", "N", new(-1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Newfoundland Daylight Time.</summary>
    public static TimeZoneData NDT => new("Newfoundland Daylight Time", "NDT", new(-2, -30, 0));

    /// <summary>Gets the TimeZone settings for the Norfolk (Island) Time.</summary>
    public static TimeZoneData NFT => new("Norfolk (Island) Time", "NFT", new(11, 30, 0));

    /// <summary>Gets the TimeZone settings for the Newfoundland Standard Time.</summary>
    public static TimeZoneData NST => new("Newfoundland Standard Time", "NST", new(-3, -30, 0));

    /// <summary>Gets the TimeZone settings for the Nome Time.</summary>
    public static TimeZoneData NT => new("Nome Time", "NT", new(-11, 0, 0));

    /// <summary>Gets the TimeZone settings for the New Zealand Daylight Time.</summary>
    public static TimeZoneData NZDT => new("New Zealand Daylight Time", "NZDT", new(13, 0, 0));

    /// <summary>Gets the TimeZone settings for the New Zealand Standard Time.</summary>
    public static TimeZoneData NZST => new("New Zealand Standard Time", "NZST", new(12, 0, 0));

    /// <summary>Gets the TimeZone settings for the New Zealand Time.</summary>
    public static TimeZoneData NZT => new("New Zealand Time", "NZT", new(12, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Oscar.</summary>
    public static TimeZoneData O => new("Oscar", "O", new(-2, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Papa.</summary>
    public static TimeZoneData P => new("Papa", "P", new(-3, 0, 0));

    /// <summary>Gets the TimeZone settings for the Pacific Daylight Time (USA &amp; Canada).</summary>
    public static TimeZoneData PDT => new("Pacific Daylight Time (USA & Canada)", "PDT", new(-7, 0, 0));

    /// <summary>Gets the TimeZone settings for the Pacific Standard Time (USA &amp; Canada).</summary>
    public static TimeZoneData PST => new("Pacific Standard Time (USA & Canada)", "PST", new(-8, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Quebec.</summary>
    public static TimeZoneData Q => new("Quebec", "Q", new(-4, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Romeo.</summary>
    public static TimeZoneData R => new("Romeo", "R", new(-5, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Sierra.</summary>
    public static TimeZoneData S => new("Sierra", "S", new(-6, 0, 0));

    /// <summary>Gets the TimeZone settings for the Swedish Summer Time (Sweden).</summary>
    public static TimeZoneData SST => new("Swedish Summer Time (Sweden)", "SST", new(2, 0, 0));

    /// <summary>Gets the TimeZone settings for the Swedish Winter Time (Sweden).</summary>
    public static TimeZoneData SWT => new("Swedish Winter Time (Sweden)", "SWT", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Tango.</summary>
    public static TimeZoneData T => new("Tango", "T", new(-7, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Uniform.</summary>
    public static TimeZoneData U => new("Uniform", "U", new(-8, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Victor.</summary>
    public static TimeZoneData V => new("Victor", "V", new(-9, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Whiskey.</summary>
    public static TimeZoneData W => new("Whiskey", "W", new(-10, 0, 0));

    /// <summary>Gets the TimeZone settings for the West Australian Daylight Time.</summary>
    public static TimeZoneData WADT => new("West Australian Daylight Time", "WADT", new(8, 0, 0));

    /// <summary>Gets the TimeZone settings for the West Africa Time.</summary>
    public static TimeZoneData WAT => new("West Africa Time", "WAT", new(-1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Western European Daylight Time.</summary>
    public static TimeZoneData WEDT => new("Western European Daylight Time", "WEDT", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Western European Summer Time.</summary>
    public static TimeZoneData WEST => new("Western European Summer Time", "WEST", new(1, 0, 0));

    /// <summary>Gets the TimeZone settings for the Western Standard Time (Australia).</summary>
    public static TimeZoneData WST => new("Western Standard Time (Australia)", "WST", new(8, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Xray.</summary>
    public static TimeZoneData X => new("Xray", "X", new(-11, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Yankee.</summary>
    public static TimeZoneData Y => new("Yankee", "Y", new(-12, 0, 0));

    /// <summary>Gets the TimeZone settings for the Yukon Daylight Time (Canada).</summary>
    public static TimeZoneData YDT => new("Yukon Daylight Time (Canada)", "YDT", new(-8, 0, 0));

    /// <summary>Gets the TimeZone settings for the Yukon Standard Time (Canada).</summary>
    public static TimeZoneData YST => new("Yukon Standard Time (Canada)", "YST", new(-9, 0, 0));

    /// <summary>Gets the TimeZone settings for the military zone Zulu.</summary>
    public static TimeZoneData Z => new("Zulu", "Z", TimeSpan.Zero);

    /// <summary>Gets the TimeZone settings for the USSR Zone 3.</summary>
    public static TimeZoneData ZP4 => new("USSR Zone 3", "ZP4", new(4, 0, 0));

    /// <summary>Gets the TimeZone settings for the USSR Zone 4.</summary>
    public static TimeZoneData ZP5 => new("USSR Zone 4.", "ZP5", new(5, 0, 0));

    /// <summary>Gets the TimeZone settings for the USSR Zone 5.</summary>
    public static TimeZoneData ZP6 => new("USSR Zone 5.", "ZP6", new(6, 0, 0));

    #endregion
}
