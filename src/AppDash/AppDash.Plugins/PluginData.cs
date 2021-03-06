﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AppDash.Plugins
{
    [JsonConverter(typeof(PluginDataConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(NewtonPluginDataConverter))]
    public class PluginData
    {
        public Dictionary<string, Tuple<Type, object>> Data { get; set; }

        public PluginData(params KeyValuePair<string,  object>[] data)
        {
            Data = data.ToDictionary(pair => pair.Key, pair => new Tuple<Type, object>(pair.Value.GetType(), pair.Value));
        }

        public PluginData()
        {
            Data = new Dictionary<string, Tuple<Type, object>>();
        }

        public bool RemoveKey(string key)
        {
            return Data.Remove(key);
        }

        public void SetData(string key, object data)
        {
            Data[key] = new Tuple<Type, object>(data.GetType(), data);
        }

        public T GetData<T>(string key)
        {
            if (!Data.ContainsKey(key))
                return default;

            return (T)Data[key].Item2;
        }
    }
}
