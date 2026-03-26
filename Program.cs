using Dapper;
using Microsoft.Data.SqlClient;

namespace ToDoAppen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string cs = "Server=localhost,1433;Database=net25_db; User ID=net25;Password=Secret-NET.25-Password!;Encrypt=True;TrustServerCertificate=True;";
            using var connection = new SqlConnection(cs);
            var db = new Database(cs); // connection string 
            var userService = new UserService(cs);
            var todoService = new ToDoService(cs);
            var listService = new ListsService(cs);
            var inloggadMenu = new InloggadMenu();
            var tagService = new TagService(cs);


            int? inloggadUserId = null;
            string? inloggadUserName = null;
            


            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(" ==========================================");
                Console.WriteLine();
                Console.WriteLine("         TODO APP - Eisenhower Model   ");
                Console.WriteLine();
                Console.WriteLine(" ==========================================");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  Hantera dina todos enligt Eisenhower -");
                Console.WriteLine("  metoden      ≽^•⩊•^≼ ");
                Console.ResetColor();

                /* ----- INLOGGAD USER MENY ----- */
                if (inloggadUserId != null)
                {
                    Console.WriteLine($"  Inloggad som:  {inloggadUserName} ");
                    Console.WriteLine(" -----------------------------------------");
                    Console.WriteLine(" [1] Lägg till ny uppgift");
                    Console.WriteLine(" [2] Visa alla uppgifter Eizenhover Sortering");
                    Console.WriteLine(" [3] Visa alla uppgifter på List Sortering ");
                    Console.WriteLine(" [4] Skapa lista");
                    Console.WriteLine(" [5] Logga ut");

                    Console.WriteLine(" -----------------------------------------");
                    Console.WriteLine(" Välj alternativ (1-3): ");
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                       
                        case ConsoleKey.D1: /* LÄGGA TILL TODO */
                            inloggadMenu.CreateNewToDo(inloggadUserId.Value, listService, todoService, tagService);
                            break;



                        case ConsoleKey.D2: /* VISA LISTAN */
                            inloggadMenu.ShowEizenhoverList(inloggadUserId.Value, todoService);
                            break;

                            // SLUT HÄR ----------------------------------------------------------------------------------------------------------------------------------

                        case ConsoleKey.D3:
                            inloggadMenu.ShowListsTodo(inloggadUserId.Value, todoService, listService);
                            break;

                        case ConsoleKey.D4:
                            inloggadMenu.CreateNewList(inloggadUserId.Value, listService);
                            break;
                          

                        case ConsoleKey.D5:
                        case ConsoleKey.NumPad3: /* -- AVSLUTA -- */
                            Console.WriteLine("Hej då :)");
                            Console.ReadLine();
                            return;

                        default: /* VALDE NÅT ANNAT ÄN 1/2 */
                            Console.WriteLine(" -----------------------------------------");
                            Console.WriteLine(" Nånting gick fel. Försök igen");
                            Console.ReadLine();
                            continue;
                    }
                  
            }
                else // ANVÄNDARE ÄR INTE INLOGGAD 
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine(" [1] Skapa konto");
                    Console.WriteLine(" [2] Logga in");
                    Console.WriteLine(" [3] Avsluta");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(" -----------------------------------------");
                    Console.Write("  Välj alternativ (1-3): ");
                    Console.ResetColor();
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    //string? choice = Console.ReadLine();
                    switch (key.Key)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:       /* -- SKAPA KONTO -- */
                            while (true)
                            {
                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.WriteLine(" ==========================================");
                                Console.WriteLine();
                                Console.WriteLine("                 SKAPA KONTO               ");
                                Console.WriteLine();
                                Console.WriteLine(" ==========================================");
                                Console.ResetColor();
                                Console.Write(" Välj username: ");
                                string username = Console.ReadLine()!.Trim();
                                if (string.IsNullOrWhiteSpace(username) || username.Any(char.IsWhiteSpace))
                                {
                                    Console.WriteLine(" -----------------------------------------");
                                    Console.WriteLine(" Ogiltig username. Försök igen!Tryck Enter");
                                    Console.ReadLine();
                                    continue;
                                }


                                Console.Write(" Välj lösenord: ");
                                string password = Console.ReadLine()!.Trim();
                                if (string.IsNullOrWhiteSpace(password))
                                {
                                    Console.WriteLine(" Ogiltig password. Försök igen.Tryck Enter");
                                    Console.ReadLine();
                                    continue;
                                }


                                Console.Write(" Vad heter du? ");
                                string name = Console.ReadLine()!.Trim();
                                if (string.IsNullOrWhiteSpace(name))
                                {
                                    Console.WriteLine(" -----------------------------------------");
                                    Console.WriteLine(" Ogiltigt namn. Försök igen!Tryck Enter");
                                    Console.ReadLine();
                                    continue;
                                }


                                bool success = userService.Register(username, password, name);
                                Console.WriteLine(success ? " Konto skapades :)" : " Nånting gick fel");
                                break;
                            }
                            Console.ReadLine();
                            Console.Clear();
                            break;

                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine(" ==========================================");
                            Console.WriteLine();
                            Console.WriteLine("                   LOGGA IN                ");
                            Console.WriteLine();
                            Console.WriteLine(" ==========================================");
                            Console.ResetColor();

                            Console.Write(" Username:  ");
                            string loginUserName = Console.ReadLine()!;

                            Console.Write(" Lösenord:  ");
                            string loginPassword = Console.ReadLine()!;

                            var successLogin = userService.Login(loginUserName, loginPassword);
                            if (successLogin != null)
                            {
                                inloggadUserId = successLogin.Value.id;
                                inloggadUserName = successLogin.Value.name;
                                Console.WriteLine();
                                Console.WriteLine(" -----------------------------------------");
                                Console.WriteLine($" Vällkommen, {loginUserName}");
                                Console.WriteLine(" Tryck Enter för att forsätta..");

                            }
                            else
                            {
                                Console.WriteLine(" -----------------------------------------");
                                Console.WriteLine(" Fel användarnamn eller lösenord eller användare saknas.");
                            }

                            Console.ReadLine();
                            Console.Clear();
                            break;

                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3: /* -- AVSLUTA -- */
                            Console.WriteLine(" -----------------------------------------");
                            Console.WriteLine(" Hej då :)");
                            Console.ReadLine();
                            return;

                        default: /* -- NÅT ANNAT -- */
                            Console.WriteLine(" -----------------------------------------");
                            Console.WriteLine(" Nånting gick fel");
                            Console.WriteLine(" Tryck enter för att försöka igen");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                    }
                }


            }

        }


    }

    }


