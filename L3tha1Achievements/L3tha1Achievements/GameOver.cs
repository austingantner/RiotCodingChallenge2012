using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace L3tha1Achievements
{
    public static class GameOver
    {

        static public void Run(Game G)
        {
            //check for achievements
            foreach (Player P in G.Team1)
            {
                AchievementCheck(G, P, true);
            }
            foreach (Player P in G.Team2)
            {
                AchievementCheck(G, P, false);
            }
            //upload
            UploadGame(G);
        }

        #region Achievements
        /// <summary>
        /// Checks for achievements.  Needs to be called before database updated
        /// </summary>
        static private void AchievementCheck(Game G, Player P, bool Team1)
        {
            P.LoadHistoricData();
            SharpShooter(P);
            Bruiser(P);
            Veteran(P);
            BigWinner(G, P, Team1);
            CouchPotato(G, P);
            MageRage(P);
            OneManWreckingCrew(P);
            Wingman(P);
            FinishingTouch(P);
            Worthless(P);
            Houdini(P);
            Warrior(P);
        }

        /*=== Achievements ===*/
        static private void SharpShooter(Player P)
        {
            if (P.Attacks > 0)
            {
                if (Convert.ToDouble(P.Hits) / Convert.ToDouble(P.Attacks) >= .75)
                {
                    P.Achievements.Add(1);
                }
            }
        }

        static private void Bruiser(Player P)
        {
            if (P.Damage >= 500)
            {
                P.Achievements.Add(2);
            }
        }

        static private void Veteran(Player P)
        {
            //add current game by subtracting 1 from 1000
            if (P.TotalWins + P.TotalLosses == 999)
            {
                P.Achievements.Add(3);
            }
        }

        static private void BigWinner(Game G, Player P, bool Team1)
        {
            //if this game is a win, add current game by subtracting 1 from 200
            if (((G.Win && Team1) || (!G.Win && !Team1)) && P.TotalWins == 199)
            {
                P.Achievements.Add(4);
            }
        }

        static private void CouchPotato(Game G, Player P)
        {
            if (P.ID != null && P.TotalPlayTime.TotalSeconds + G.RunningTime.TotalSeconds >= 1800000)
            {
                try
                {
                    SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                    SQLiteCommand SelectCommand = new SQLiteCommand("SELECT ID FROM Achievements WHERE PlayerID = " + P.ID + " AND AchievementID = 5", conn);
                    conn.Open();
                    DataTable Results = new DataTable();
                    Results.Load(SelectCommand.ExecuteReader());
                    conn.Close();
                    if (Results.Rows.Count == 0)
                    {
                        P.Achievements.Add(5);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static private void MageRage(Player P)
        {
            if (P.SpellDamage >= 500)
            {
                P.Achievements.Add(6);
            }
        }

        static private void OneManWreckingCrew(Player P)
        {
            if (P.ID != null && P.Kills + P.TotalKills >= 5000)
            {
                try
                {
                    SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                    SQLiteCommand SelectCommand = new SQLiteCommand("SELECT ID FROM Achievements WHERE PlayerID = " + P.ID + " AND AchievementID = 7", conn);
                    conn.Open();
                    DataTable Results = new DataTable();
                    Results.Load(SelectCommand.ExecuteReader());
                    conn.Close();
                    if (Results.Rows.Count == 0)
                    {
                        P.Achievements.Add(7);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static private void Wingman(Player P)
        {
            if (P.ID != null && P.Assists + P.TotalAssists >= 8000)
            {
                try
                {
                    SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                    SQLiteCommand SelectCommand = new SQLiteCommand("SELECT ID FROM Achievements WHERE PlayerID = " + P.ID + " AND AchievementID = 8", conn);
                    conn.Open();
                    DataTable Results = new DataTable();
                    Results.Load(SelectCommand.ExecuteReader());
                    conn.Close();
                    if (Results.Rows.Count == 0)
                    {
                        P.Achievements.Add(8);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static private void FinishingTouch(Player P)
        {
            if (P.LastHits >= 250)
            {
                P.Achievements.Add(9);
            }
        }

        static private void Worthless(Player P)
        {
            if (P.Leave && P.TotalLeaves == 99)
            {
                P.Achievements.Add(10);
            }
        }

        static private void Houdini(Player P)
        {
            if (P.Escapes > P.Deaths)
            {
                P.Achievements.Add(11);
            }
        }

        static private void Warrior(Player P)
        {
            if (P.Kills > 0 && P.Kills >= 2 * P.Deaths)
            {
                P.Achievements.Add(12);
            }
        }

        #endregion

        #region Upload
        static private void UploadGame(Game G)
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                conn.Open();
                SQLiteCommand Insert = new SQLiteCommand("INSERT INTO Games (Win, T1Kills, T1Assists, T2Kills, T2Assists, TotalTime) " +
                                                         "VALUES (" + Convert.ToInt32(G.Win) + ", " + G.T1Kills + ", " + G.T1Assists + ", " + G.T2Kills + ", " +
                                                         G.T2Assists + ", " + G.RunningTime.TotalSeconds + ")", conn);
                SQLiteCommand GetGameID = new SQLiteCommand("select last_insert_rowid()", conn);                
                Insert.ExecuteNonQuery();
                G.ID = Convert.ToInt32(GetGameID.ExecuteScalar());
                foreach (Player P in G.Team1)
                {
                    UploadPlayer(G, P, true, G.Win, conn);
                }
                foreach (Player P in G.Team2)
                {
                    UploadPlayer(G, P, false, !G.Win, conn);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //this function is here because you need access to game variables as well as player variables
        static private void UploadPlayer(Game G, Player P, bool Team1, bool Win, SQLiteConnection conn)
        {
            if (P.ID != null)//player needs to exist
            {
                try
                {
                    string InsertPerGameStatsString = "INSERT INTO PerGameStats (PlayerID,GameID,Team1,Attacks,Hits,Damage,Kills,LastHits," +
                                                      "Assists,Spells,SpellDamage, Deaths, Escapes,Leave) " +
                                                      "VALUES(" + P.ID + ", " + G.ID + ", " + Convert.ToInt16(Team1) + ", " + P.Attacks + ", " + P.Hits +
                                                      ", " + P.Damage + ", " + P.Kills + ", " + P.LastHits + ", " + P.Assists + ", " + P.Spells + ", " +
                                                      P.SpellDamage + ", " + P.Deaths + ", " + P.Escapes + ", " + Convert.ToInt16(P.Leave) + ")";
                    SQLiteCommand InsertPerGameStats = new SQLiteCommand(InsertPerGameStatsString, conn);
                    InsertPerGameStats.ExecuteNonQuery();

                    string UpdatePlayerString = "UPDATE PLAYER SET Attacks = Attacks +" + P.Attacks + ", Hits = Hits +" + P.Hits + ", Damage = Damage +" + P.Damage +
                                          ", Kills = Kills +" + P.Kills + ", LastHits = LastHits +" + P.LastHits + ", Assists = Assists +" + P.Assists +
                                          ", Spells = Spells +" + P.Spells + ", SpellDamage = SpellDamage+" + P.SpellDamage + ", Escapes = Escapes+" + P.Escapes +
                                          ", Deaths = Deaths +" + P.Deaths + ", Time = Time +" + G.RunningTime.TotalSeconds + ", Wins = Wins + " + Convert.ToInt16(Win)
                                          + ", Losses = Losses +" + Convert.ToInt16(!Win) + ", Leaves = Leaves +" + Convert.ToInt16(P.Leave) + " WHERE ID = " + P.ID;
                    SQLiteCommand UpdatePlayer = new SQLiteCommand(UpdatePlayerString, conn);
                    UpdatePlayer.ExecuteNonQuery();
                    foreach (int Achievement in P.Achievements)
                    {
                        SQLiteCommand InsertAchievements = new SQLiteCommand("INSERT INTO Achievements (PlayerID, AchievementID, GameID) Values("
                                                                                + P.ID + ", " + Achievement + ", " + G.ID + ")", conn);
                        InsertAchievements.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion
    }
}
