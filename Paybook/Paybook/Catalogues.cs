using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaybookSDK
{
    public class Catalogues : Paybook
    {

        public List<Account_Type> get_account_types(Session session, string id_user = "", Dictionary<string, string> d = null)
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
                List<Account_Type> account_types;

                JObject response = call("catalogues/account_types" + parameters, method.get);

                account_types = JsonConvert.DeserializeObject<List<Account_Type>>(response["response"].ToString());

                return account_types;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }


        public List<Attachment_Type> get_attachment_types(Session session, string id_user = "", Dictionary<string, string> d = null)
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
                List<Attachment_Type> attachment_types;

                JObject response = call("catalogues/attachment_types" + parameters, method.get);
                
                attachment_types = JsonConvert.DeserializeObject<List<Attachment_Type>>(response["response"].ToString());

                return attachment_types;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }


        public List<Country> get_countries(Session session, string id_user = "", Dictionary<string, string> d = null)
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
                List<Country> countries;

                JObject response = call("catalogues/countries" + parameters, method.get);

                countries = JsonConvert.DeserializeObject<List<Country>>(response["response"].ToString());

                return countries;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public List<Site> get_sites(Session session, string id_user = "", Dictionary<string, string> d = null, bool is_test = false)
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

                if (is_test)
                {
                    parameters = parameters + "&is_test=" + is_test;
                }
                List<Site> sites;

                JObject response = call("catalogues/sites" + parameters, method.get);
                sites = JsonConvert.DeserializeObject<List<Site>>(response["response"].ToString());

                return sites;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }

        public List<Site_Organization> get_site_organizations(Session session, string id_user = "", Dictionary<string, string> d = null, bool is_test = false)
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

                if (is_test)
                {
                    parameters = parameters + "&is_test=" + is_test;
                }
                List<Site_Organization> sites_organization;

                JObject response = call("catalogues/site_organizations" + parameters, method.get);

                sites_organization = JsonConvert.DeserializeObject<List<Site_Organization>>(response["response"].ToString());

                return sites_organization;
            }
            catch (Error ex)
            {
                throw ex;
            }
        }
    }
}
