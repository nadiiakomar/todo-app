using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoAppen
{
    public class TagService
    {
        private readonly string _cs;
        public TagService(string cs)
        { _cs = cs; }
        public int GetOrCreateTag(string name)
        { using var connection = new SqlConnection(_cs);
            connection.Open();
            name = name.Trim();
            var tagId = connection.QueryFirstOrDefault<int?>(@"
                    SELECT tags.id FROM tags WHERE name=@name",
                new {name = name});
            if(tagId != null)
            { return tagId.Value; }
            else
            {
                var createdTagId = connection.QuerySingleOrDefault<int>(@"
                    INSERT INTO tags(name)
                    OUTPUT INSERTED.Id
                    VALUES (@name)",
                    new {name = name});
                return createdTagId;

            }
        }

        public void ConnectTag(int todoId, int tagId)
        { using var connection = new SqlConnection(_cs);
            connection.Open();
            try
            {
                connection.Execute(@"
                INSERT INTO todoTags (todo_id, tags_id)
                        VALUES (@todoId,@tagId)",
                            new { todoId = todoId, tagId = tagId }
                                );
            }
            catch(SqlException ex) { Console.WriteLine(ex.Message); }
        }
    }
}
