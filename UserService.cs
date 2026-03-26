using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

// TODO: Lägga till transaction hantering så det få hanteras bra med listaskapelse?


namespace ToDoAppen
{
    public class UserService
    {
        private readonly string _cs;
        public UserService(string connectionString)
        { _cs = connectionString; }


        //SKAPA KONTO FLYTTA HIT
        public bool Register(string username, string passwordInput, string name)
        {
            using var connection = new SqlConnection(_cs);
            connection.Open();
            //lägg till skapande av osorterad lista automatiskt när man skapar user
            using var transaktion = connection.BeginTransaction();
            try
            {
                var userId = connection.QuerySingle<int>(@"
                    INSERT INTO users 
                        (user_name, password, name)
                    OUTPUT INSERTED.Id --returnerar just skapat Id
                    VALUES 
                        (@user_name, @password, @name)",
                    new { name = name, password = passwordInput, user_name = username },
                    transaktion);
                
                 CreateBasicList(connection,transaktion, userId);
                transaktion.Commit();
                return userId > 0; // kollar upp om finns påverkade rader 
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                transaktion.Rollback();
                return false;
            }


        }
        public (int id, string name)? Login(string username, string password)
        {
            using var connection = new SqlConnection(_cs);
            connection.Open();                                  // Query returnerar en lista, QuerySingleOrDefault returnerar ett värde 
            var user = connection.QuerySingleOrDefault<User>(@"
                    SELECT
                        id, 
                        name, 
                        password  
                    FROM users
                    WHERE 
                        user_name = @user_name",
            new { user_name = username });

            if( user == null)
            { return null; }

            if(user.Password == password)
            { return (user.Id, user.Name); }
            else
            { return null; }

        
        }

        public void CreateBasicList(SqlConnection connection,SqlTransaction transaktion, int user_id)
        {
            connection.Execute(@"
                    INSERT INTO lists (user_id, name)
                    VALUES (@user_id, @name)",
                    new {user_id = user_id, name = "Osorterad" },
                    transaktion
                    );
        }
    }
}

public class User
{
    public int Id { get; set; }
    public string User_name { get; set; } = "";
    public string Password { get; set; } = "";
    public string Name { get; set; } = "";
}