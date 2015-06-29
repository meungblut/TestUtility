namespace MEungblut.TestUtility
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class CamelCaseJsonConverter
    {
        private readonly JsonSerializerSettings settings;

        public CamelCaseJsonConverter()
        {
            this.settings = new JsonSerializerSettings
            { 
                Converters = new JsonConverter[]
                {
                    new StringEnumConverter(),
                },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public string Serialize(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, this.settings);
        }

        public TTypeToDeserialize Deserialize<TTypeToDeserialize>(string jsonToDeserialize)
        {
            return JsonConvert.DeserializeObject<TTypeToDeserialize>(jsonToDeserialize, this.settings);
        }

        public object Deserialize(string jsonToDerserialize, Type typeToDeserializeTo)
        {
            return JsonConvert.DeserializeObject(jsonToDerserialize, typeToDeserializeTo, this.settings);
        }
    }
}