using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppDash.Plugins
{
    /// <summary>
    /// A custom JSON converter to convert a PluginData object with its data to the right types.
    /// </summary>
    public class PluginDataConverter : JsonConverter<PluginData>
    {
        public override PluginData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            PluginData pluginData = new PluginData();

            reader.Read();
            reader.Read();

            pluginData.Data = reader.TokenType == JsonTokenType.Null ? null : new Dictionary<string, Tuple<Type, object>>();

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                string key = reader.GetString();

                reader.Read();
                reader.Read();
                reader.Read();

                string ty = reader.GetString();
                Type type = Type.GetType(ty);

                reader.Read();
                reader.Read();

                pluginData.Data?.Add(key, new Tuple<Type, object>(type, JsonSerializer.Deserialize(ref reader, type)));

                reader.Read();
                reader.Read();
            }

            reader.Read();
            reader.Read();

            return pluginData;
        }

        public override void Write(Utf8JsonWriter writer, PluginData value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if(value.Data == null)
                writer.WriteNull("Data");
            else
            {
                writer.WriteStartObject("Data");

                foreach (KeyValuePair<string, Tuple<Type, object>> keyValuePair in value.Data)
                {
                    writer.WriteStartObject(keyValuePair.Key);

                    writer.WriteString("Item1", keyValuePair.Value.Item1.FullName);

                    writer.WritePropertyName("Item2");
                    JsonSerializer.Serialize(writer, keyValuePair.Value.Item2);

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}