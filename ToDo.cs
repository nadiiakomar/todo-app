using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoAppen
{
    public class ToDo
    {
        public int ToDoId { get; set; }
        public int ListId { get; set; }
        public string Title { get; set; } = "";
        public bool IsImportant { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsDone { get; set; }
        public string ListName { get; set; } = "";
    }
}
