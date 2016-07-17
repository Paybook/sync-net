using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaybookSDK
{
    public class Session : Paybook
    {
        public Session() : base() { }
        public User user { get; set; }
        public string token { get; set; }
        public string iv { get; set; }
        public string key { get; set; }
        public Session init(User user, string token = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    this.token = token;
                    this.iv = string.Empty;
                    this.key = string.Empty;
                }
                else
                {
                    JObject data = new JObject(new JProperty("api_key", api_key), new JProperty("id_user", user.id_user));
                    JObject response = call("sessions", method.post, data);
                    this.token = response["response"]["token"].ToString();
                    this.iv = response["response"]["iv"].ToString();
                    this.key = response["response"]["key"].ToString();
                }
                return this;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public bool verify()
        {
            try
            {
                JObject response = call(@"sessions/" + this.token + "/verify", method.get);
                return true;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public bool delete(string token)
        {
            try
            {
                JObject response = call(@"sessions/" + this.token + "", method.delete);
                return true;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }
    }
}
