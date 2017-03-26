using System;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Penny.Extensions;

namespace TerrariaServerList
{

  #region Json Converters

  public class WeirdDateTimeConverter : DateTimeConverterBase
  {
    public override bool CanWrite => false;
    public override bool CanConvert(Type objectType) => objectType == typeof(DateTime);

    private static string Cleaner(string input)
      => new StringBuilder(input)
        .Replace("EST", "")
        .Replace(",", "")
        .Replace("st", "")
        .Replace("nd", "")
        .Replace("rd", "")
        .Replace("th", "")
        .ToString();

    public override object ReadJson
      (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      => DateTime.Parse(Cleaner(reader.Value.ToString())).AddHours(5); // EST -> GMT

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }

  public class EpochDateTimeConverter : DateTimeConverterBase
  {
    public override object ReadJson
      (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      => Convert.ToDouble(reader.Value).FromEpochSeconds();

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      => writer.WriteRawValue(((DateTime) value).ToEpochSeconds().ToString(CultureInfo.InvariantCulture));
  }

  public class MonthYearDateTimeConverter : IsoDateTimeConverter
  {
    public MonthYearDateTimeConverter()
    {
      DateTimeFormat = "yyyyMM";
      Culture = CultureInfo.InvariantCulture;
    }
  }

  public class TruthyJsonConverter : JsonConverter
  {
    internal static bool ParseTruthy(string input)
    {
      switch (input.ToLower().Trim())
      {
        case "true":
        case "yes":
        case "y":
        case "1":
          return true;

        case "false":
        case "no":
        case "n":
        case "0":
          return false;
        default:
          throw new ArgumentException("Provided argument isn't a supported truthy.", nameof(input));
      }
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(bool);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
      JsonSerializer serializer) => ParseTruthy(reader.Value.ToString());

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      => writer.WriteRawValue(((bool) value).ToString());
  }

  #endregion
}