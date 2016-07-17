using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaybookSDK;
using Newtonsoft.Json.Linq;

namespace QuickStarts
{
    public class quickstart_sat
    {
        const string YOUR_API_KEY = "YOUR_API_KEY";
        const string RFC = "YOUR_RFC";
        const string CIEC = "YOUR_CIEC";

        public quickstart_sat() { }

        private User user;
        private User new_user;
        private Session session;
        private Catalogues catalogues;
        private Credentials credentials;
        private Transaction transaction;
        private Attachment attachment;

        private void Initialize()
        {
        }
        public void start()
        {
            try
            {
                Initialize();
                Paybook.init(YOUR_API_KEY);

                user = new User();
                new_user = user.init("MY_USER");

                List<User> my_users = user.get();

                foreach (var user in my_users)
                {
                    Console.WriteLine(user.name);
                }

                session = new Session();
                Session currentSession = session.init(new_user);
                Console.WriteLine("Session token: " + currentSession.token);

                bool session_verified = currentSession.verify();
                Console.WriteLine("Session verified: " + session_verified);

                catalogues = new Catalogues();
                Site sat_site = new Site();
                List<Site> sites = catalogues.get_sites(currentSession, is_test: true);

                sites.ForEach(s =>
                {
                    Console.WriteLine(s.name);
                    if (s.name == "CIEC")
                        sat_site = s;
                });
                Console.WriteLine("SAT site: " + sat_site.id_site + " " + sat_site.name);

                credentials = new Credentials();
                JObject credentials_data = new JObject(new JProperty("rfc", RFC), new JProperty("password", CIEC));
                Credentials sat_credentials = credentials.init(currentSession, id_site: sat_site.id_site, credentials: credentials_data);
                Console.WriteLine(sat_credentials.username);

                bool sat_sync_completed = false;
                JArray sat_status;
                while (!sat_sync_completed)
                {
                    Console.WriteLine("Polling ...");
                    System.Threading.Thread.Sleep(5);
                    sat_status = sat_credentials.get_status(currentSession);
                    foreach (var item in sat_status.Children())
                    {
                        var itemProperties = item.Children<JProperty>();
                        var status_item = itemProperties.FirstOrDefault(x => x.Name == "code" && x.Value.ToString() == "200");
                        if (status_item != null)
                        {
                            sat_sync_completed = true;
                            break;
                        }
                    }
                }

                transaction = new Transaction();
                List<Transaction> sat_transactions = transaction.get(currentSession);
                Console.WriteLine("Facturas del SAT: " + sat_transactions.Count);

                attachment = new Attachment();
                List<Attachment> attachments = attachment.get(currentSession);
                Console.WriteLine("Archivos XML/PDF del SAT: " + attachments.Count);

                int i = 0;
                string id_attachment;
                if (attachments.Count > 0)
                {
                    i = 0;
                    foreach (var attachment_item in attachments)
                    {
                        i++;
                        id_attachment = attachment_item.url.Substring(1,attachment_item.url.Length -1);
                        Console.WriteLine(id_attachment);
                        List<Attachment> attachment_content = attachment.get(currentSession, id_attachment: id_attachment);
                        Console.WriteLine("Attachment " + i + ":");
                        if(attachment_content.Count > 0)
                            Console.WriteLine(attachment_content[0]);
                        if (i == 2)
                            break;
                    }
                }
                Console.WriteLine("Quickstart script executed successfully");
            }
            catch (Error ex)
            {
                Console.WriteLine("An error has occurred in quickstart_sat: " + ex.message);
            }
        }

        private string StringToUTF8(string cadena)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(cadena);
            return encodedBytes.ToString();
        }
    }
}
