
##QUICKSTART BANCO CON TOKEN

A lo largo de este tutorial te enseñaremos como sincronizar una institución bancaria con token, es decir, aquella que además de requerir una autenticación (usuario y contraseña) solicita un token, ejemplos de estas instituciones pueden ser Banorte o BBVA Bancomer. En el tutorial asumiremos que ya hemos creado usuarios y por tanto tenemos usuarios ligados a nuestra API KEY, también asumiremos que hemos instalado la librería de python y hecho las configuraciones pertinentes. Si tienes dudas acerca de esto te recomendamos que antes de tomar este tutorial consultes el [Quickstart para sincronizar al SAT](https://github.com/Paybook/sync-py/blob/master/quickstart_sat.md) ya que aquí se abordan los temas de creación de usuarios y sesiones.  

### Requerimientos

1. Haber consultado el tutorial [Quickstart General](https://github.com/Paybook/sync-net/blob/master/quickstart_normal_bank.md)
2. Tener credenciales de alguna institución bancaria del catálogo de Paybook que solicite token


##Ejecución:

Este tutorial está basado en el script [quickstart_token_bank.cs](https://github.com/Paybook/sync-net/blob/master/Paybook/QuickStarts/QuickStarts/quickstart_token_bank.cs) de la solucion PayBook.sln, por lo que puedes descargar toda la solucion de la [carpeta](https://github.com/Paybook/sync-net/tree/master/Paybook), configurar como projecto de inicio el archivo "Quickstarts.csproj", configurar los valores YOUR_API_KEY, YOUR_BANK_USERNAME y YOUR_BANK_PASSWORD compilar toda la solucion y ejecutarlo en tu equipo (F5). Es recomendable tener el token a la mano puesto que el script eventualmente lo solicitará:

```
public class Program
{
	static void Main(string[] args)
	{
	    //uncomment this section in order to execute quickstart_normal
		//quickstart_normal normal = new quickstart_normal();
		//normal.start();

		//uncomment this section in order to execute quickstart_sat
		//quickstart_sat sat = new quickstart_sat();
		//sat.start();

		quickstart_token_bank token_bank = new quickstart_token_bank();
		token_bank.start();
	}
}
```

Una vez que has ejecutado el archivo podemos continuar analizando el código.

####1. Obtenemos un usuario e iniciamos sesión:
El primer paso para realizar la mayoría de las acciones en Paybook es tener un usuario e iniciar una sesión, por lo tanto haremos una consulta de nuestra lista de usuarios y seleccionaremos el usuario con el que deseamos trabajar. Una vez que tenemos al usuario iniciamos sesión con éste.


```C#
user = new User();
List<User> user_list = user.get();
currentUser = user_list[0];
Console.WriteLine(currentUser.name + " " + currentUser.id_user);

session = new Session();
currentSession = session.init(currentUser);
Console.WriteLine(currentSession.token);
```

####2. Consultamos el catálogo de las instituciones de Paybook:
Recordemos que Paybook tiene un catálogo de instituciones que podemos seleccionar para sincronizar nuestros usuarios. A continuación consultaremos este catálogo:

```C#
List<Site> sites = catalogues.get_sites(currentSession);
sites.ForEach(s =>
{
	Console.WriteLine(s.name);
});
```

El catálogo muestra las siguienes instituciones:

1. AfirmeNet
2. Personal
3. BancaNet Personal
4. eBanRegio
5. Banorte Personal
6. CIEC
7. Banorte en su empresa
8. BancaNet Empresarial
9. Banca Personal
10. Corporativo
11. Banco Azteca
12. American Express México
13. SuperNET Particulares
14. ScotiaWeb
15. Empresas
16. InbuRed

Para efectos de este tutorial seleccionaremos **Banorte en su empresa** pero tu puedes seleccionar la institución de la cual tienes credenciales y token.

```C#
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
```

####3. Registramos las credenciales:

A continuación registraremos las credenciales de nuestro banco, es decir, el usuario y contraseña que nos proporcionó el banco para acceder a sus servicios en línea:

```C#
credentials = new Credentials();
JObject credentials_data = new JObject(new JProperty("username", BANK_USERNAME), new JProperty("password", BANK_PASSWORD));
Credentials bank_credentials = credentials.init(currentSession, id_site: bank_site.id_site, credentials: credentials_data);
Console.WriteLine(bank_credentials.id_credential + " " + bank_credentials.username);
```
####4. Checamos el estatus

Una vez que has registrado las credenciales de una institución bancaria para un usuario en Paybook el siguiente paso consiste en checar el estatus de las credenciales, el estatus será una lista con los diferentes estados por los que las credenciales han pasado, el último será el estado actual. A continuación se describen los diferentes estados de las credenciales:

## Códigos:

| Código         | Descripción                                |                                
| -------------- | ---------------------------------------- | ------------------------------------ |
| 100 | Credenciales registradas   | 
| 101 | Validando credenciales  | 
| 401      | Credenciales inválidas    |
| 410      | Esperando token   |
| 102      | La institución se está sincronizando    |
| 200      | La institución ha sido sincronizada    | 

## Maquina de estados exitosos:

![Success](https://github.com/Paybook/sync-py/blob/master/token.png "Success")

## Maquina de estados de error:

![Errores](https://github.com/Paybook/sync-py/blob/master/error.png "Errores")

**Importante** El código 401 se puede presentar múltiples veces en caso de que la autenticación con la institución bancaria requiera múltiples pasos e.g. usuario, contraseña (primera autenticación) y además token (segunda autenticación). Entonces al código 401 únicamente le puede preceder un código 100 (después de introducir usuario y password), o bien, un código 401 (después de haber introducido un token). El código 405 indica que el acceso a la cuenta bancaria está bloqueado y, por otro lado, el código 406 indica que hay una sesión iniciada en esa cuenta por lo que Paybook no puede conectarse.

Checamos el estatus de las credenciales:

```C#
sync_status = bank_credentials.get_status(currentSession);
printInstance(sync_status);
```
####5. Analizamos el estatus:

El estatus se muestra a continuación:

```
[{u'code': 100}, {u'code': 101}]
```

Esto quiere decir que las credenciales han sido registradas y se están validando. La institución bancaria a sincronizar i.e. Banorte, requiere de token por lo que debemos esperar un estatus 410, para esto podemos polear mediante un bucle sobre los estados de las credenciales hasta que se tenga un estatus 410, es decir, que el token sea solicitado:

```C#
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
```

**Importante:** En este paso también se debe contemplar que en vez de un código 410 (esperando token) se puede obtener un código 401 (credenciales inválidas) lo que implica que se deben registrar las credenciales correctas, por lo que el bucle se puede módificar para agregar esta lógica.

####6. Enviar token bancario
Ahora hay que ingresar el valor del token, el cual lo podemos solicitar en python a través de la interfaz raw_input:

```C#
Console.WriteLine("Ingresa el código de seguridad: ");
string twofa_value = Console.ReadLine();
bool twofa = bank_credentials.set_twofa(currentSession, twofa_value: twofa_value);
Console.WriteLine("Twofa: " + twofa.ToString());
```

Una vez que el token bancario es enviado, volvemos a polear por medio de un bucle buscando que el estatus sea 102, es decir, que el token haya sido validado y ahora Paybook se encuentre sincronzando a nuestra institución bancaria, o bien, buscando el estatus 401, es decir, que el token no haya sido validado y por tanto lo tengamos que volver a enviar:

```C#
Console.WriteLine("Esperando validacion de token ...");
string status_102_or_401 = string.Empty;
while (string.IsNullOrEmpty(status_102_or_401))
{
	Console.WriteLine(". . .");
	System.Threading.Thread.Sleep(5);
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
			Console.WriteLine("Error en credenciales");
			break;
		}
	}
}
```

Es importante checar el código 401 que indica que el token introducido es incorrecto, por lo que se puede programar una rutina para pedir el token nuevamente:

```C#
var status_item_error = itemProperties.FirstOrDefault(x => x.Name == "code" && x.Value.ToString() == "401");
	# Rutina para pedir el token nuevamente	
```

En caso de que el estatus sea 102 se evitará la validación previa y podremos continuar con los siguientes pasos.

####7. Esperamos a que la sincronización termine

Una vez que la sincronización se encuentra en proceso (código 102), podemos construir un bucle para polear y esperar por el estatus de fin de sincronización (código 200).

```C#
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
```

####8. Consultamos las transacciones de la institución bancaria:

Una vez que la sincronización ha terminado podemos consultar las transacciones:

```C#
transaction = new Transaction();
List<Transaction> transactions = transaction.get(currentSession);
```

Podemos desplegar información de las transacciones:

```C#
int i = 0;
foreach (var transaction_item in transactions)
{
	i++;
	Console.WriteLine(i + ". " + transaction_item.description + " $" + transaction_item.amount);
}
```

¡Felicidades! has terminado con este tutorial.

###Siguientes Pasos

- Puedes consultar y analizar la documentación completa de la librearía [aquí](https://github.com/Paybook/sync-net/blob/master/README.md)

- Puedes consultar y analizar la documentación del API REST [aquí](https://www.paybook.com/sync/docs#api-Overview)

- Acceder a nuestro proyecto en Github y checar todos los recursos que Paybook tiene para ti [aquí](https://github.com/Paybook)














