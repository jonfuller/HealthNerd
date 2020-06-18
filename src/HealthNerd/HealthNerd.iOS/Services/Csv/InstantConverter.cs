using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using NodaTime;

namespace HealthNerd.iOS.Services.Csv
{
    public class InstantConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException("Well, we're only ever supposed to be going outbound here... so no need to implement this. 🐸");
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((Instant) value).ToString();
        }
    }
}