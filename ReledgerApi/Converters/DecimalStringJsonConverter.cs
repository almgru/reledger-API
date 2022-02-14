using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReledgerApi.Converters
{
    public class DecimalStringJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!decimal.TryParse(
                        reader.GetString(),
                        NumberStyles.Number ^ NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture,
                        out var value))
            {
                throw new JsonException("Decimal values must be specified as culture invariant strings.");
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
