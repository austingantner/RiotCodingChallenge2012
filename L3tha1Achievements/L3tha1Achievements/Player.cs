using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace L3tha1Achievements
{
    public class Player
    {
        public int? ID = null;
        //Game Stats
        public int Attacks = 0;
        public int Hits = 0;
        public int Damage = 0;
        public int Kills = 0;
        public int LastHits = 0;
        public int Assists = 0;
        public int Spells = 0;
        public int SpellDamage = 0;
        public int Deaths = 0;
        //an escape is when a player survives a fight with less
        //than 10% of his health
        public int Escapes = 0;
        public bool Leave = false;
        //historic statis
        public int TotalAttacks = 0;
        public int TotalHits = 0;
        public int TotalDamage = 0;
        public int TotalKills = 0;
        public int TotalLastHits = 0;
        public int TotalAssists = 0;
        public int TotalSpells = 0;
        public int TotalSpellDamage = 0;
        public int TotalDeaths = 0;
        public int TotalEscapes = 0;
        public TimeSpan TotalPlayTime = TimeSpan.FromSeconds(0);
        public int TotalWins = 0;
        public int TotalLosses = 0;
        public int TotalLeaves = 0;
        //List of Achievement ID's that were completed this game
        //name and descriptions are stored in AchievementMaster
        public List<int> Achievements = new List<int>();

        public void LoadHistoricData()
        {
            if (ID != null)
            {
                try
                {
                    //rather than run a select statement for each achievement I run a general one here
                    SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                    SQLiteCommand PlayerStats = new SQLiteCommand("SELECT * FROM Player WHERE ID = " + ID, conn);
                    conn.Open();
                    DataTable PlayerStatsResults = new DataTable();
                    PlayerStatsResults.Load(PlayerStats.ExecuteReader());
                    conn.Close();
                    if (PlayerStatsResults.Rows.Count == 1)
                    {
                        TotalAttacks = Convert.ToInt32(PlayerStatsResults.Rows[0]["Attacks"]);
                        TotalHits = Convert.ToInt32(PlayerStatsResults.Rows[0]["Hits"]);
                        TotalDamage = Convert.ToInt32(PlayerStatsResults.Rows[0]["Damage"]);
                        TotalKills = Convert.ToInt32(PlayerStatsResults.Rows[0]["Kills"]);
                        TotalLastHits = Convert.ToInt32(PlayerStatsResults.Rows[0]["LastHits"]);
                        TotalAssists = Convert.ToInt32(PlayerStatsResults.Rows[0]["Assists"]);
                        TotalSpells = Convert.ToInt32(PlayerStatsResults.Rows[0]["Spells"]);
                        TotalSpellDamage = Convert.ToInt32(PlayerStatsResults.Rows[0]["SpellDamage"]);
                        TotalDeaths = Convert.ToInt32(PlayerStatsResults.Rows[0]["Deaths"]);
                        TotalEscapes = Convert.ToInt32(PlayerStatsResults.Rows[0]["Escapes"]);
                        TotalPlayTime = TimeSpan.FromSeconds(Convert.ToInt32(PlayerStatsResults.Rows[0]["Time"]));
                        TotalWins = Convert.ToInt32(PlayerStatsResults.Rows[0]["Wins"]);
                        TotalLosses = Convert.ToInt32(PlayerStatsResults.Rows[0]["Losses"]);
                        TotalLeaves = Convert.ToInt32(PlayerStatsResults.Rows[0]["Leaves"]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
