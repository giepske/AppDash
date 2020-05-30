using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AppDash.Plugins
{
    /// <summary>
    /// A custom JSON converter to convert a PluginData object with its data to the right types.
    /// </summary>
    public class NewtonPluginDataConverter : JsonConverter<PluginData>
    {
        public override void WriteJson(JsonWriter writer, PluginData value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value.Data == null)
            {
                writer.WritePropertyName("Data");
                writer.WriteNull();
            }

            else
            {
                writer.WritePropertyName("Data");
                writer.WriteStartObject();

                foreach (KeyValuePair<string, Tuple<Type, object>> keyValuePair in value.Data)
                {
                    writer.WritePropertyName(keyValuePair.Key);
                    writer.WriteStartObject();

                    writer.WritePropertyName("Item1");
                    writer.WriteValue(keyValuePair.Value.Item1.FullName);

                    writer.WritePropertyName("Item2");
                    serializer.Serialize(writer, keyValuePair.Value.Item2);

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        public override PluginData ReadJson(JsonReader reader, Type objectType, PluginData existingValue, bool hasExistingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            PluginData pluginData = new PluginData();

            reader.Read();
            reader.Read();

            pluginData.Data = reader.TokenType == JsonToken.Null ? null : new Dictionary<string, Tuple<Type, object>>();

            reader.Read();

            while (reader.TokenType != JsonToken.EndObject)
            {
                
                string key = reader.Value as string;

                reader.Read();
                reader.Read();
                reader.Read();

                string ty = reader.Value as string;
                Type type = Type.GetType(ty);

                if (type == null)
                {
                    type = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => !a.IsDynamic && a.GetType(ty) != null)
                        ?.GetType(ty);

                    if (type == null)
                        throw new Exception("Unable to find type while trying to convert JSON to PluginData object: " + ty);
                }

                reader.Read();
                reader.Read();

                pluginData.Data?.Add(key, new Tuple<Type, object>(type, serializer.Deserialize(reader, type)));

                reader.Read();
                reader.Read();
            }

            reader.Read();
            reader.Read();

            return pluginData;
        }
    }

    /// <summary>
    /// A custom JSON converter to convert a PluginData object with its data to the right types.
    /// </summary>
    public class PluginDataConverter : System.Text.Json.Serialization.JsonConverter<PluginData>
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

                if (type == null)
                {
                    type = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => !a.IsDynamic && a.GetType(ty) != null)
                        ?.GetType(ty);

                    if(type == null)
                        throw new Exception("Unable to find type while trying to convert JSON to PluginData object: " + ty);
                }

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