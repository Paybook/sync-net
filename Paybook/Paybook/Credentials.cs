using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaybookSDK
{
    public class Credentials : Paybook
    {
        public string id_credential { get; set; }
        public string id_site { get; set; }
        public string username { get; set; }
        public string id_site_organization { get; set; }
        public string id_site_organization_type { get; set; }
        public string ws { get; set; }
        public string status { get; set; }
        public string twofa { get; set; }
        public string twofa_config { get; set; }
        public Credentials init(Session session = null, string id_user = "", string id_site = "", JObject credentials = null, JObject credentials_json = null)
        {
            try
            {
                this.id_site = "";
                this.twofa_config = "";
                if (credentials_json == null)
                {
                    JObject data;
                    if (!string.IsNullOrEmpty(id_user))
                    {
                        data = new JObject(new JProperty("api_key", api_key), new JProperty("id_user", id_user));
                    }
                    else
                    {
                        data = new JObject(new JProperty("token", session.token));
                    }
                    data.Add(new JProperty("id_site", id_site));
                    data.Add(new JProperty("credentials", credentials));
                    Console.WriteLine(data.ToString());
                    credentials_json = call("credentials", method.post, data);
                }
                if (!string.IsNullOrEmpty(id_site))
                {
                    this.id_site = id_site;
                }
                else if (credentials_json.GetValue("id_site") != null)
                {
                    this.id_site = credentials_json["id_site"].ToString();
                }
                Console.WriteLine(credentials_json.ToString());
                Credentials credential = JsonConvert.DeserializeObject<Credentials>(credentials_json["response"].ToString());
                this.id_credential = credential.id_credential;
                this.username = credential.username;
                this.id_site_organization = credential.id_site_organization;
                this.id_site_organization_type = credential.id_site_organization_type;
                this.ws = credential.ws;
                this.status = credential.status;
                this.twofa = credential.twofa;
                return this;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public bool delete(string id_credential, Session session = null, string id_user = "")
        {
            try
            {
                string parameters;
                if (!string.IsNullOrEmpty(id_user))
                {
                    parameters = "?api_key=" + api_key + "&id_user=" + id_user;
                }
                else
                {
                    parameters = "?token=" + session.token;
                }
                call("credentials/" + id_credential + parameters, method.delete);
                return true;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public List<Credentials> get(Session session = null, string id_user = "")
        {
            try
            {
                string parameters;
                if (!string.IsNullOrEmpty(id_user))
                {
                    parameters = "?api_key=" + api_key + "&id_user=" + id_user;
                }
                else
                {
                    parameters = "?token=" + session.token;
                }

                JObject response = call("credentials" + parameters, method.get);

                return JsonConvert.DeserializeObject<List<Credentials>>(response["response"].ToString());
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public JArray get_status(Session session = null, string id_user = "")
        {
            try
            {
                string parameters;
                if (!string.IsNullOrEmpty(id_user))
                {
                    parameters = "?api_key=" + api_key + "&id_user=" + id_user;
                }
                else
                {
                    parameters = "?token=" + session.token;
                }
                if (!string.IsNullOrEmpty(this.id_site))
                {
                    parameters = parameters + "&id_site=" + id_site;
                }
                JObject response = call(this.status + parameters, method.get);
                JArray status = (JArray)response["response"];

                foreach (var item in status.Children())
                {
                    var itemProperties = item.Children<JProperty>();
                    var status_item = itemProperties.FirstOrDefault(x => x.Name == "code" && x.Value.ToString() == "410");
                    if (status_item != null)
                    {
                        this.twofa = item["address"].ToString();
                        this.twofa_config = item["twofa"][0].ToString();
                        Console.WriteLine(twofa);
                        Console.WriteLine(twofa_config);
                        break;
                    }
                }
                return status;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public bool set_twofa(Session session, string id_user = "", string twofa_value = "")
        {
            try
            {
                JObject data;
                if (!string.IsNullOrEmpty(id_user))
                {
                    data = new JObject(new JProperty("api_key", api_key), new JProperty("id_user", id_user));
                }
                else
                {
                    data = new JObject(new JProperty("token", session.token));
                }
                if (!string.IsNullOrEmpty(this.id_site))
                {
                    data.Add(new JProperty("id_site", this.id_site));
                    data.Add(new JProperty("twofa", (new JObject(new JProperty(JObject.Parse(this.twofa_config)["name"].ToString(), twofa_value)))));
                    call(this.twofa, method.post, data);
                    return true;
                }
                return false;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }
    }
}
