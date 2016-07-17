using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaybookSDK;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace UnitTesting
{
    [TestClass]
    public class PaybookUnitTest
    {
        static User user;
        static Session session;
        static Catalogues catalogues;
        static List<Site> sites;
        static Site test_site;
        static Credentials credentials;
        static Site SAT_site;
        static JObject credentials_params;
        static Account accounts;
        static Transaction transaction;
        static Attachment attachment;
        const string PAYBOOK_API_KEY = "YOUR_API_KEY";
        const string PAYBOOK_WRONG_API_KEY = "Hola";
        const string USERNAME = "SOME_PAYBOOK_USERNAME";
        const string TEST_SITE_NAME = "CIEC"; //For testing with SAT
        const string CREDENTIALS = @"{ 'rfc' : 'SOME_RFC', 'password' : 'SOME_CIEC' }"; //End of CREDENTIALS 

        [ClassInitialize()]
        public static void Initialize(TestContext context)
        {
            Console.WriteLine("******* PAYBOOK .NET LIBRARY UNIT TESTING SCRIPT ***** ");
            user = new User();
            session = new Session();
            catalogues = new Catalogues();
            sites = new List<Site>();
            test_site = new Site();
            credentials = new Credentials();
            SAT_site = new Site();
            credentials_params = new JObject();
            accounts = new Account();
            transaction = new Transaction();
            attachment = new Attachment();
        }

        [TestMethod]
        public void InicializaAPIKEYIncorrecta()
        {
            Console.WriteLine("Inicializa API KEY incorrecta: "+ PAYBOOK_WRONG_API_KEY);
            Paybook.init(PAYBOOK_WRONG_API_KEY);
        }

        [TestMethod]
        [ExpectedException(typeof(Error))]
        public void HacerUnaLlamadaConAPIIncorrecta()
        {
            Console.WriteLine("Hacer una llamada incorrecta");
            user.get();
        }

        [TestMethod]
        public void InicializaAPIKEYCorrecta()
        {
            Console.WriteLine("Inicializa API KEY correcta: " + PAYBOOK_API_KEY);
            Paybook.init(PAYBOOK_API_KEY);
        }

        [TestMethod]
        public void ConsultaListaUsuarios()
        {
            Console.WriteLine("Consulta Lista de usuarios ");
            List<User> usuarios = null;
            usuarios = user.get();
            Console.WriteLine("users: " + usuarios.Count);
            Assert.AreNotEqual(null, usuarios);
        }

        [TestMethod]
        public void CrearUsuarioNuevo()
        {
            Console.WriteLine("Crear un nuevo usuario");
            user = user.init(USERNAME, "");
            Console.WriteLine("username: " + user.name + "  id_user: " + user.id_user);
        }

        [TestMethod]
        public void ConsultarListaUsuarioMasUno()
        {
            Console.WriteLine("Consultar lista de usuarios (debe haber uno más que en el paso 1): ");
            List<User> usuarios = null;
            usuarios = user.get();
            Console.WriteLine("users: " + usuarios.Count);
            Assert.AreNotEqual(null, usuarios);
        }

        [TestMethod]
        public void BorrarUsuario()
        {
            Console.WriteLine("Borrar al usuario: " + user.id_user);
            bool deleted = user.delete(user.id_user);
            
            Assert.AreEqual(true, deleted);
        }

        [TestMethod]
        public void ConsultaListaUsuariosMismosPaso1()
        {
            Console.WriteLine("Consultar lista de usuarios (debe haber los mismos que en el paso 1): ");
            List<User> usuarios = null;
            usuarios = user.get();
            Console.WriteLine("users: " + usuarios.Count);
            Assert.AreNotEqual(null, usuarios);
        }

        [TestMethod]
        public void CrearUsuarioNuevoOtraVez()
        {
            Console.WriteLine("Crear un usuario nuevo (nuevamente) y guardar su id_user:");
            user = user.init(USERNAME, "");
            Console.WriteLine("username: " + user.name + "  id_user: " + user.id_user);
        }

        [TestMethod]
        public void ConsultarUsuarioConIdUser()
        {
            Console.WriteLine("Crear un usuario a partir de id_user (consultar usuario existente): " + user.id_user);
            user = user.init("", user.id_user);
            Console.WriteLine("username: " + user.name + "  id_user: " + user.id_user);
        }

        [TestMethod]
        public void CrearSessionUsuario()
        {
            Console.WriteLine("Crear una sesión para el usuario del paso 8: " + user.id_user);
            session.init(user);
            Console.WriteLine("token: " + session.token);
        }

        [TestMethod]
        public void VerificarSession()
        {
            Console.WriteLine("Verifica la session (debe ser valida) ");
            bool session_verified = session.verify();
            Console.WriteLine("verified: " + session_verified);
        }

        [TestMethod]
        public void BorrarSession()
        {
            Console.WriteLine("Borrar la sesión");
            bool session_deleted = session.delete(session.token);
            Console.WriteLine("deleted: " + session_deleted);
        }

        [TestMethod]
        [ExpectedException(typeof(Error))]
        public void VerificarNuevamenteSession()
        {
            try
            {
                Console.WriteLine("Verifica la session (debe ser invalida puesto que ya no existe) ");
                bool session_verified = session.verify();
                Console.WriteLine("verified: " + session_verified);
            }
            catch (Error ex)
            {
                Console.WriteLine("error: " + ex.code + " " + ex.message);
                throw ex;
            }
        }

        [TestMethod]
        public void CrearNuevamenteSessionUsuario()
        {
            Console.WriteLine("Crear una sesión nuevamente para el usuario del paso 8: " + user.id_user);
            session.init(user);
            Console.WriteLine("token: " + session.token);
        }

        [TestMethod]

        public void ConsultarCatalogosTipoCuentas()
        {
            Console.WriteLine("CATALOGUES ENDPOINTS");
            Console.WriteLine("Consultar los catálogos de tipos de cuentas");
            List<Account_Type> account_types = catalogues.get_account_types(session);
            Console.WriteLine("account_types: " + account_types.Count);
        }

        [TestMethod]

        public void ConsultarCatalogosTipoCuentasPorIdUser()
        {
            Console.WriteLine("Consultar los catálogos de tipos de cuentas por id_user: " + user.id_user);
            List<Account_Type> account_types = catalogues.get_account_types(null, user.id_user);
            Console.WriteLine("account_types: " + account_types.Count);
        }

        [TestMethod]

        public void ConsultaCatalogoTipoArchivoAdjunto()
        {
            Console.WriteLine("Consultar los catálogos de tipos de archivos adjuntos: " + session.token);
            List<Attachment_Type> attachment_types = catalogues.get_attachment_types(session);
            Console.WriteLine("account_types: " + attachment_types.Count);
        }

        [TestMethod]

        public void ConsultarCatalogoTipoPaises()
        {
            Console.WriteLine("Consultar los catálogos de tipos de paises: " + session.token);
            List<Country> countries = catalogues.get_countries(session);
            Console.WriteLine("countries: " + countries.Count + " info:" + countries[1].id_country + ":" +countries[1].name + ":" + countries[1].code);
        }

        [TestMethod]
        public void ConsultarCatalogoTipoSitios()
        {
            Console.WriteLine("Consultar los catálogos de tipos de sitios: " + session.token);
            sites = catalogues.get_sites(session);
            Console.WriteLine("sites: " + sites.Count + "info:" + sites[0].id_site + ":" + sites[0].id_site_organization + ":" + sites[0].id_site_organization_type + ":" + sites[0].name + ":" + sites[0].credentials[0].label + ":" + sites[0].credentials[0].name + ":" + sites[0].credentials[0].required + ":" + sites[0].credentials[0].type + ":" + sites[0].credentials[0].username + ":" + sites[0].credentials[0].validation);            
        }

        [TestMethod]
        public void ConsultarCatalogoTipoSitiosTest()
        {
            Console.WriteLine("Consultar los catálogos de tipos de sitios (test) y guarda el de test token: " + session.token);
            List<Site> test_sites = catalogues.get_sites(session, "", null, true);
            Console.WriteLine("test_sites: " + test_sites.Count + "info:" + test_sites[0].id_site + ":" + test_sites[0].id_site_organization + ":" + test_sites[0].id_site_organization_type + ":" + test_sites[0].name + ":" + test_sites[0].credentials[0].label + ":" + test_sites[0].credentials[0].name + ":" + test_sites[0].credentials[0].required + ":" + test_sites[0].credentials[0].type + ":" + test_sites[0].credentials[0].username + ":" + test_sites[0].credentials[0].validation);
            test_sites.ForEach(s => { if (s.name == "Token") {
                    test_site = s;
                    Console.WriteLine("test id_site: " + test_site.id_site + " name: " + test_site.name);
            } });
        }

        [TestMethod]
        public void ConsultarCatalogoTipoOrganizaciones()
        {
            Console.WriteLine("Consultar los catálogos de tipos de organizaciones: " + session.token);
            List<Site_Organization> site_organizations = catalogues.get_site_organizations(session);
            printInstance(site_organizations[0]);
            Console.WriteLine("site_organizations: " + site_organizations.Count + "info:" + site_organizations[0].avatar + ":" + site_organizations[0].cover+ ":" + site_organizations[0].id_country + ":" + site_organizations[0].id_site_organization+ ":" + site_organizations[0].id_site_organization_type + ":" + site_organizations[0].name + ":" + site_organizations[0].small_cover);
        }

        [TestMethod]

        public void ObtenerSitedelSAT()
        {
            Console.WriteLine("Obtener el site del SAT");
            sites.ForEach(s => { if (s.name == TEST_SITE_NAME) {
                    SAT_site = s;
                    Console.WriteLine("SAT id_site: " + SAT_site.id_site + " name: " + SAT_site.name);
                } });
        }

        [TestMethod]
        public void ConsultarListaCredenciales()
        {
            Console.WriteLine("Consultar la lista de credenciales: " + session.token);
            List<Credentials> credentials_list = credentials.get(session);
            Console.WriteLine("credentials: " + credentials_list.Count);
        }

        [TestMethod]
        public void CrearCredencialesConfiguracionCorrecta()
        {
            Console.WriteLine("A partir del site del SAT crear unas credenciales con la configuración correcta");
            foreach (var item in SAT_site.credentials)
            {
                credentials_params.Add(item.name, item.name);
            }
            Console.WriteLine("credentials_params: " + credentials_params.ToString());
        }

        [TestMethod]
        public void CrearCredencialesSAT()
        {
            Console.WriteLine("Crear las credenciales del SAT");
            credentials = credentials.init(session, "", SAT_site.id_site, credentials_params);
            Console.WriteLine("id_credential: " + credentials.id_credential);
            Console.WriteLine("username: " + credentials.username);
            Console.WriteLine("status: " + credentials.status);
            Console.WriteLine("twofa: " + credentials.twofa);
        }

        [TestMethod]
        public void ConsultarListaCredencialesMasUna()
        {
            Console.WriteLine("Consultar la lista de credenciales (debe haber una más que en el paso 21)");
            List <Credentials> credentials_list = credentials.get(session);
            Console.WriteLine("credentials: " + credentials_list.Count);
        }

        [TestMethod]
        public void BorrarCredencialesSATCreadas()
        {
            Console.WriteLine("Borrar las credenciales del SAT creadas");
            bool credentials_deleted = credentials.delete(credentials.id_credential, session);
            Console.WriteLine("deleted: " + credentials_deleted);
        }

        [TestMethod]
        public void ConsultarListaCredencialesMismas()
        {
            Console.WriteLine("Consultar la lista de credenciales (debe haber las mismas que en paso 21)");
            List<Credentials> credentials_list = credentials.get(session);
            if(credentials_list.Count>0)
                printInstance(credentials_list[0]);
            Console.WriteLine("credentials: " + credentials_list.Count);
        }

        [TestMethod]
        public void CrearNuevamenteCredencialesSAT()
        {
            Console.WriteLine(" Crear las credenciales del SAT nuevamente");
            credentials = credentials.init(session, "", SAT_site.id_site, credentials_params);
            Console.WriteLine("id_credential: " + credentials.id_credential);
            Console.WriteLine("username: " + credentials.username);
            Console.WriteLine("status: " + credentials.status);
            Console.WriteLine("twofa: " + credentials.twofa);
        }

        [TestMethod]
        public void CrearCredencialesTokenTest()
        {
            Console.WriteLine(" Crear las credenciales de Token (test): " + test_site.id_site);
            credentials_params.RemoveAll();
            credentials_params = new JObject(new JProperty("username", "test"), new JProperty("password", "test"));
            Console.WriteLine(credentials_params.ToString());
            credentials = credentials.init(session, "", test_site.id_site, credentials_params);
            Console.WriteLine("id_credential: " + credentials.id_credential);
            Console.WriteLine("username: " + credentials.username);
            Console.WriteLine("status: " + credentials.status);
            Console.WriteLine("twofa: " + credentials.twofa);
        }

        [TestMethod]
        public void ChecaStatusParaIntroducirToken()
        {
            Console.WriteLine("Checa el estatus y espera el code 410 para introducir token");
            bool code_410 = false;
            JArray statuses;
            while (!code_410)
            {
                Console.WriteLine("....");
                System.Threading.Thread.Sleep(1);
                statuses = credentials.get_status(session);
                foreach (var item in statuses.Children())
                {
                    var itemProperties = item.Children<JProperty>();
                    var status_item = itemProperties.FirstOrDefault(x => x.Name == "code" && x.Value.ToString() == "410");
                    if (status_item != null)
                    {
                        code_410 = true;
                        break;
                    }
                }
            }
            Console.WriteLine("410: " + code_410.ToString());
        }

        [TestMethod]
        public void MandaToken()
        {
            Console.WriteLine("Manda el token");
            bool twofa = credentials.set_twofa(session, "", "test");
            Console.WriteLine("Token sent: " + twofa.ToString());
        }

        [TestMethod]
        public void ConsultarCuentasUsuario()
        {
            Console.WriteLine("ACCOUNTS ENDPOINTS");
            Console.WriteLine("Consultar la lista de cuentas del usuario");
            List<Account> accounts_items = accounts.get(session);
            if(accounts_items.Count>0)
                printInstance(accounts_items[0]);
            Console.WriteLine("accounts: " + accounts_items.Count);
        }

        [TestMethod]
        public void ConsultarNoTransaccionesUsuario()
        {
            Console.WriteLine("TRANSACTION ENDPOINTS");
            Console.WriteLine("Consultar el número de transacciones del usuario");
            int transactions_count = transaction.get_count(session);
            Console.WriteLine("transactions_count: " + transactions_count);
        }

        [TestMethod]
        public void ConsultarListaTransaccionesUsuario()
        {
            Console.WriteLine("Consultar la lista de transacciones del usuario");
            List<Transaction> transactions = transaction.get(session);
            Console.WriteLine("transactions_count: " + transactions.Count);
        }

        [TestMethod]
        public void ConsultarNoAttachmentUsuario()
        {
            Console.WriteLine("ATTACHMENT ENDPOINTS ");
            Console.WriteLine("Consultar el número de attachments del usuario");
            int attachments_count  = attachment.get_count(session);
            Console.WriteLine("attachments: " + attachments_count);
        }

        [TestMethod]
        public void ConsultarListaAttachmentUsuario()
        {
            Console.WriteLine("Consultar la lista de attachments del usuario");
            List<Attachment> attachments = attachment.get(session: session);

            if (attachments.Count > 0)
            {
                string id_attachment = attachments[0].url.Substring(1, attachments[0].url.Length-1);
                Console.WriteLine("id_attachment: " + id_attachment);
                Console.WriteLine("Consultar el contenido de un attachment especifico");
                List<Attachment> attachment_especifico = attachment.get(session, id_attachment: id_attachment);
                if (attachment_especifico.Count > 0)
                    printInstance(attachment_especifico[0]);

                Console.WriteLine("Consultar el extra de un attachment especifico");
                JObject attachment_especifico_extra = null;
                attachment_especifico_extra = attachment.get_extra(session, id_attachment: id_attachment);
                if (attachment_especifico_extra != null)
                    printInstance(attachment_especifico_extra);
            }
        }
        private void printInstance(object obj)
        {
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                Console.WriteLine("{0}={1}", name, value);
            }
        }
    }
}
