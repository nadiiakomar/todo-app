using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ToDoAppen
    {
        public class Database
        {
            private readonly string _cs;
            public Database(string cs)
            {
                _cs = cs;
                Init();
            }

            private void Init()
            {
                using var connection = new SqlConnection(_cs); // connection string 

                connection.Open();
            //ANPASSA TILL MS SQL 
            connection.Execute(@" 
                               IF NOT EXISTS 
                                    (SELECT 1 FROM sys.tables WHERE name='users')
		                      CREATE TABLE users (
                                            id INT IDENTITY(1,1) PRIMARY KEY,
                                            user_name NVARCHAR(20) NOT NULL UNIQUE,
                                            password NVARCHAR(20)NOT NULL,
                                            name NVARCHAR(20) NOT NULL)
                ");
              
                    connection.Execute(@"
                                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='lists')
			                CREATE TABLE lists(
								                id INT IDENTITY(1,1) PRIMARY KEY,
								                user_id INT NOT NULL,
								                name NVARCHAR(20) NOT NULL,
								                UNIQUE(user_id, name),
								                FOREIGN KEY (user_id) REFERENCES users(id)
								                ON DELETE CASCADE)");
                /* --- todos tabell --- */
          
                    connection.Execute(@"
                                IF NOT EXISTS	
	                    (SELECT 1 FROM sys.tables WHERE name='todos')
		                    CREATE TABLE todos(
							                    id INT IDENTITY(1,1) PRIMARY KEY,
							                    title NVARCHAR(40) NOT NULL,
							                    is_urgent BIT NOT NULL,                    -- Tidigare hade check på 0/1
							                    is_important BIT NOT NULL,
							                    is_done BIT NOT NULL DEFAULT 0,
							                    list_id INT NOT NULL,
							                    FOREIGN KEY (list_id) REFERENCES lists(id)
							                    ON DELETE NO ACTION
							                    )");

                    connection.Execute(@"

					    		 IF NOT EXISTS	
	                    (SELECT 1 FROM sys.tables WHERE name='tags')
		                    CREATE TABLE tags(
							                    id INT IDENTITY(1,1) PRIMARY KEY,
												name NVARCHAR(40) NOT NULL UNIQUE,
							                    )");
            connection.Execute(@"
                                IF NOT EXISTS	
	                    (SELECT 1 FROM sys.tables WHERE name='todoTags')
		                    CREATE TABLE todoTags(
							                    todo_id INT,
												tags_id INT,
												PRIMARY KEY(todo_id, tags_id),
												FOREIGN KEY (todo_id) REFERENCES todos(id) ON DELETE CASCADE,
												FOREIGN KEY (tags_id) REFERENCES tags(id) ON DELETE CASCADE
							                    )
");
                   
                }
            }
        }
 