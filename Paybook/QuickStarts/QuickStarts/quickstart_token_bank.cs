using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaybookSDK;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace QuickStarts
{
    public class quickstart_token_bank
    {
        const string YOUR_API_KEY = "YOUR_API_KEY";
        const string BANK_USERNAME = "YOUR_BANK_USERNAME";
        const string BANK_PASSWORD = "YOUR_BANK_PASSWORD";

        public quickstart_token_bank() { }

        private User user;
        private User currentUser;
        private Session session;
        private Session currentSession;
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
                currentUser = user_list[0];
                Console.WriteLine(currentUser.name + " " + currentUser.id_user);

                session = new Session();
                currentSession = session.init(currentUser);
                Console.WriteLine(currentSession.token);

                Site bank_site = new Site();
                catalogues = new Catalogues();
                List<Site> sites = catalogues.get_sites(currentSession);
                sites.ForEach(s =>
                {
                    Console.WriteLine(s.name);
                    if (s.name == "Banorte en su empresa")
                        bank_site = s;
                });
                Console.WriteLine("Bank site: " + bank_site.name + " " + bank_site.id_site);

                credentials = new Credentials();
                JObject credentials_data = new JObject(new JProperty("username", BANK_USERNAME), new JProperty("password", BANK_PASSWORD));
                Credentials bank_credentials = credentials.init(currentSession, id_site: bank_site.id_site, credentials: credentials_data);
                Console.WriteLine(bank_credentials.id_credential + " " + bank_credentials.username);
                JArray sync_status = bank_credentials.get_status(currentSession);

                printInstance(sync_status);
                Console.WriteLine("Esperando por token ... ");

                string status_410 = string.Empty;
                while (string.IsNullOrEmpty(status_410))
                {
                    Console.WriteLine(". . .");
                    System.Threading.Thread.Sleep(3);
                    sync_status = bank_credentials.get_status(currentSession);
                    printInstance(sync_status);
                    foreach (var item in sync_status.Children())
                    {
                        var itemProperties = item.Children<JProperty>();
                        var status_item = itemProperties.FirstOrDefault(x => x.Name == "code" && x.Value.ToString() == "410");
                        if (status_item != null)
                        {
                            status_410 = status_item.Value.ToString();
                            break;
                        }
                    }
                }

                Console.WriteLine("Ingresa el código de seguridad: ");
                string twofa_value = Console.ReadLine();

                bool twofa = bank_credentials.set_twofa(currentSession, twofa_value: twofa_value);

                Console.WriteLine("Twofa: " + twofa.ToString());
                Console.WriteLine("Esperando validacion de token ...");

                string status_102_or_401 = string.Empty;
                while (string.IsNullOrEmpty(status_102_or_401))
                {
                    Console.WriteLine(". . .");
                    Thread.Sleep(5);
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
                            Console.WriteLine("Error en credenciales, press any key to exit...");
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
                    Thread.Sleep(3);
                    sync_status = bank_credentials.get_status(currentSession);
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

                transaction = new Transaction();
                List<Transaction> transactions = transaction.get(currentSession);

                int i = 0;
                foreach (var transaction_item in transactions)
                {
                    i++;
                    Console.WriteLine(i + ". " + transaction_item.description + " $" + transaction_item.amount);
                }

                Console.WriteLine("Quickstart bank script executed successfully");
                return true;
            }
            catch (Error ex)
            {
                Console.WriteLine("An error has occurred in quickstart_token_bank: " + ex.message);
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
