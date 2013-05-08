using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace L3tha1Achievements
{
    public static class Simulate
    {

        /// <summary>
        /// Creates new Player with random values and a null ID.
        /// </summary>
        static public Player RandomPlayer()
        {
            return RandomPlayer(null);
        }
        /// <summary>
        /// Creates a new random Player
        /// </summary>
        /// <param name="ID">Player ID</param>
        static public Player RandomPlayer(int? ID)
        {
            Player TempPlayer = new Player();
            TempPlayer.ID = ID;
            //The random limits of these variables are proportional to each other in
            //ways that make the data look better.  They don't neccessarily
            //have any real meaning to how they are generated.
            TempPlayer.Attacks = gv.rand.Next(0, 200);
            TempPlayer.Hits = gv.rand.Next(0, TempPlayer.Attacks);
            TempPlayer.Damage = gv.rand.Next(TempPlayer.Hits, 6 * TempPlayer.Hits);
            TempPlayer.Kills = gv.rand.Next(0, TempPlayer.Hits / 4);
            //they way I interpret "last hit" kills from the 
            //description is like CS in league
            TempPlayer.LastHits = gv.rand.Next(0, 325);
            TempPlayer.Assists = gv.rand.Next(0, 25);
            TempPlayer.Spells = gv.rand.Next(0, 100);
            TempPlayer.SpellDamage = gv.rand.Next(0, 8 * TempPlayer.Spells);
            TempPlayer.Deaths = gv.rand.Next(0, 35);
            TempPlayer.Escapes = gv.rand.Next(0, 10);
            //For testings sake there is a 1/50 chance that the player leaves
            //That number doesn't really come from anywhere
            TempPlayer.Leave = !Convert.ToBoolean(gv.rand.Next(0, 50));
            return TempPlayer;
        }
        static public Game RandomGame()
        {
            Game TempGame = new Game();
            int Num = gv.rand.Next(3, 6);//number of players
            List<int> RandIds = GetRandomIDs();
            for (int i = 1; i <= Num; i++)
            {
                TempGame.Team1.Add(RandomPlayer(RandIds[(2 * i - 2)]));
                TempGame.Team2.Add(RandomPlayer(RandIds[2 * i - 1]));
            }
            //b/w 20min and 60min
            TempGame.RunningTime = TimeSpan.FromSeconds(gv.rand.Next(1200, 3601));
            //this should give the team with more kills a better chance at winning
            //since that would make sense in game
            TempGame.Win = (gv.rand.Next(0, (TempGame.T1Kills + TempGame.T2Kills)) < TempGame.T1Kills);
            return TempGame;
        }

        static public Game RandomTeamGame(int Team1ID, int Team2ID)
        {

            Game TempGame = new Game();
            try
            {
                SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                SQLiteCommand Team1Command = new SQLiteCommand("SELECT * FROM TeamPlayers WHERE TeamID = " + Team1ID, conn);
                SQLiteCommand Team2Command = new SQLiteCommand("SELECT * FROM TeamPlayers WHERE TeamID = " + Team2ID, conn);
                conn.Open();
                DataTable Team1Results = new DataTable();
                DataTable Team2Results = new DataTable();
                Team1Results.Load(Team1Command.ExecuteReader());
                Team2Results.Load(Team2Command.ExecuteReader());
                conn.Close();
                foreach (DataRow D1 in Team1Results.Rows)
                {
                    TempGame.Team1.Add(RandomPlayer(Convert.ToInt32(D1["PlayerID"])));
                }
                foreach (DataRow D2 in Team2Results.Rows)
                {
                    TempGame.Team2.Add(RandomPlayer(Convert.ToInt32(D2["PlayerID"])));
                }
                //b/w 20min and 60min
                TempGame.RunningTime = TimeSpan.FromSeconds(gv.rand.Next(1200, 3601));
                //this should give the team with more kills a better chance at winning
                //since that would make sense in game
                TempGame.Win = (gv.rand.Next(0, (TempGame.T1Kills + TempGame.T2Kills)) < TempGame.T1Kills);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return TempGame;
        }
        static private List<int> GetRandomIDs()
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                SQLiteCommand SelectCommand = new SQLiteCommand("SELECT ID FROM Player", conn);
                conn.Open();
                DataTable Results = new DataTable();
                Results.Load(SelectCommand.ExecuteReader());
                conn.Close();
                List<int> temp = new List<int>();
                while (temp.Count < 10)
                {
                    int num = gv.rand.Next(0, Results.Rows.Count);
                    if (!temp.Contains(Convert.ToInt32(Results.Rows[num][0].ToString())))
                    {
                        temp.Add(Convert.ToInt32(Results.Rows[num][0].ToString()));
                    }
                }
                return temp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            }
        }

        static public Tournament RandomTournament()
        {
            Stack<int> Temp = new Stack<int>();
            for (int i = 1; i <= 8; i++)
            {
                Temp.Push(i);
            }
            return BuildTournament(Temp);
        }
        static private Tournament BuildTournament(Stack<int> TeamIDs)
        {
            if (Math.Log((TeamIDs.Count), 2) % 1 == 0)
            {
                Tournament Temp = new Tournament();
                int DepthRequired = Convert.ToInt32(Math.Log((TeamIDs.Count), 2));
                int NodeID = 0;
                Temp.Champion = new node(null);



                SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                conn.Open();
                SQLiteCommand Insert = new SQLiteCommand("INSERT INTO Tournaments (Name) Values ('Random Tournament')", conn);
                SQLiteCommand GetTournamentID = new SQLiteCommand("select last_insert_rowid()", conn);
                Insert.ExecuteNonQuery();
                Temp.ID = Convert.ToInt32(GetTournamentID.ExecuteScalar());


                conn.Close();


                BuildTournametRecursiveCall(ref NodeID, TeamIDs, Temp.Champion, DepthRequired, Convert.ToInt32(Temp.ID));
                return Temp;
            }
            return new Tournament();
        }
        static private void BuildTournametRecursiveCall(ref int NodeID, Stack<int> TeamIDs, node current, int DepthRequired, int TournamentID)
        {
            current.ID = NodeID;
            NodeID++;
            if (current.Parent == null)
            {
                current.Depth = 0;
            }
            else
            {
                current.Depth = 1 + current.Parent.Depth;
            }
            if (current.Depth == DepthRequired - 1)
            {
                current.Team1ID = TeamIDs.Pop();
                current.Team2ID = TeamIDs.Pop();
            }
            else
            {
                current.Left = new node(current);
                current.Right = new node(current);
                BuildTournametRecursiveCall(ref NodeID, TeamIDs, current.Left, DepthRequired, TournamentID);
                BuildTournametRecursiveCall(ref NodeID, TeamIDs, current.Right, DepthRequired, TournamentID);
                if (current.Left.TournamentGame.Win)
                {
                    current.Team1ID = current.Left.Team1ID;
                }
                else
                {
                    current.Team1ID = current.Left.Team2ID;
                }
                if (current.Right.TournamentGame.Win)
                {
                    current.Team2ID = current.Right.Team1ID;
                }
                else
                {
                    current.Team2ID = current.Right.Team2ID;
                }
            }
            current.TournamentGame = RandomTeamGame(current.Team1ID, current.Team2ID);
            GameOver.Run(current.TournamentGame);            
            SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
            SQLiteCommand UploadTournamentGame = new SQLiteCommand("INSERT INTO TournamentGames (TournamentID, TournamentGameID, GameID, Team1ID, Team2ID) " +
                                                        "Values (" + TournamentID + "," + current.ID + "," + current.TournamentGame.ID + ", " +
                                                            current.Team1ID + ", " + current.Team2ID + ")", conn);
            conn.Open();
            UploadTournamentGame.ExecuteNonQuery();
            conn.Close();
        }
    }
}
