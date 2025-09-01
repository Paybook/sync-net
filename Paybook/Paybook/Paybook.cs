using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PaybookSDK
{
    public abstract class Paybook
    {
        const string paybook_link = "https://opendata-api.syncfy.com/v1/";

        public static string api_key { get; set; }
        public enum method { post, get, delete }

        /// <summary>
        /// Paybook constructor
        /// </summary>
        public Paybook()
        {
        }

        public static void init(string api_key)
        {
            Paybook.api_key = api_key;
        }

        public JObject call(string endpoint, method method, JObject data = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //executing
                    HttpResponseMessage response;
                    if (method == method.post)
                    {
                        response = client.PostAsync(endpoint, new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;
                    }
                    else if(method == method.get)
                    {
                        response = client.GetAsync(endpoint).Result;
                    }
                    else
                    {
                        response = client.DeleteAsync(endpoint).Result;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string respon = response.Content.ReadAsStringAsync().Result;
                            return JObject.Parse(respon);
                        }
                        catch (Exception)
                        {
                            JObject file = new JObject(new JProperty("content",response.Content.ReadAsStringAsync().Result));
                            return file;
                        }
                    }
                    else
                        throw new Error(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (Error ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Error("500", false, ex.Message, null);
            }
        }
    }
}
