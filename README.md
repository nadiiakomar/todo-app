# ToDoAppen - Eisenhower Model
Detta är en konsolbaserat ToDo-app som är byggd i C# med SQL Server och Dapper. 

Appen låter användare skapa och hantera sina uppgifter enligt Eisehower-modellen (Viktigt/Inte viktigt och Brådskande/Inte Brådskande)
## Funktionalitet
I appen kan man:
- skapa konto och logga in
- skapa egna listor
- lägga till todos
- Visa todos i 4 kvadranter (enligt Eisenhower-modellen)
- Markera todos som klara
- Lägga till taggar på todos

## Databas
**Tabeller:**
- users 
- lists
- todos
- tags
- todoTags

**Relationer:**
- En användare kan ha flera listor (one-to-many)
- En lista kan ha flera todos (one-to-many)
- En todo kan ha flera tags (many-to-many via todoTags)

**Designval:**
- UNIQUE på listnamn per användare, flera användare kan ha samma namn på lista, men en användare kan inte ha flera listor med samma namn)
- Cmposite primary key i todoTags
- ON DELETE CASCADE i relationstabellen
- ON DELETE NO ACTION mellan lists och todos

## Databasstruktur
Database är normaliserad och anväder foreign keys. Many-to-many relationen mellan todos och tags hanteras via tabellen **todoTags** men en samansatt primärnyckel: ``PRIMARY KEY(todo_id, tag_id)``, vilket förhindrar dubbla kopplingar mellan samma todo och tag.

## Installation (Docker och SQL Server)
**1. Starta SQL Server med Docker.**

   Kör i terminalen:
    
``docker pull mcr.microsoft.com/mssql/server:2022-latest``

``docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Secret-NET.25-Password!" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest``

**2. Skapa databas**

 Anslut via SSMS och kör:

 ``CREATE DATABASE net25_db;``
 
**3. Kör applikationen:**

    
 Kör via Visual Studio eller terminalen:
 
``dotnet run``

