using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaybookSDK
{
    public class Transaction : Paybook
    {
        public string id_transaction { get; set; }
        public string id_user { get; set; }
        public string id_external { get; set; }
        public string id_site { get; set; }
        public string id_site_organization { get; set; }
        public string id_site_organization_type { get; set; }
        public string id_account { get; set; }
        public string id_account_type { get; set; }
        public int is_disable { get; set; }
        public string description { get; set; }
        public string amount { get; set; }
        public string dt_transaction { get; set; }
        public string dt_refresh { get; set; }

        public void init(JObject transaction_json)
        {
            try
            {
                this.id_transaction = transaction_json["id_transaction"].ToString();
                this.id_user = transaction_json["id_user"].ToString();
                this.id_external = transaction_json["id_external"].ToString();
                this.id_site = transaction_json["id_site"].ToString();
                this.id_site_organization = transaction_json["id_site_organization"].ToString();
                this.id_site_organization_type = transaction_json["id_site_organization_type"].ToString();
                this.id_account = transaction_json["id_account"].ToString();
                this.id_account_type = transaction_json["id_account_type"].ToString();
                this.is_disable = int.Parse(transaction_json["is_disable"].ToString());
                this.description = transaction_json["description"].ToString();
                this.amount = transaction_json["amount"].ToString();
                this.dt_transaction = transaction_json["dt_transaction"].ToString();
                this.dt_refresh = transaction_json["dt_refresh"].ToString();
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public List<Transaction> get(Session session, string id_user = "", JObject options = null)
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

                List<Transaction> transaction_jsons;

                JObject response = call("transactions" + parameters, method.get);
                Console.WriteLine(response["response"].ToString());
                transaction_jsons = JsonConvert.DeserializeObject<List<Transaction>>(response["response"].ToString());

                return transaction_jsons;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public int get_count(Session session, string id_user = "", JObject options = null)
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

                JObject response = call("transactions/count" + parameters, method.get);
                Console.WriteLine(response["response"].ToString());
                return int.Parse(response["response"]["count"].ToString());
            }
            catch (Error ex)
            {
                throw ex;
            }
        }
    }
}
