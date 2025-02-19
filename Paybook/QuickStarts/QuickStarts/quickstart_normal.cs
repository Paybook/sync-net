using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaybookSDK;
using Newtonsoft.Json.Linq;

namespace QuickStarts
{
    public class quickstart_normal
    {
        const string YOUR_API_KEY = "YOUR_API_KEY";
        const string BANK_USERNAME = "test";
        const string BANK_PASSWORD = "test";

        public quickstart_normal() { }

        private User user;
        private Session session;
        private Catalogues catalogues;
        private Credentials credentials;
        private Transaction transaction;

        private void Initialize()
        {
            
        }
        public bool start()
        {
            try
            {
                Initialize();
                Paybook.init(YOUR_API_KEY);

                user = new User();
                List<User> user_list = user.get();
                User currentUser = user_list[0];
                Console.WriteLine("name: " + currentUser.name + " id_user: " + currentUser.id_user);

                session = new Session();
                Session currentSession = session.init(currentUser);
                Console.WriteLine("token: " + currentSession.token);

                Site bank_site= new Site();
                catalogues = new Catalogues();
                List<Site> sites = catalogues.get_sites(currentSession);
                sites.ForEach(s =>
                {
                    Console.WriteLine("Bank site: " + s.name + " id_site: " + s.id_site);
                    if (s.name == "SuperNET Particulares")
                    {
                        bank_site = s;
                    }
                });
                
                credentials = new Credentials();
                JObject new_credentials = new JObject(new JProperty("username", BANK_USERNAME), new JProperty("password", BANK_PASSWORD));
                Credentials bank_credentials = credentials.init(currentSession, "", bank_site.id_site, new_credentials);

                Console.WriteLine("Esperando validacion de credenciales ...");
                string status_102_or_401 = string.Empty;
                JArray sync_status;
                while (string.IsNullOrEmpty(status_102_or_401))
                {
                    Console.WriteLine(". . .");
                    System.Threading.Thread.Sleep(3);
                    sync_status = bank_credentials.get_status(currentSession);
                    printInstance(sync_status);
                    foreach (var item in sync_status.Children())
                    {
                        var itemProperties = item.Children<JProperty>();
                        var status_item = itemProperties.FirstOrDefault(x => x.Name == "code" && (x.Value.ToString() == "102" || x.Value.ToString() == "401"));
                        if (status_item != null)
                        {
                            status_102_or_401 = status_item.Value.ToString();
                            break;
                        }
                        var status_item_error = itemProperties.FirstOrDefault(x => x.Name == "code" && x.Value.ToString() == "401");
                        if (status_item_error != null)
                        {
                            status_102_or_401 = "Error en credenciales";
                            Console.WriteLine("Error en credenciales, press any key to exit program...");
                            break;
                        }
                    }
                }

                if (status_102_or_401 == "Error en credenciales")
                {
                    return false;
                }

                Console.WriteLine("Esperando sincronizacion ...");
                string status_200 = string.Empty;
                while (string.IsNullOrEmpty(status_200))
                {
                    Console.WriteLine(". . .");
                    System.Threading.Thread.Sleep(3);
                    sync_status = bank_credentials.get_status(currentSession);
                    printInstance(sync_status);
                    foreach (var item in sync_status.Children())
                    {
                        var itemProperties = item.Children<JProperty>();
                        var status_item = itemProperties.FirstOrDefault(x => x.Name == "code" && x.Value.ToString() == "200");
                        if (status_item != null)
                        {
                            status_200 = status_item.Value.ToString();
                            break;
                        }
                    }
                }

                Console.WriteLine("Getting transactions ...");
                transaction = new Transaction();
                List<Transaction> transactions = transaction.get(currentSession);
                Console.WriteLine("Transactions: " + transactions.Count);
                int i = 0;
                foreach (var item in transactions)
                {
                    i++;
                    Console.WriteLine(i + "." + item.description + " $" +item.amount);
                }

                Console.WriteLine("Quickstart bank script executed successfully");
                return true;
            }
            catch (Error ex)
            {
                Console.WriteLine("An error has occurred in quickstart_normal: " + ex.message);
                return false;
            }
        }
        private void printInstance(object obj)
        {
            string name;
            object value;
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(obj))
            {
                name = descriptor.Name;
                value = descriptor.GetValue(obj);
                Console.WriteLine("{0}={1}", name, value);
            }
        }
    }
}
