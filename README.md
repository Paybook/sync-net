# sync-net

**Overview**
----
	-- This is a lightweight Paybook SDK for .NET, wich allows you to consume Paybook methods.

**Prerequisites**
----
    -- Visual Studio 2022 or above
    -- Microsoft .NET Framework 8.0 or above

**Downloading from Github**
----
	-- Open in Visual Studio
	-- Create a new web or webapi project
    -- Download Paybook.dll from this site which is contained into lib folder
	-- Create a folder called "lib" in your main project folder
	-- Paste Paybook.dll into lib folder
	-- Go back to Visual Studio
	-- Right click on References
	-- On contextual menu select the option "Add Reference..." 
	-- Click on "Browse" button, in find Paybook.dll
	-- Click on "Add" button
	-- Click on "OK" button
	-- You will see on References a Paybook.dll added
	
**Downloading from Nuget Gallery**
----
    -- In progress
    	
**Signup**
----
	Register a new user to Paybook

* **Method**
  
	string signup(string username)

   **Required:**
 
	`username=[string]`

* **Success Response:**

	`id_user=[string]`
 
* **Error Response:**
  
	`Exception=[Exception]`

* **Sample Call:**

    ```
	Paybook paybook = new Paybook();
	string id_user = paybook.signup("some username");
	```

**Login**
----
  Login an exisiting user

* **Method:**
  
	string login(string id_user)

   **Required:**
 
	`id_user=[string]`

* **Success Response:**
  
	`token=[string]`
 
* **Error Response:**
  
	`Exception=[Exception]`

* **Sample Call:**

    ```
	Paybook paybook = new Paybook();
	token = paybook.login("some id_user");
	```

**Catalogs**
----
  Retrieve the set of institutions available

* **Method:**
  
	string catalogs(string token)

   **Required:**
 
	`token=[string]`

* **Success Response:**
  
	`response=[string]`
	Note: this response is a JSON Object
 
* **Error Response:**
  
	`Exception=[Exception]`

* **Sample Call:**
	
	```
	Paybook paybook = new Paybook();
	token = paybook.login("some id_user");
	```

**Credentials**
----
  Register credentials for a specific institution
  
* **Method:**
  
	JObject credentials(JObject newcredentials)

   **Required:**
 
	`newcredentials=[JObject]`

* **Success Response:**
  
	`response=[JObject]`
	Note: this response is a JSON Object
 
* **Error Response:**
  
	`Exception=[Exception]`

* **Sample Call:**
	
	```
	Paybook paybook = new Paybook();
	JObject credentials = new JObject();
    JObject new_credentials = paybook.credentials(credentials);
	```
	
**Status**
----
  Get the sync status of a specific institution

* **Method:**
  
	string status(string token, string id_site, string url_status)

   **Required:**
 
	`token=[string]`
	`id_site=[string]`
	`url_status=[string]`
	
* **Success Response:**
  
	`response=[string]`
	Note: this response is a JSON Object
	
* **Error Response:**
  
	`Exception=[Exception]`

* **Sample Call:**
	
	```
	Paybook paybook = new Paybook();
    status = paybook.status("some token", "some id_site", "some url_status");
	```
	
**Twofa**
----
  Set twofa to a specific institution

* **Method:**
  
	string twofa(JObject twofa, string address

   **Required:**
 
	`twofa=[JObject]`
	`address=[string]`

* **Success Response:**
  
	`response=[string]`
	Note: this response is a JSON Object
	
* **Error Response:**
  
	`Exception=[Exception]`

* **Sample Call:**
	
	```
    paybook.twofa({"token":"some token"}, "some url twofa");
	```
  
**Accounts**
----
  Get the accounts registered in a specific institution

* **Method:**
  
	string accounts(string token, string id_site)

   **Required:**
 
	`token=[string]`
	`id_site=[string]`
	
* **Success Response:**
  
	`response=[string]`
	Note: this response is a JSON Object
	
* **Error Response:**
  
	`Exception=[Exception]`

* **Sample Call:**

	```
	Paybook paybook = new Paybook();
    accounts = paybook.accounts("some token", "some id_site");
	```
  
**Transactions**
----
  Get the accounts registered in a specific account

* **Method:**
  
	string transactions(string token, string id_account)

   **Required:**
 
	`token=[string]`
	`id_account=[string]`
	
* **Success Response:**
  
	`response=[string]`
	Note: this response is a JSON Object
	
* **Error Response:**
  
	`Exception=[Exception]`

* **Sample Call:**

	```
	Paybook paybook = new Paybook();
	transactions = paybook.transactions("some token", "some id_account");
	```
