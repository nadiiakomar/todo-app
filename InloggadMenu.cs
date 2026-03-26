using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoAppen
{
    public class InloggadMenu
    {
        public InloggadMenu()
        {}
        public void CreateNewToDo(int userId, ListsService listService, ToDoService todoService, TagService tagService)
        {
            bool isUrgent;
            bool isImportant;
            int selectedToDoListId = 0;
            Console.Clear();
            Console.WriteLine(" =========================================");
            Console.WriteLine();
            Console.WriteLine("                 SKAPA NY UPPGIFT         ");
            Console.WriteLine();
            Console.WriteLine(" =========================================");
            Console.Write(" Vad ska göras?             ");
            string title = Console.ReadLine()!;

            /* --- VIKTIGT? --- */
            while (true)
            {

                Console.Write(" Är todo viktigt?    (J/N):  ");
                ConsoleKeyInfo keyJaNej = Console.ReadKey(true);

                if (keyJaNej.Key == ConsoleKey.J)
                {
                    isImportant = true;
                    Console.WriteLine("Ja");
                    break;
                }
                else if (keyJaNej.Key == ConsoleKey.N)
                {
                    isImportant = false;
                    Console.WriteLine("Nej");
                    break;
                }
                else { Console.WriteLine(" Fel skrivet, försök igen!"); }
                Console.ReadLine();
                Console.Clear();
            }
            ;

            /* --- BRÅDSKA? --- */
            while (true)
            {

                Console.Write(" Är todo brådskande? (J/N):  ");

                ConsoleKeyInfo keyJaNej = Console.ReadKey(true);

                if (keyJaNej.Key == ConsoleKey.J)
                {
                    isUrgent = true;
                    Console.WriteLine("Ja");

                    break;
                }
                else if (keyJaNej.Key == ConsoleKey.N)
                {
                    isUrgent = false;
                    Console.WriteLine("Nej");
                    break;
                }
                else { Console.WriteLine(" Fel skrivet, försök igen!"); }
                Console.ReadLine();
                Console.Clear();
            }
            ;

            while (true)
            {
                Console.WriteLine(" -----------------------------------------");
                Console.WriteLine(" Vill du lägga till i lista?");
                Console.WriteLine(" (J/N - standard: Osorterad)");
                ConsoleKeyInfo keyJaNej = Console.ReadKey(true);

                if (keyJaNej.Key == ConsoleKey.J)
                {
                    Console.WriteLine();
                    Console.WriteLine(" DINA LISTOR:");
                    var toDoLists = listService.GetListsByUser(userId);
                    for (int i = 0; i < toDoLists.Count; i++)
                    {
                        Console.Write($"  [{i + 1}]");
                        Console.WriteLine($" {toDoLists[i].Name}");
                    }
                    Console.WriteLine(" Välj lista och tryck [Enter]. Om listan saknas skriv namn på lista för att skapa den.");

                    //Lägg till att skapa lista i fall den saknas 
                    var markedToDoList = Console.ReadLine()!;
                    bool success = int.TryParse(markedToDoList, out int markedToDoListNumber);
                    if (success)
                    {
                        markedToDoListNumber -= 1;
                        if (markedToDoListNumber < 0 || markedToDoListNumber >= toDoLists.Count)
                        {
                            Console.WriteLine(" -----------------------------------------");
                            Console.WriteLine(" Prova att skriva rätt index igen!");
                            continue;
                        }
                        else 
                        {
                            var selectedTodoList = toDoLists[markedToDoListNumber];
                            selectedToDoListId = selectedTodoList.Id;
                            break;
                        }
                    }
                    else //skriven inte siffran
                    {
                        Console.WriteLine(" -----------------------------------------");
                        Console.WriteLine(" En ny listan är skapat och to do lagt till :)");
                        //Skapar en ny lista här

                        if (!string.IsNullOrWhiteSpace(markedToDoList))
                        {

                            int createList = listService.GetOrCreateList(userId, markedToDoList); //markedtodolist är text här, 
                            selectedToDoListId = createList;
                            break;


                        }
                        else
                        {
                            Console.WriteLine("Ogiltigt namn, prova igen");
                            Console.WriteLine(" Tryck [Enter] för att fortsätta...");
                            Console.ReadLine();
                            Console.Clear();
                            continue;
                        }
                    }

                }
                else if (keyJaNej.Key == ConsoleKey.N)
                {

                    selectedToDoListId = listService.GetDefaultListId(userId);

                    break;
                }
            }

                    var todoId = todoService.AddToDo(
                                    title,
                                    isUrgent,
                                    isImportant,
                                    selectedToDoListId
                                     );

            while (true)
            {
                Console.WriteLine(" -----------------------------------------");
                Console.WriteLine(" Vill du lägga till taggar? (J/N)");
                ConsoleKeyInfo keyJaNej = Console.ReadKey(true);

                if (keyJaNej.Key == ConsoleKey.J)
                {
                    Console.Write("Skriv namn för tag: ");
                    string tagName = Console.ReadLine()!;
                    if (string.IsNullOrWhiteSpace(tagName))
                    {
                        Console.WriteLine("Prova skriva igen eller tryck X för att inte lägga till taggar");
                        continue;
                    }
                    else
                    {
                        var tagId = tagService.GetOrCreateTag(tagName);
                        tagService.ConnectTag(todoId, tagId);
                    }
                    // Frågar om namn på tagget, om det finns tar ID, inte -> skapar ny tag
                    break;
                }
                if (keyJaNej.Key == ConsoleKey.N)
                {
                    break;
                }
                else { continue; }
            }
            Console.WriteLine(" -----------------------------------------");
            Console.WriteLine(" Todo sparat!");
            Console.WriteLine(" -----------------------------------------");
            Console.WriteLine(" Tryck [Enter] för att fortsätta");
            Console.ReadLine();
            Console.Clear();
        }


        public void ShowEizenhoverList(int userId, ToDoService todoService)
        {    //BÖRJAN HÄR  -------------------------------------------------------------------------------------------------------------------------------
            Console.Clear();
            Console.WriteLine(" ============================================");
            Console.WriteLine();
            Console.WriteLine("               TODO DASHBOARD Eizenhover  ");
            Console.WriteLine();
            Console.WriteLine(" ============================================");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine();
            Console.WriteLine(" --------------- KVADRANT 1 ---------------");
            Console.WriteLine("             Viktigt & Brådska          ");
            Console.ResetColor();

            var prio1 = todoService.GetTodos(userId, true, true);
            if (prio1.Count == 0)
            { Console.WriteLine(" (Inga todos) "); }
            else
            {
                foreach (var todo in prio1)
                { Console.WriteLine($" - {todo.Title}   [{todo.ListName}]"); }
            }


            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine();
            Console.WriteLine(" --------------- KVADRANT 2 ---------------");
            Console.WriteLine("             Viktigt & Inte Brådska      ");
            Console.ResetColor();

            var prio2 = todoService.GetTodos(userId, true, false);
            if (prio2.Count == 0)
            { Console.WriteLine(" (Inga todos) "); }
            else
            {
                foreach (var todo in prio2)
                { Console.WriteLine($" - {todo.Title}   [{todo.ListName}]"); }
            }
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine();
            Console.WriteLine(" --------------- KVADRANT 3 ---------------");
            Console.WriteLine("        Inte Viktigt & Brådska           ");
            Console.ResetColor();

            var prio3 = todoService.GetTodos(userId, false, true);
            if (prio3.Count == 0)
            { Console.WriteLine(" (Inga todos) "); }
            else
            {
                foreach (var todo in prio3)
                { Console.WriteLine($" - {todo.Title}   [{todo.ListName}]"); }
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine(" --------------- KVADRANT 4 ---------------");
            Console.WriteLine("        Inte Viktigt & Inte Brådska      ");
            Console.ResetColor();

            var prio4 = todoService.GetTodos(userId, false, false);
            if (prio4.Count == 0)
            { Console.WriteLine(" (Inga todos) "); }
            else
            {
                foreach (var todo in prio4)
                { Console.WriteLine($" - {todo.Title}   [{todo.ListName}]"); }
            }

            Console.WriteLine(" --------------------------------------------");
            Console.WriteLine("  [Enter x2] Tillbaka  [D] Markera som klar");
            Console.WriteLine(" --------------------------------------------");
            List<ToDo> markList = new List<ToDo>();
            ConsoleKeyInfo done = Console.ReadKey(true);
            switch (done.Key)
            {
                case ConsoleKey.D:
                    Console.WriteLine(" Välj kvadrant i vilken du vill markera klar todo:");
                    ConsoleKeyInfo prioListNumber = Console.ReadKey(true);
                    switch (prioListNumber.Key)
                    {
                        case ConsoleKey.D1:
                            markList = prio1;

                            break;
                        case ConsoleKey.D2:
                            markList = prio2;

                            break;
                        case ConsoleKey.D3:
                            markList = prio3;

                            break;
                        case ConsoleKey.D4:
                            markList = prio4;

                            break;

                        default:
                            break;
                    }
                    Console.Clear();

                    for (int i = 0; i < markList.Count; i++)
                    {
                        Console.Write(i + 1);
                        Console.WriteLine($".  {markList[i].Title}");
                    }

                    Console.WriteLine("Skriv nummer på todo som du vill markera klar: ");
                    while (true)
                    {
                        var markedToDo = Console.ReadLine()!;
                        int markedToDoNumber;
                        bool success = int.TryParse(markedToDo, out markedToDoNumber);
                        if (success)
                        {
                            markedToDoNumber -= 1;
                            if (markedToDoNumber < 0 || markedToDoNumber >= markList.Count)
                            {
                                Console.WriteLine(" -----------------------------------------");
                                Console.WriteLine(" Prova att skriva rätt index igen!");
                                continue;
                            }
                            else
                            {
                                var selectedTodo = markList[markedToDoNumber];
                                var selectedToDoId = selectedTodo.ToDoId;
                                todoService.MarkAsDone(selectedToDoId);

                                Console.WriteLine(" -----------------------------------------");
                                Console.WriteLine(" Bra jobbat! :) ToDo är markerat som klar!");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine(" -----------------------------------------");
                            Console.WriteLine(" Prova att skriva rätt index igen!");
                        }
                    }
                    break;
                case ConsoleKey.Enter:
                    break;
            }
            Console.ReadLine();
            Console.Clear();
        }

        public void ShowListsTodo(int userId, ToDoService todoService, ListsService listService)
        {
            Console.Clear();
            Console.WriteLine(" ==============================================");
            Console.WriteLine();
            Console.WriteLine("               TODO - DASHBOARD Lists      ");
            Console.WriteLine();
            Console.WriteLine(" ==============================================");
            var allToDos = new List<ToDo>();
            var lists = listService.GetListsByUser(userId);
            foreach (var list in lists)
            // TODO: Lägg till färg på ListNamn
            {
                Console.WriteLine($"  ------------{list.Name}------------");
                var todos = todoService.GetToDosByList(userId, list.Id);
                if (!todos.Any())
                {
                    Console.WriteLine("  Inga todos än");

                }
                foreach (var todo in todos)
                {
                    Console.WriteLine($"  {allToDos.Count + 1}. {todo.Title}");
                    allToDos.Add(todo);

                }
                Console.WriteLine();

            }
            //TODO Ändra till en metod
            Console.WriteLine(" ----------------------------------------------");
            Console.WriteLine("  [Enter x2] Tillbaka    [D] Markera som klar");
            Console.WriteLine(" ----------------------------------------------");
            ConsoleKeyInfo doneList = Console.ReadKey(true);
            switch (doneList.Key)
            {
                case ConsoleKey.D:
                    Console.WriteLine(" Välj nummer att markera klar todo:");
                    var input = Console.ReadLine();
                    if (int.TryParse(input, out int index))
                    {
                        index = index - 1;
                        if (index >= 0 && index < allToDos.Count)
                        {
                            var selected = allToDos[index];
                            todoService.MarkAsDone(selected.ToDoId);
                            Console.WriteLine("Bra jobbat! Todo markerat som klar");
                            break;

                        }
                    }
                    Console.ReadLine();

                    Console.Clear();
                    break;
            }
            Console.ReadLine();
            Console.Clear();
        }


        public void CreateNewList(int userId, ListsService listService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(" =========================================");
                Console.WriteLine();
                Console.WriteLine("                 SKAPA NY LISTA         ");
                Console.WriteLine();
                Console.WriteLine(" =========================================");
                Console.Write(" Vad ska listan heta?             ");
                var listName = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(listName))
                {
                    try
                    {
                        int success = listService.GetOrCreateList(userId, listName);
                        Console.WriteLine(" Listan har skapats :)");
                        Console.WriteLine(" Tryck [Enter] för att fortsätta...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Listan finns redan");
                        Console.WriteLine(" Tryck [Enter] för att fortsätta...");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    }



                }
                else
                {
                    Console.WriteLine("Ogiltigt namn, prova igen");
                    Console.WriteLine(" Tryck [Enter] för att fortsätta...");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }
            }
        }
    }
}
