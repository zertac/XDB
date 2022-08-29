# XDB.Net Framework MySql Connection Layer & Local Cache System

**Description :** This project targeting easy to use connect to MySql database, crawl data and cache data in local machine.

**Installation :** Download project and add to your solution. Add as project reference to your project and start to use.

**Requirements :** .Net Framework 6,  MySql.Data, # Newtonsoft.Json

    Install-Package MySql.Data -Version 8.0.30

    Install-Package Newtonsoft.Json -Version 13.0.1

## Usage

**Configure Your Connection String**
```cs
    XDB.XDBConfigurator.SetConnection("Server=xxx.xxx.xxx.xxx;Port=3306; Database=xxx;Uid=xxx;Pwd=xxxx;Connect Timeout=30;Pooling=True;CharSet=utf8;");
```
**Init Your Configurator** / **Load Queries**

    XDB.XDBConfigurator.Init(XDB.LoadType.PROCEDURE, "database_name");
    
    XDB.XDBConfigurator.Init(XDB.LoadType.FOLDER, "database_name", "C:\\sql");

There is 2 options for load your queries into Configurator.

> **Option 1 :** XDB.LoadType.PROCEDURE This option will read all stored procedures from your database and configurator will prepare statement
> parameters for execute.
> 
> **Option 2 :** XDB.LoadType.PROCEDURE.FOLDER This option will read your queries from specified folder with *.sql files extensions.

**Retrive Data**

When you want retrieve data from database you have to pass <T> type parameter to function.

**Example**

**Return List Array**

      public class User
      {
          public int Id { get;set;}
          public string UserName { get;set;}
          public string LoginDate { get;set;}
          public bool Status { get;set;}
      }

    var result = XDB.Main.GetData<List<User>>("get_users");

or return Single Row

    var result = XDB.Main.GetData<User>("get_user");

call with parameters

    var paramters = new Dictionary(string,object);
    paramater.add("id",12345);
    
    var result = XDB.Main.GetData<User>("get_user_by_id",parameters);

**Handle Zero Row / Custom Error**

When you try retrieve data from database if returned row count is zero then result will be null. if you want to handle zero with custom handler you can use as in example.

> var result = XDB.Main.GetData<List<User>>("get_users", null, (error)
> =>
>     {
>         if (error.Error == -1)
>         {
>             Console.WriteLine("no data found");
>         }
>         else
>         {
	>             	Console.WriteLine("Error No : " + error.Error + " Description : ", error.Description);
>         }
>     });

**Define Custom Error Declaration Stored Procedure**
Just put this code inside your stored procedure and return ERROR and DESCRIPTION fields.

> Select 1 AS ERROR, "user not found" AS DESCRIPTION;

**Using Local Cache**
This option when you execute db command then data will retrieve from database and whole data will save on the memory. When you execute second time same command by key, data will return from memory until CacheTimeout;

**Configure Cache**
XDBConfigurator.UseCache = true;
XDBConfigurator.CacheTimeout = 3; //minute

> **1th Call** 
> var result = XDB.Main.GetData<User>("get_user"); // Data will
> retrieve from database
> 
> **2th Call**
> var result = XDB.Main.GetData<User>("get_user"); // Data will
> retrieve from memory

**Flush Cache**

    XDB.Main.FlushCache();

Example Database Struct

![enter image description here](https://github.com/zertac/XDB/blob/main/Screenshots/db.jpg)

![enter image description here](https://github.com/zertac/XDB/blob/main/Screenshots/pr1.jpg)

![enter image description here](https://github.com/zertac/XDB/blob/main/Screenshots/pr2.jpg)
