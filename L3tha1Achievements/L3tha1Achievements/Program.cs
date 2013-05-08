/*
 * Programmer: Austin Gantner
 * SummonerName: L3tha1Am6iti0n
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
namespace L3tha1Achievements
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (TestConnection())
            {
                bool done = false;
                while (!done)
                {
                    string menu = "MENU:\n" +
                                  "1.Simulate Game With Results\n" +
                                  "2.Run X Games\n"+
                                  "3.View Specific Game\n"+
                                  "4.Simulate Tournament\n"+
                                  "5.View Specific Tournament\n"+
                                  "6.Done";
                    Console.Clear();
                    Console.WriteLine(menu);
                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.Clear();
                            SimOneGame();
                            break;
                        case "2":
                            Console.Clear();
                            RunXGames();
                            break;
                        case "3":
                            Console.Clear();
                            ViewSpecificGame();
                            break;
                        case "4":
                            Console.Clear();
                            SimTournament();
                            break;
                        case "5":
                            Console.Clear();
                            ViewSpecificTournament();
                            break;
                        case "6":
                            done = true;
                            break;
                    }
                }
            }
        }

        public static bool TestConnection()
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                conn.Open();
                conn.Close();
            }
            catch
            {
                Console.WriteLine("Error. Could not find database file.");
                Console.WriteLine("In GlobalVariables.cs set \"private static string LocationOfDBFile\" to the location of LAData.db.");
                Console.WriteLine("Further instructions are written in comments there");
                Console.WriteLine("Program tried connection string: " + gv.ConnectionString);
                Console.Read();
                return false;
            }
            return true;
        }

        static void SimOneGame()
        {
            Game test = Simulate.RandomGame();
            GameOver.Run(test);
            test.Print();
            Console.Read();
        }

        static void RunXGames()
        {
            try
            {
                Console.WriteLine("How many games would you like to run? (0-50)");
                int temp = Convert.ToInt32(Console.ReadLine());
                if (temp <= 50)
                {
                    for (int i = 0; i < temp; i++)
                    {
                        Console.Write('.');
                        Game Test = Simulate.RandomGame();
                        GameOver.Run(Test);
                    }
                }
                else
                {
                    Console.WriteLine("That is too large");
                }
                Console.WriteLine("Done");
                Console.Read();
            }
            catch
            {
                Console.WriteLine("Error. That was not an integer. Press Enter");
            }
        }

        static void ViewSpecificGame()
        {
            try
            {
                Console.WriteLine("Game ID: ");
                int temp = Convert.ToInt32(Console.ReadLine());
                Game Test = new Game(temp);
                Test.Print();                
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }

        static void SimTournament()
        {
            Tournament Test = Simulate.RandomTournament();
            
            bool done = false;
            while (!done)
            {
                string menu = "\n\nMENU:\n" +
                              "1.View Game\n" +
                              "2.MainMenu";
                Console.Clear();
                Test.Print();
                Console.WriteLine(menu);
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("Game ID: ");
                        Test.ViewGame(Convert.ToInt32(Console.ReadLine()));
                        Console.Read();
                        break;
                    case "2":
                        done = true;
                        break;
                }
            }
        }

        static void ViewSpecificTournament()
        {
            try
            {
                Console.WriteLine("Tournament ID: ");
                int temp = Convert.ToInt32(Console.ReadLine());
                Console.Clear();
                Tournament Test = new Tournament(temp);
                bool done = false;
                while (!done)
                {
                    string menu = "\n\nMENU:\n" +
                                  "1.View Game\n" +
                                  "2.MainMenu";
                    Console.Clear();
                    Test.Print();
                    Console.WriteLine(menu);
                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.WriteLine("Game ID: ");
                            Test.ViewGame(Convert.ToInt32(Console.ReadLine()));
                            Console.Read();
                            break;
                        case "2":
                            done = true;
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error. That was not an integer. Press Enter");
            }
        }
    }
}
