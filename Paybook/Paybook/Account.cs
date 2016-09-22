using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaybookSDK
{
    public class Account : Paybook
    {
        public string id_account { get; set; }
        public string id_user { get; set; }
        public string id_external { get; set; }
        public string id_credential { get; set; }
        public string id_site { get; set; }
        public string id_site_organization { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string balance { get; set; }
        public string site { get; set; }
        public string dt_refresh { get; set; }
        
        public void init(JObject account_json)
        {
            try
            {
                this.id_account = account_json["id_account"].ToString();
                this.id_user = account_json["id_user"].ToString();
                this.id_external = account_json["id_external"].ToString();
                this.id_credential = account_json["id_credential"].ToString();
                this.id_site = account_json["id_site"].ToString();
                this.id_site_organization = account_json["id_site_organization"].ToString();
                this.name = account_json["name"].ToString();
                this.number = account_json["number"].ToString();
                this.balance = account_json["balance"].ToString();
                this.site = account_json["site"].ToString();
                this.dt_refresh = account_json["dt_refresh"].ToString();
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public List<Account> get(Session session, string id_user = "", JObject options = null)
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

                if (options != null)
                {
                    parameters = parameters + Converter.JsonToQueryString(options);
                }

                List<Account> accounts;

                JObject response = call("accounts" + parameters, method.get);
                
                accounts = JsonConvert.DeserializeObject<List<Account>>(response["response"].ToString());

                return accounts;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }
    }
}
