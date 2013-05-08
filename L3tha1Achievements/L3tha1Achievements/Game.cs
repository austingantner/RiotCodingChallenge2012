using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;


namespace L3tha1Achievements
{
    public class Game
    {
        public int? ID = null; //only used if game is pulled from database
        /// <summary>
        /// Win = true if team1 wins and false if team 2 wins
        /// </summary>
        public bool Win = true;
        public TimeSpan RunningTime = TimeSpan.FromSeconds(0);
        public List<Player> Team1 = new List<Player>();
        public List<Player> Team2 = new List<Player>();
        /*=== Calculated Values ===*/
        public int T1Kills
        {
            get
            {
                int temp = 0;
                foreach (Player p in Team1)
                {
                    temp += p.Kills;
                }
                return temp;
            }
        }
        public int T2Kills
        {
            get
            {
                int temp = 0;
                foreach (Player p in Team2)
                {
                    temp += p.Kills;
                }
                return temp;
            }
        }
        public int T1Assists
        {
            get
            {
                int temp = 0;
                foreach (Player p in Team1)
                {
                    temp += p.Assists;
                }
                return temp;
            }
        }
        public int T2Assists
        {
            get
            {
                int temp = 0;
                foreach (Player p in Team2)
                {
                    temp += p.Assists;
                }
                return temp;
            }
        }

        public Game()
        {

        }
        public Game(int LoadID)
        {
            LoadGame(LoadID);
        }

        private void LoadGame(int LoadID)
        {
            try
            {
                ID = LoadID;
                SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                conn.Open();
                SQLiteCommand GetGameInfo = new SQLiteCommand("SELECT * FROM Games WHERE ID = " + LoadID, conn);
                DataTable GameInfo = new DataTable();
                GameInfo.Load(GetGameInfo.ExecuteReader());
                if (GameInfo.Rows.Count == 1)
                {
                    Win = Convert.ToBoolean(GameInfo.Rows[0]["Win"]);
                    RunningTime = TimeSpan.FromSeconds(Convert.ToInt16(GameInfo.Rows[0]["TotalTime"]));
                }

                SQLiteCommand GetPlayerInfo = new SQLiteCommand("SELECT * FROM PerGameStats WHERE GameID = " + LoadID, conn);
                DataTable PlayerInfo = new DataTable();
                PlayerInfo.Load(GetPlayerInfo.ExecuteReader());
                foreach (DataRow D in PlayerInfo.Rows)
                {
                    Player TempPlayer = new Player();
                    TempPlayer.ID = Convert.ToInt32(D["PlayerID"]);
                    TempPlayer.Attacks = Convert.ToInt32(D["Attacks"]);
                    TempPlayer.Hits = Convert.ToInt32(D["Hits"]);
                    TempPlayer.Damage = Convert.ToInt32(D["Damage"]);
                    TempPlayer.Kills = Convert.ToInt32(D["Kills"]);
                    TempPlayer.LastHits = Convert.ToInt32(D["LastHits"]);
                    TempPlayer.Assists = Convert.ToInt32(D["Assists"]);
                    TempPlayer.Spells = Convert.ToInt32(D["Spells"]);
                    TempPlayer.SpellDamage = Convert.ToInt32(D["SpellDamage"]);
                    TempPlayer.Deaths = Convert.ToInt32(D["Deaths"]);
                    TempPlayer.Escapes = Convert.ToInt32(D["Escapes"]);

                    SQLiteCommand GetAchievementInfo = new SQLiteCommand("SELECT * FROM Achievements WHERE PlayerID = " +
                                                                TempPlayer.ID + " AND GameID = " + LoadID, conn);
                    DataTable AchievementInfo = new DataTable();
                    AchievementInfo.Load(GetAchievementInfo.ExecuteReader());

                    foreach (DataRow A in AchievementInfo.Rows)
                    {
                        TempPlayer.Achievements.Add(Convert.ToInt32(A["AchievementID"]));
                    }

                    if (Convert.ToBoolean(D["Team1"]))
                    {
                        Team1.Add(TempPlayer);
                    }
                    else
                    {
                        Team2.Add(TempPlayer);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Print()
        {
            if (ID != null)
            {
                Console.Write("Game: " + ID);
            }
            Console.WriteLine("                               Winner: " + (Win ? "Team1" : "Team2"));
            Console.WriteLine("Running Time: " + RunningTime.ToString());
            Console.WriteLine("================================================================================");
            Console.WriteLine("          Team 1                                       Team 2");
            Console.WriteLine("Kills: " + T1Kills + "   Assists: " + T1Assists + "                      Kills: " + T2Kills + "   Assists: " + T2Assists);
            for (int i = 0; i < Team1.Count; i++)
            {
                Console.WriteLine("================================================================================");
                Console.WriteLine("PlayerID: " + Team1[i].ID + "                                PlayerID: " + Team2[i].ID);
                Console.WriteLine("Attacks: " + Team1[i].Attacks + "                                Attacks: " + Team2[i].Attacks);
                Console.WriteLine("Hits: " + Team1[i].Hits + "                                    Hits: " + Team2[i].Hits);
                Console.WriteLine("Damage: " + Team1[i].Damage + "                                  Damage: " + Team2[i].Damage);
                Console.WriteLine("Kills: " + Team1[i].Kills + "                                    Kills: " + Team2[i].Kills);
                Console.WriteLine("Last Hits: " + Team1[i].LastHits + "                               Last Hits: " + Team2[i].LastHits);
                Console.WriteLine("Assists: " + Team1[i].Assists + "                                 Assists: " + Team2[i].Assists);
                Console.WriteLine("Spells: " + Team1[i].Spells + "                                  Spells: " + Team2[i].Spells);
                Console.WriteLine("Spell Damage: " + Team1[i].SpellDamage + "                           Spell Damage: " + Team2[i].SpellDamage);
                Console.WriteLine("Deaths: " + Team1[i].Deaths + "                                  Deaths: " + Team2[i].Deaths);
                Console.WriteLine("================================================================================");
                if (Team1[i].Achievements.Count > 0)
                {
                    Console.WriteLine("Player " + Team1[i].ID + " achievements");
                    foreach (int A in Team1[i].Achievements)
                    {
                        Console.WriteLine(gv.Achievements[A]);
                    }
                }
                if (Team2[i].Achievements.Count > 0)
                {
                    Console.WriteLine("Player " + Team2[i].ID + " achievements");
                    foreach (int A in Team2[i].Achievements)
                    {
                        Console.WriteLine(gv.Achievements[A]);
                    }
                }
            }
            Console.WriteLine("================================================================================");
            Console.WriteLine("Press Enter When Done");
        }
    }
}
