using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoAppen
{
    public class ToDoService
    {
        private readonly string _cs;

        public ToDoService(string connectionString)
        { _cs = connectionString; }


        public int AddToDo(string title, bool isUrgent, bool isImportant, int list)
        {
            using var connection = new SqlConnection(_cs);
            connection.Open();
            int AddedTodo = connection.QuerySingle<int>(@"
                        INSERT INTO todos 
                            ( title, is_urgent, is_important, list_id)
                        OUTPUT INSERTED.Id
                        VALUES 
                            ( @title, @urgent, @important, @list)",
                new {title = title, urgent = isUrgent, important = isImportant, list = list});
            return AddedTodo;
            
        }

        public List<ToDo> GetTodos(int userId, bool isUrgent, bool isImportant)
        {
            using var connection = new SqlConnection(_cs);
            connection.Open();
            List<ToDo> ToDos  = connection.Query<ToDo>(@"
                    SELECT 
                        todos.title AS Title, 
                        todos.is_urgent AS IsUrgent,
                        todos.is_important AS IsImportant,
                        todos.is_done AS IsDone,
                        todos.id AS ToDoId,
                        lists.id AS ListId,
                        lists.name AS ListName

                    FROM todos
                    JOIN lists ON todos.list_id = lists.id
                        WHERE 
                            lists.user_id = @userId AND
                            is_urgent = @isUrgent AND
                            is_important = @isImportant AND
                            is_done = 0",
                            new {userId = userId, isUrgent=isUrgent, isImportant = isImportant}
                            ).ToList();
            return ToDos;

        }



        public void MarkAsDone(int selectedToDoId)
        {
            using var connection = new SqlConnection(_cs);
            connection.Open();
            connection.Execute(@"
		          UPDATE todos 
		          SET is_done = 1
		          WHERE todos.id = @selectedToDoId",
                  new { selectedToDoId = selectedToDoId});
        }

        //TODO: GetToDosByList metod för att visa listan 
        public List<ToDo> GetToDosByList(int userId, int listId)
        { using var connection = new SqlConnection(_cs);
            connection.Open();
            List<ToDo> todosList = connection.Query<ToDo>(@"
                    SELECT 
                        todos.title AS Title, 
                        todos.is_urgent AS IsUrgent,
                        todos.is_important AS IsImportant,
                        todos.is_done AS IsDone,
                        todos.id AS ToDoId,
                        lists.id AS ListId,
                        lists.name AS ListName

                    FROM todos
                    JOIN lists ON todos.list_id = lists.id
                        WHERE 
                            lists.user_id = @userId AND
                            lists.id=@listId AND
                            is_done = 0",
                            new {userId = userId, listId=listId}
                            ).ToList();
            return todosList;
        }
    }
}
