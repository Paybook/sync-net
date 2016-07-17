using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaybookSDK
{
    public class User : Paybook
    {
        public User() : base() { }
        public string id_user { get; set; }
        public string id_external { get; set; }
        public string name { get; set; }
        public string dt_create { get; set; }
        public string dt_modify { get; set; }

        public User init(string name, string id_user = "")
        {
            try
            {
                User user = null;

                if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(id_user))
                {
                    JObject data = new JObject(new JProperty("api_key", api_key), new JProperty("name", name));
                    JObject response = call("users", method.post, data);

                    user = JsonConvert.DeserializeObject<User>(response["response"].ToString());
                }
                else if (!string.IsNullOrEmpty(id_user))
                {
                    user = this.get().Where(u => u.id_user == id_user).FirstOrDefault<User>();
                }
                return user;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public List<User> get(Dictionary<string, string> d = null)
        {
            try
            {
                List<User> users = new List<User>();
                
               JObject response = call("users?api_key="+ api_key, method.get);

               users = JsonConvert.DeserializeObject<List<User>>(response["response"].ToString());
                
                return users;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public bool delete(string id_user)
        {
            try
            {
                JObject response = call(@"users/" + id_user + "?api_key="+ api_key, method.delete);
                return (bool)response["status"];
            }
            catch (Error ex)
            {
                throw ex;
            }
        }
    }
}
