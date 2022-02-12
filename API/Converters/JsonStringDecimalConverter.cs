using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Converters
{
    public class StringDecimalJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Decimal values must be specified as strings.");
            }

            return JsonSerializer.Deserialize<decimal>(reader.GetString(), options);
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value.ToString(), options);
    }
}
