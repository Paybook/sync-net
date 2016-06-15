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
    public class Paybook
    {
        string paybook_link;
        string api_key;

        /// <summary>
        /// Paybook constructor
        /// reads by default from webconfig file
        /// </summary>
        public Paybook()
        {
            paybook_link = ConfigurationManager.AppSettings["PAYBOOK_LINK"];
            api_key = ConfigurationManager.AppSettings["API_KEY"];

            if (string.IsNullOrEmpty(paybook_link))
                throw new Exception("paybook_link can not be empty, please configure it in your webconfig file");

            if (string.IsNullOrEmpty(api_key))
                throw new Exception("api_key can not be empty, please configure it in your webconfig file");
        }

        /// <summary>
        /// Register a new user to Paybook
        /// </summary>
        /// <param name="username">string</param>
        /// <returns>string</returns>
        public string signup(string username)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Creating request parameters
                    JObject data = new JObject(new JProperty("api_key", api_key), new JProperty("name", username));
                    // http Post
                    HttpResponseMessage response = client.PostAsync("users", new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        JObject user = JObject.Parse(respon);
                        return user["response"]["id_user"].ToString();
                    }
                    else
                        throw new PBException(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (PBException pbEx)
            {
                throw pbEx;
            }
            catch (Exception ex)
            {
                throw new PBException("500", false, ex.Message, null);
            }
        }

        /// <summary>
        /// Login an exisiting user
        /// </summary>
        /// <param name="id_user">string</param>
        /// <returns>string</returns>
        public string login(string id_user)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Creating request parameters
                    JObject data = new JObject(new JProperty("api_key", api_key), new JProperty("id_user", id_user));

                    HttpResponseMessage response = client.PostAsync("sessions", new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        JObject user = JObject.Parse(respon);
                        return user["response"]["token"].ToString();
                    }
                    else
                        throw new PBException(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (PBException pbEx)
            {
                throw pbEx;
            }
            catch (Exception ex)
            {
                throw new PBException("500", false, ex.Message, null);
            }
        }

        /// <summary>
        ///  Retrieve the set of institutions available as json object
        /// </summary>
        /// <param name="token">string</param>
        /// <returns>string</returns>
        public string catalogs(string token, bool is_test = false)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // HTTP GET Using QueryStrings
                    string parameters = "catalogues/sites?token=" + token + (is_test ? "&is_test=1" : string.Empty);

                    HttpResponseMessage response = client.GetAsync(parameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        JObject catalogs = JObject.Parse(respon);
                        return catalogs["response"].ToString();
                    }
                    else
                        throw new PBException(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (PBException pbEx)
            {
                throw pbEx;
            }
            catch (Exception ex)
            {
                throw new PBException("500", false, ex.Message, null);
            }
        }

        /// <summary>
        /// Register credentials for a specific institution
        /// </summary>
        /// <param name="newcredentials">JObject</param>
        /// <returns>JObject</returns>
        public JObject credentials(JObject newcredentials)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.PostAsync("credentials", new StringContent(newcredentials.ToString(), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        JObject user = JObject.Parse(respon);
                        return (JObject)user["response"];
                    }
                    else
                        throw new PBException(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (PBException pbEx)
            {
                throw pbEx;
            }
            catch (Exception ex)
            {
                throw new PBException("500", false, ex.Message, null);
            }
        }

        /// <summary>
        /// Get the sync status of a specific institution as json object
        /// </summary>
        /// <param name="token">string</param>
        /// <param name="id_site">string</param>
        /// <param name="url_status">string</param>
        /// <returns>string</returns>
        public string status(string token, string id_site, string url_status)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string parameters = url_status + "?token=" + token + "&id_site=" + id_site;

                    HttpResponseMessage response = client.GetAsync(parameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        JObject credentials = JObject.Parse(respon);
                        return credentials["response"].ToString();
                    }
                    else
                        throw new PBException(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (PBException pbEx)
            {
                throw pbEx;
            }
            catch (Exception ex)
            {
                throw new PBException("500", false, ex.Message, null);
            }
        }

        /// <summary>
        /// Send token for a credential
        /// </summary>
        /// <param name="twofa">JObject</param>
        /// <param name="address">string</param>
        /// <returns>string</returns>
        public JObject twofa(JObject twofa, string address)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.PostAsync(address, new StringContent(twofa.ToString(), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        JObject token = JObject.Parse(respon);
                        return token;
                    }
                    else
                        throw new PBException(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (PBException pbEx)
            {
                throw pbEx;
            }
            catch (Exception ex)
            {
                throw new PBException("500", false, ex.Message, null);
            }
        }

        /// <summary>
        /// Get the accounts registered in a specific institution as json object
        /// </summary>
        /// <param name="token">string</param>
        /// <param name="id_site">string</param>
        /// <returns>string</returns>
        public string accounts(string token, string id_site)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string parameters = "accounts?token=" + token + "&id_site=" + id_site;

                    HttpResponseMessage response = client.GetAsync(parameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        JObject credentials = JObject.Parse(respon);
                        return credentials["response"].ToString();
                    }
                    else
                        throw new PBException(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (PBException pbEx)
            {
                throw pbEx;
            }
            catch (Exception ex)
            {
                throw new PBException("500", false, ex.Message, null);
            }
        }

        /// <summary>
        /// Get the accounts registered in a specific account as json object
        /// </summary>
        /// <param name="token">string</param>
        /// <param name="id_account">string</param>
        /// <returns>string</returns>
        public string transactions(string token, string id_account)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(paybook_link);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string parameters = "transactions?token=" + token + "&id_account=" + id_account;

                    HttpResponseMessage response = client.GetAsync(parameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        JObject credentials = JObject.Parse(respon);
                        return credentials["response"].ToString();
                    }
                    else
                        throw new PBException(response.StatusCode.ToString(), response.IsSuccessStatusCode, response.ReasonPhrase, response);
                }
            }
            catch (PBException pbEx)
            {
                throw pbEx;
            }
            catch (Exception ex)
            {
                throw new PBException("500", false, ex.Message, null);
            }
        }
    }
}