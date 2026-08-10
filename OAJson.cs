using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    internal class OAJson {
        private JsonNode jsonNode;

        public static OAJson FromString(string serializedJson) {
            try {
                JsonObject? body = JsonSerializer.Deserialize<JsonObject>(serializedJson);
                if (body != null) {
                    return new OAJson(body);
                }
            } catch (JsonException e) {
                //Console.WriteLine("serialized json = " + serializedJson);
            }
            
            return new OAJson();
        }

        public OAJson() {
        }

        public OAJson(JsonNode jsonNode) {
            this.jsonNode = jsonNode;
        }

        public bool HasKey(string key) {
            return jsonNode != null && jsonNode[key] != null;
        }

        public OAJson? Get(string key) {
            return jsonNode != null && jsonNode[key] != null ? new OAJson(jsonNode[key]) : null;
        }

        public string GetString(string key) {
            return jsonNode != null && jsonNode[key] != null ? jsonNode[key].GetValue<string>() : "";
        }

        public int GetInt(string key) {
            return jsonNode != null && jsonNode[key] != null ? jsonNode[key].GetValue<int>() : 0;
        }

        public bool GetBool(string key) {
            return jsonNode != null && jsonNode[key] != null ? jsonNode[key].GetValue<bool>() : false;
        }

        public List<T> GetList<T>(string key) {
            if (jsonNode != null && jsonNode[key] != null) {
                IEnumerable<T> enumerable = jsonNode[key].AsArray().GetValues<T>();
                return new List<T>(enumerable);
            }
            return new List<T>();
        }

        public List<OAJson> GetJsonArray(string key) {
            List<OAJson> list = new List<OAJson>();
            if (jsonNode != null && jsonNode[key] != null) {
                foreach (JsonNode? node in jsonNode[key].AsArray()) {
                    if (node != null) {
                        list.Add(new OAJson(node));
                    }
                }
            }
            return list;
        }

        public OAJson GetJson(string key) {
            return jsonNode != null && jsonNode[key] != null ? new OAJson(jsonNode[key]) : new OAJson();
        }
    }
}
