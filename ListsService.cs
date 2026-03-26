using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoAppen
{
    public class ListsService
    {
        private readonly string _cs;

        public ListsService(string connectionString)
        { _cs = connectionString; }
        public List<ToDoList> GetListsByUser(int userId)
        {
            using var connection = new SqlConnection(_cs);
            connection.Open();
            List<ToDoList> lists = connection.Query<ToDoList>(@"
            SELECT 
                    lists.id, 
                    lists.name, 
                    lists.user_id 
            FROM  lists
            WHERE 
                lists.user_id = @userId",
                new { userId = userId }).ToList();

            return lists;
        }

        public int GetDefaultListId(int userId)
        {
            using var connection = new SqlConnection(_cs);
            connection.Open();
            var resultId = connection.QuerySingleOrDefault<int?>(@"
                    SELECT lists.id 
                    FROM lists
		        	WHERE lists.name = @defaultList AND 
                          user_id=@userId",
                          new {userId = userId,defaultList = "Osorterad"});
            if (resultId == null)
            { throw new Exception("Default lista saknas"); }
            else
            {
                return resultId.Value;
            }
        }

        public int GetOrCreateList(int userId, string name) //vill kolla upp om listan finns eller inte, i fall finns inte, då skapas den och returnerar id som kan användas. 
        {
            using var connection = new SqlConnection(_cs);
            connection.Open();

            int? listExists = connection.QuerySingleOrDefault<int?>(@"
                    SELECT lists.id FROM lists
                        WHERE user_id=@userId AND lists.name = @name",
                        new {name = name, userId=userId});

            if(listExists == null)
            {
                var createdListId = connection.QuerySingle<int>(@"
                    INSERT INTO lists (user_id, name)
                    OUTPUT INSERTED.Id
                    VALUES (@userId, @name)",
                 new { userId = userId, name = name });
                return createdListId;
            }
            else { return listExists.Value; }
            
             

        }




    }
    }
