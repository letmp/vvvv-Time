using System;
using System.Collections.Generic;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.Time
{
    internal class TimeZoneManager
    {
        // abbreviations of timezones like CET are too ambiguos to be used- a certain Timezone 
        // can have multiple Abbreviations and some Abbreviations can refer to more than one timezone
        public static void Update()
        {
            var timezones = new List<string>();
            foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones()) timezones.Add(z.Id);
            EnumManager.UpdateEnum("TimezoneEnum", "UTC", timezones.ToArray());
        }
    }



/*
"UTC-11", 
"Hawaiian Standard Time", 
"Alaskan Standard Time", 
"Pacific Standard Time (Mexico)", 
"Pacific Standard Time", 
"US Mountain Standard Time", 
"Mountain Standard Time (Mexico)", 
"Mountain Standard Time", 
"Central Standard Time", 
"Central Standard Time (Mexico)", 
"Central America Standard Time", 
"Canada Central Standard Time", 
"SA Pacific Standard Time", 
"Eastern Standard Time", 
"US Eastern Standard Time", 
"Venezuela Standard Time", 
"Paraguay Standard Time", 
"Atlantic Standard Time", 
"Central Brazilian Standard Time", 
"SA Western Standard Time", 
"Pacific SA Standard Time", 
"Newfoundland Standard Time", 
"E. South America Standard Time", 
"Argentina Standard Time", 
"SA Eastern Standard Time", 
"Greenland Standard Time", 
"Montevideo Standard Time", 
"Bahia Standard Time", 
"UTC-02", 
"Mid-Atlantic Standard Time", 
"Azores Standard Time", 
"Cape Verde Standard Time", 
"Morocco Standard Time", 
"GMT Standard Time", 
"UTC", 
"Greenwich Standard Time", 
"W. Europe Standard Time", 
"Central Europe Standard Time", 
"Romance Standard Time", 
"Central European Standard Time", 
"W. Central Africa Standard Time", 
"Namibia Standard Time", 
"Libya Standard Time", 
"GTB Standard Time", 
"Middle East Standard Time", 
"Syria Standard Time", 
"South Africa Standard Time", 
"FLE Standard Time", 
"Turkey Standard Time", 
"Israel Standard Time", 
"Egypt Standard Time", 
"E. Europe Standard Time", 
"Jordan Standard Time", 
"Arabic Standard Time", 
"Kaliningrad Standard Time", 
"Arab Standard Time", 
"E. Africa Standard Time", 
"Iran Standard Time", 
"Arabian Standard Time", 
"Azerbaijan Standard Time", 
"Caucasus Standard Time", 
"Russian Standard Time", 
"Mauritius Standard Time", 
"Georgian Standard Time", 
"Afghanistan Standard Time", 
"West Asia Standard Time", 
"Pakistan Standard Time", 
"India Standard Time", 
"Sri Lanka Standard Time", 
"Nepal Standard Time", 
"Central Asia Standard Time", 
"Bangladesh Standard Time", 
"Ekaterinburg Standard Time", 
"Myanmar Standard Time", 
"SE Asia Standard Time", 
"N. Central Asia Standard Time", 
"North Asia Standard Time", 
"Singapore Standard Time", 
"China Standard Time", 
"W. Australia Standard Time", 
"Taipei Standard Time", 
"Ulaanbaatar Standard Time", 
"North Asia East Standard Time", 
"Tokyo Standard Time", 
"Korea Standard Time", 
"Cen. Australia Standard Time", 
"AUS Central Standard Time", 
"E. Australia Standard Time", 
"AUS Eastern Standard Time", 
"West Pacific Standard Time", 
"Tasmania Standard Time", 
"Yakutsk Standard Time", 
"Central Pacific Standard Time", 
"Vladivostok Standard Time", 
"New Zealand Standard Time", 
"Fiji Standard Time", 
"UTC+12", 
"Magadan Standard Time", 
"Kamchatka Standard Time", 
"Tonga Standard Time", 
"Samoa Standard Time"
 */

}


