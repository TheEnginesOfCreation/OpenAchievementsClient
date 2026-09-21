using System.Collections.Generic;
using System.Diagnostics;

namespace OpenAchievements.Client {
    internal class OAJson {

        private const int STATE_DEFAULT = 0;
        private const int STATE_IN_PROPERTY_KEY = 1;
        private const int STATE_AFTER_PROPERTY_KEY = 2;
        private const int STATE_IN_STRING = 3;
        private const int STATE_IN_INT = 4;
        private const int STATE_IN_BOOL = 5;
        private const int STATE_IN_ARRAY = 6;
        private const int STATE_IN_OBJECT = 7;

        private string SerializedJSON;
        private Dictionary<string, string> data = new Dictionary<string, string>();

        public OAJson(string serializedJSON) {
            SerializedJSON = serializedJSON;
            Parse();
        }

        private void Parse() {
            int nestCount = 0;
            int index = 0;
            int state = STATE_DEFAULT;
            string propertyKeyBuffer = "";
            string propertyValueBuffer = "";

            while (index < SerializedJSON.Length) {
                char chr = SerializedJSON[index];

                switch (state) {
                    case STATE_DEFAULT:
                        if (chr == '"') {
                            state = STATE_IN_PROPERTY_KEY;
                            propertyKeyBuffer = "";
                            propertyValueBuffer = "";
                        }
                        break;
                    case STATE_IN_PROPERTY_KEY:
                        if (chr == '"') {
                            state = STATE_AFTER_PROPERTY_KEY;
                        } else {
                            propertyKeyBuffer += chr;
                        }
                        break;
                    case STATE_AFTER_PROPERTY_KEY:
                        if (chr == '"') {
                            state = STATE_IN_STRING;
                        } else if (chr >= 48 && chr <= 57) {
                            state = STATE_IN_INT;
                            propertyValueBuffer += chr;
                        } else if (chr == 'f' || chr == 'F' || chr == 't' || chr == 'T') {
                            state = STATE_IN_BOOL;
                            propertyValueBuffer += chr;
                        } else if (chr == '[') {
                            state = STATE_IN_ARRAY;
                            propertyValueBuffer += chr;
                        } else if (chr == '{') {
                            state = STATE_IN_OBJECT;
                            nestCount = 1;
                            propertyValueBuffer += chr;
                        }
                        break;
                    case STATE_IN_STRING:
                        if (chr == '"') {
                            state = STATE_DEFAULT;
                            data.Add(propertyKeyBuffer, propertyValueBuffer);
                        } else {
                            propertyValueBuffer += chr;
                        }
                        break;
                    case STATE_IN_INT:
                    case STATE_IN_BOOL:
                        if (chr == ',' || chr == '}') {
                            state = STATE_DEFAULT;
                            data.Add(propertyKeyBuffer, propertyValueBuffer);
                        } else if (chr != ' ') {
                            propertyValueBuffer += chr;
                        }
                        break;
                    case STATE_IN_ARRAY:
                        propertyValueBuffer += chr;
                        if (chr == ']') {
                            state = STATE_DEFAULT;
                            data.Add(propertyKeyBuffer, propertyValueBuffer);
                        }
                        break;
                    case STATE_IN_OBJECT:
                        propertyValueBuffer += chr;
                        if (chr == '{') {
                            nestCount++;
                        }
                        if (chr == '}') {
                            nestCount--;
                        }
                        if (nestCount == 0) {
                            state = STATE_DEFAULT;
                            data.Add(propertyKeyBuffer, propertyValueBuffer);
                        }
                        break;
                }
                index++;
            }
        }

        public bool HasKey(string key) {
            return data.ContainsKey(key);
        }


        public string GetString(string key) {
            return data[key];
        }

        public int GetInt(string key) {
            string value = data[key];
            int result = 0;
            if (int.TryParse(value, out result)) {
                return result;
            }
            return 0;
        }

        public bool GetBool(string key) {
            string value = data[key];
            return value.ToLower() == "true";
        }

        public OAJson getJSONObject(string key) {
            return new OAJson(data[key]);
        }

        public List<string> GetStringArray(string key) {
            string serializedArray = data[key];
            List<string> arrayValues = GetArrayValues(serializedArray);
            return arrayValues;
        }

        public List<OAJson> GetObjectArray(string key) {
            List<string> arrayValues = GetStringArray(key);
            List<OAJson> result = new List<OAJson>();
            foreach (string value in arrayValues) {
                result.Add(new OAJson(value));
            }
            return result;
        }

        private List<string> GetArrayValues(string serializedArray) {
            List<string> result = new List<string>();
            int index = 0;
            int state = STATE_DEFAULT;
            int nestCount = 0;

            string itemBuffer = "";
            while (index < serializedArray.Length) {
                char chr = serializedArray[index];

                switch (state) {
                    case STATE_DEFAULT:
                        if (chr == '"') {
                            state = STATE_IN_STRING;
                        } else if (chr == '{') {
                            state = STATE_IN_OBJECT;
                            nestCount = 1;
                            itemBuffer += chr;
                        }
                        break;
                    case STATE_IN_STRING:
                        if (chr == '"') {
                            state = STATE_DEFAULT;
                            result.Add(itemBuffer);
                            itemBuffer = "";
                        } else {
                            itemBuffer += chr;
                        }
                        break;
                    case STATE_IN_OBJECT:
                        itemBuffer += chr;
                        if (chr == '{') {
                            nestCount++;
                        }
                        if (chr == '}') {
                            nestCount--;
                        }
                        if (nestCount == 0) {
                            state = STATE_DEFAULT;
                            result.Add(itemBuffer);
                            Debug.WriteLine("found object in array " + itemBuffer);
                            itemBuffer = "";
                        }
                        break;
                }
                index++;
            }
            return result;
        }
    }
}
