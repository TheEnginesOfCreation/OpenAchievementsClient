#define DEBUG
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    internal class OARequest {
        private string endpoint;
        private string method;
        private string gamePrivateId;
        private string? token;
        private HttpClient httpClient = new HttpClient();
        private Dictionary<string, string> arguments = new Dictionary<string, string>();
        private bool debugEnabled;

        public OARequest AddArgument(string key, string value) {
            arguments.Add(key, value);
            return this;
        }

        public OARequest AddArgument(string key, int value) {
            arguments.Add(key, value.ToString());
            return this;
        }

        public OARequest AddArgument(string key, List<string> values) {
            string argValue = "";
            foreach (string value in values) {
                if (argValue.Length > 0) {
                    argValue += ";";
                }
                argValue += value;
            }
            return AddArgument(key, argValue);
        }

        public OARequest(string endpoint, string method, bool debugEnabled, string gamePrivateId, string? token = null) {
            this.endpoint = endpoint;
            this.method = method;
            this.gamePrivateId = gamePrivateId;
            this.token = token;
            this.debugEnabled = debugEnabled;
        }

        public async Task<OAResponse> DoRequest() {
            WriteLog("\nExecuting request to: " + GetUrl());
            OAResponse oaResponse;
            try {
                HttpResponseMessage response = await httpClient.PostAsync(GetUrl(), GetFormData());
                if (response.IsSuccessStatusCode) {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    WriteLog("response to " + GetUrl() + " = " + responseBody);
                    oaResponse = new OAResponse(responseBody);
                } else {
                    oaResponse = new OAResponse(response.StatusCode);
                }
            } catch (HttpRequestException e) {
                oaResponse = new OAResponse(System.Net.HttpStatusCode.ServiceUnavailable);
            }
            
            return oaResponse;
        }

        private MultipartFormDataContent GetFormData() {
            MultipartFormDataContent formData = new MultipartFormDataContent();
            if (!string.IsNullOrEmpty(token)) {
                WriteLog("  token = " + token);
                formData.Add(new StringContent(token, Encoding.UTF8, MediaTypeNames.Text.Plain), "token");
            }
            if (!string.IsNullOrEmpty(gamePrivateId)) {
                WriteLog("  gamePrivateId = " + gamePrivateId);
                formData.Add(new StringContent(gamePrivateId, Encoding.UTF8, MediaTypeNames.Text.Plain), "gamePrivateId");
            }

            foreach (string key in arguments.Keys) {
                WriteLog("  " + key + " = " + arguments[key]);
                formData.Add(new StringContent(arguments[key], Encoding.UTF8, MediaTypeNames.Text.Plain), key);
            }
            return formData;
        }

        private string GetUrl() {
            return OAConstants.SERVICE_URL + "/" + endpoint + "/" + method;
        }

        private void WriteLog(string message) {
            if (debugEnabled) {
                Console.WriteLine(message);
                Debug.WriteLine(message);
            }
        }
    }
}
