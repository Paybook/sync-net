
##QUICKSTART BANCO NORMAL

A lo largo de este tutorial te enseñaremos como sincronizar una institución bancaria normal, es decir, aquella que solo requiere una autenticación (usuario y contraseña), ejemplos de estas instituciones pueden ser Banamex o Santander. En el tutorial asumiremos que ya hemos creado usuarios y por tanto tenemos usuarios ligados a nuestra API KEY, también asumiremos que hemos instalado la librería de python y hecho las configuraciones pertinentes. Si tienes dudas acerca de esto te recomendamos que antes de tomar este tutorial consultes el [Quickstart para sincronizar al SAT](https://github.com/Paybook/sync-py/blob/master/quickstart_sat.md) ya que aquí se abordan los temas de creación de usuarios y sesiones.  

### Requerimientos

1. Haber consultado el tutorial [Quickstart para sincronizar al SAT](https://github.com/Paybook/sync-net/blob/master/quickstart_sat.cs)
2. Tener credenciales de alguna institución bancaria que cuente con autenticación simple (usuario y contraseña)

##Ejecución:

Este tutorial está basado en el script [quickstart_normal_bank.cs](https://github.com/Paybook/sync-net/blob/master/quickstart_normal_bank.cs) por lo que puedes descargar el archivo, configurar los valores YOUR_API_KEY, YOUR_BANK_USERNAME y YOUR_BANK_PASSWORD y ejecutarlo en tu equipo:

```
$ quickstart_normal_bank.cs
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
catalogues = new Catalogues();
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

Para efectos de este tutorial seleccionaremos **SuperNET Particulares (Santander)** pero tu puedes seleccionar la institución de la cual tienes credenciales.

```C#
Site bank_site = new Site();
catalogues = new Catalogues();
List<Site> sites = catalogues.get_sites(currentSession);
sites.ForEach(s =>
{
	Console.WriteLine(s.name);
	if (s.name == "SuperNET Particulares")
	{
		bank_site = s;
	}
});
Console.WriteLine("Bank site: " + bank_site.name + " " + bank_site.id_site);
```

####3. Registramos las credenciales:

A continuación registraremos las credenciales de nuestro banco, es decir, el usuario y contraseña que nos proporcionó el banco para acceder a sus servicios en línea:

```C#
credentials = new Credentials();
JObject new_credentials = new JObject(new JProperty("username", BANK_USERNAME), new JProperty("password", BANK_PASSWORD));
Credentials bank_credentials = credentials.init(currentSession, id_site: bank_site.id_site, new_credentials);
Console.WriteLine(bank_credentials.id_credential + " " + bank_credentials.username);
```
####4. Checamos el estatus

Una vez que has registrado las credenciales de una institución bancaria para un usuario en Paybook el siguiente paso consiste en checar el estatus de las credenciales, el estatus será una lista con los diferentes estados por los que las credenciales han pasado, el último será el estado actual. A continuación se describen los diferentes estados de las credenciales:

| Código         | Descripción                                |                                
| -------------- | ---------------------------------------- | ------------------------------------ |
| 100 | Credenciales registradas   | 
| 101 | Validando credenciales  | 
| 401      | Credenciales inválidas    |
| 102      | La institución se está sincronizando    |
| 200      | La institución ha sido sincronizada    | 

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

Esto quiere decir que las credenciales han sido registradas y se están validando. Puesto que la institución bancaria a sincronizar i.e. Santander, no requiere autenticación de dos pasos e.g. token o captcha podemos únicamente checar el estatus buscando que las credenciales hayan sido validadads (código 102) o bien hayan sido inválidas (código 401)

```C#
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
			Console.WriteLine("Error en credenciales");
			break;
		}
	}
}
```

####6. Esperamos a que la sincronización termine

Una vez que la sincronización se encuentra en proceso (código 102), podemos construir un bucle para polear y esperar por el estatus de fin de sincronización (código 200).

```C#
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
```

####7. Consultamos las transacciones de la institución bancaria:

Una vez que la sincronización ha terminado podemos consultar las transacciones:

```C#
Console.WriteLine("Getting transactions ...");
transaction = new Transaction();
List<Transaction> transactions = transaction.get(currentSession);
Console.WriteLine("Transactions: " + transactions.Count);
```

Podemos desplegar información de las transacciones:

```C#
int i = 0;
foreach (var item in transactions)
{
	i++;
	Console.WriteLine(i + "." + item.description + " $" +item.amount);
}
```

¡Felicidades! has terminado con este tutorial.

###Siguientes Pasos

- Revisar el tutorial de como sincronizar una institución bancaria con token [aquí](https://github.com/Paybook/sync-net/blob/master/quickstart_token_bank.md)

- Puedes consultar y analizar la documentación completa de la librearía [aquí](https://github.com/Paybook/sync-net/blob/master/readme.md)

- Puedes consultar y analizar la documentación del API REST [aquí](https://www.paybook.com/sync/docs#api-Overview)

- Acceder a nuestro proyecto en Github y checar todos los recursos que Paybook tiene para ti [aquí](https://github.com/Paybook)














