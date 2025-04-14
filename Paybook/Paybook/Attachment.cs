using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaybookSDK
{
    public class Attachment : Paybook
    {
        public string id_account { get; set; }
        public string id_user { get; set; }
        public string id_external { get; set; }
        public string id_attachment_type { get; set; }
        public string id_transaction { get; set; }
        public string file { get; set; }
        public string extra { get; set; }
        public string url { get; set; }
        public string content { get; set; }
        public string dt_refresh { get; set; }
        public void init(JObject attachment_json)
        {
            try
            {
                this.id_account = attachment_json["id_account"].ToString();
                this.id_user = attachment_json["id_user"].ToString();
                this.id_external = attachment_json["id_external"].ToString();
                this.id_attachment_type = attachment_json["id_attachment_type"].ToString();
                this.id_transaction = attachment_json["id_transaction"].ToString();
                this.file = attachment_json["file"].ToString();
                this.extra = attachment_json["extra"].ToString(); //if "extra" in attachment_json else None
                this.url = attachment_json["url"].ToString();
                this.dt_refresh = attachment_json["dt_refresh"].ToString();
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public List<Attachment> get(Session session, string id_user = "", string id_attachment = "")
        {
            try
            {
                string endpoint;
                string parameters;
                List<Attachment> attachments = new List<Attachment>();
                if (!string.IsNullOrEmpty(id_user))
                {
                    parameters = "?api_key=" + api_key + "&id_user=" + id_user;
                }
                else
                {
                    parameters = "?token=" + session.token;
                }
                if (!string.IsNullOrEmpty(id_attachment))
                {
                    endpoint = id_attachment + parameters;
                }
                else
                {
                    endpoint = "attachments" + parameters;
                }

                Console.WriteLine(endpoint);
                JObject response = call(endpoint, method.get);
                if (response.GetValue("response") != null)
                {
                    return JsonConvert.DeserializeObject<List<Attachment>>(response["response"].ToString());
                }
                else
                {
                    attachments.Add(new Attachment() { content = response["content"].ToString() });
                    return attachments;
                }
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public JObject get_extra(Session session, string id_user = "", string id_attachment = "")
        {
            try
            {
                string endpoint;
                string parameters;
                List<Attachment> attachments = new List<Attachment>();
                if (!string.IsNullOrEmpty(id_user))
                {
                    parameters = "?api_key=" + api_key + "&id_user=" + id_user;
                }
                else
                {
                    parameters = "?token=" + session.token;
                }
                if (!string.IsNullOrEmpty(id_attachment))
                {
                    endpoint = id_attachment + "/extra" + parameters;
                }
                else
                {
                    endpoint = "attachments" + parameters;
                }
                Console.WriteLine(endpoint);
                JObject response = call(endpoint, method.get);
                return response;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public int get_count(Session session, string id_user = "")
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
                JObject response = call("attachments/count" + parameters, method.get);
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
