
##QUICKSTART SAT

### Requerimientos

1. Manejo de proyectos tipo consola
2. Algunas credenciales de acceso al SAT (RFC y CIEC)
3. Visual Studio 2015(recomended)
4. Tener instalado la libreria Paybook desde el Nuget Package

### Introducción

A lo largo de este tutorial te enseñaremos como consumir el API Rest de Paybook por medio de la librería de Paybook. Al terminar este tutorial habrás podido crear nuevos usuarios en Paybook, sincronizar algunas instituciones de estos usuarios y visualizar las transacciones sincronizadas.

La documentación completa de la librería la puedes consultar [aquí](https://github.com/Paybook/sync-net/blob/master/README.md) 

##En la consola:

####1. Instalamos la librería de Paybook y dependencias:

Para consumir el API de Paybook lo primero que tenemos que hacer es instalar la libreria de Paybook haciendo uso del paquete de instalaciones (En visual studio ir a Tools/Nuget Package Manager/Package Manager Console):

```
PM > Install-Package Paybook
```

**Importante: ** Es posible que la ejecución del comando anterior requiera que visual studio se ejecute como administrador.

####2. Ejecutamos el Script:
Este tutorial está basado en el script [quickstart_sat.cs](https://github.com/Paybook/sync-net/blob/master/Paybook/QuickStarts/QuickStarts/quickstart_sat.cs) de la solucion PayBook.sln, por lo que puedes descargar toda la solucion de la [carpeta](https://github.com/Paybook/sync-net/tree/master/Paybook), configurar como projecto de inicio el archivo "Quickstarts.csproj", configurar los valores YOUR_API_KEY, YOUR_RFC y YOUR_CIEC, compilar toda la solucion y ejecutarlo en tu equipo (F5):

```C#
public class Program
{
	static void Main(string[] args)
	{
	    //uncomment this section in order to execute quickstart_normal
		//quickstart_normal normal = new quickstart_normal();
		//normal.start();

		quickstart_sat sat = new quickstart_sat();
		sat.start();

		//uncomment this section in order to execute quickstart_token_bank
		//quickstart_token_bank token_bank = new quickstart_token_bank();
		//token_bank.start();
	}
}
```

A continuación explicaremos detalladamente la lógica del script que acabas de ejecutar.

####3. Importamos paybook
El primer paso es importar la librería y algunas dependencias:

```C#
using PaybookSDK;
using Newtonsoft.Json.Linq;
```

####4. Configuramos la librería
Una vez importada la librería tenemos que configurarla, para esto únicamente se necesita tu API KEY de Paybook.

```C#
 paybook._init(YOUR_API_KEY);
```

####5. Creamos un usuario:
Una vez configurada la librería, el primer paso será crear un usuario, este usuario será, por ejemplo, aquel del cual queremos obtener sus facturas del SAT.

**Importante**: todo usuario estará ligado al API KEY con el que configuraste la librería (paso 4)

```C#
user = new User();
new_user = user.init("MY_USER");
```

####6. Consultamos los usuarios ligados a nuestra API KEY:
Para verificar que el usuario creado en el paso 5 se haya creado corréctamente podemos consultar la lista de usuarios ligados a nuestra API KEY.

```C#
List<User> my_users = user.get();

foreach (var user in my_users)
{
	Console.WriteLine(user.name);
}
```

####7. Creamos una nueva sesión:
Para sincronizar las facturas del SAT primero tenemos que crear una sesión, la sesión estará ligada al usuario y tiene un proceso de expiración de 5 minutos después de que ésta ha estado inactiva. Para crear una sesión:

```C#
session = new Session();
Session currentSession = session.init(new_user);
Console.WriteLine("Session token: " + currentSession.token);
```

####8. Podemos validar la sesión creada:
De manera opcional podemos validar la sesión, es decir, checar que no haya expirado.

```C#
bool session_verified = currentSession.verify();
Console.WriteLine("Session verified: " + session_verified);
```

####9. Consultamos el catálogo de instituciones que podemos sincronizar y extraemos el SAT:
Paybook tiene un catálogo de instituciones que podemos sincronizar por usuario:

![Instituciones](https://github.com/Paybook/sync-py/blob/master/sites.png "Instituciones")

A continuación consultaremos este catálogo y seleccionaremos el sitio del SAT para sincronizar las facturas del usuario que hemos creado en el paso 5:

```C#
catalogues = new Catalogues();
Site sat_site = new Site();
List<Site> sites = catalogues.get_sites(currentSession);

sites.ForEach(s =>
{
	Console.WriteLine(s.name);
	if (s.name == "CIEC")
		sat_site = s;
});
Console.WriteLine("SAT site: " + sat_site.id_site + " " + sat_site.name);
```

####10. Configuramos nuestras credenciales del SAT:
Una vez que hemos obtenido el sitio del SAT del catálogo de institiciones, configuraremos las credenciales de nuestro usuario (estas credenciales son las que el usuario utiliza para acceder al portal del SAT).

```C#
credentials = new Credentials();
JObject credentials_data = new JObject(new JProperty("rfc", "SOME_RFC"), new JProperty("password", "SOME_CIEC"));
Credentials sat_credentials = credentials.init(currentSession, id_site: sat_site.id_site, credentials: credentials_data);
Console.WriteLine(sat_credentials.username);
```

####11. Checamos el estatus de sincronización de las credenciales creadas y esperamos a que la sincronización finalice:
Cada vez que registamos unas credenciales Paybook inicia un Job (proceso) que se encargará de validar esas credenciales y posteriormente sincronizar las transacciones. Este Job se puede representar como una maquina de estados:

![Job Estatus](https://github.com/Paybook/sync-py/blob/master/normal.png "Job Estatus")

Una vez registradas las credenciales se obtiene el primer estado (Código 100), posteriormente una vez que el Job ha empezado se obtiene el segundo estado (Código 101). Después de aquí, en caso de que las credenciales sean válidas, prosiguen los estados 202, 201 o 200. Estos indican que la sincronización está en proceso (código 201), que no se encontraron transacciones (código 202), o bien, la sincronización ha terminado (código 200). La librería proporciona un método para consultar el estado actual del Job. Este método se puede ejecutar constantemente hasta que se obtenga el estado requerido por el usuario, para este ejemplo especifico consultamos el estado hasta que se obtenga un código 200, es decir, que la sincronización haya terminado:

```C#
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
```

####12. Consultamos las facturas sincronizadas:
Una vez que ya hemos checado el estado de la sincronización y hemos verificado que ha terminado (código 200) podemos consultar las facturas sincronizadas:
```C#
transaction = new Transaction();
List<Transaction> sat_transactions = transaction.get(currentSession);
Console.WriteLine("Facturas del SAT: " + sat_transactions.Count);
```

####13. Consultamos la información de archivos adjuntos:
Podemos también consultar los archivos adjuntos a estas facturas, recordemos que por cada factura el SAT tiene una archivo XML y un archivo PDF:
```C#
attachment = new Attachment();
List<Attachment> attachments = attachment.get(currentSession);
Console.WriteLine("Archivos XML/PDF del SAT: " + attachments.Count);
```

####14. Obtenemos el XML y PDF de alguna factura:
Podemos descargar estos archivos:
```C#
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
```

¡Felicidades! has terminado con este tutorial. 

### Siguientes Pasos

- Revisar el tutorial de como sincronizar una institución bancaria con credenciales simples (usuario y contraseña) [aquí](https://github.com/Paybook/sync-net/blob/master/quickstart_normal_bank.md)

- Revisar el tutorial de como sincronizar una institución bancaria con token [aquí](https://github.com/Paybook/sync-net/blob/master/quickstart_token_bank.md)

- Puedes consultar y analizar la documentación completa de la librería [aquí](https://github.com/Paybook/sync-net/blob/master/README.md)

- Puedes consultar y analizar la documentación del API REST [aquí](https://www.paybook.com/sync/docs#api-Overview)

- Acceder a nuestro proyecto en Github y checar todos los recursos que Paybook tiene para ti [aquí](https://github.com/Paybook)


























