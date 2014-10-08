using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VVVV.Packs.Time
{
    public class TimeSerializer :JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var t = (Time)value;
            writer.WritePropertyName("UTC");
            writer.WriteValue( t.UniversalTime.ToString("yyyy-MM-dd HH:mm:ss.ffff") );

            writer.WritePropertyName("ZoneId");
            writer.WriteValue(t.TimeZone.Id);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var jUtc = jsonObject.GetValue("UTC");
            string utc = (string)jUtc.ToObject(typeof(string), serializer);
            
            var jZoneId = jsonObject.GetValue("ZoneId");
            string zoneId = (string)jZoneId.ToObject(typeof(string), serializer);

            Time utcTime = Time.StringAsTime("UTC", utc, "yyyy-MM-dd HH:mm:ss.ffff");
            Time time = Time.ChangeTimezone(utcTime, zoneId);

            return time;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Time).IsAssignableFrom(objectType);
        }
    }
}
