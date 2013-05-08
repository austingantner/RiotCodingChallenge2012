using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace L3tha1Achievements
{
    public class Tournament
    {
        public int? ID = null;
        public node Champion = null;

        public Tournament() { }
        public Tournament(int LoadID)
        {
            Load(LoadID);
        }

        public void Print()
        {
            if (Champion == null)
            {
                Console.WriteLine("This tournament is empty");
            }
            else
            {
                Console.WriteLine("Tournament ID: "+ID+"\n");
                Console.WriteLine(Champion.TournamentGame.Win ? Champion.Team1ID : Champion.Team2ID);
                Champion.Print("", true);
            }
        }

        private void Load(int LoadID)
        {
            ID = LoadID;
            SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
            SQLiteCommand SelectCommand = new SQLiteCommand("SELECT * FROM Tournamentgames WHERE TournamentID = " + ID + " ORDER BY TournamentGameID ASC", conn);
            conn.Open();
            DataTable Results = new DataTable();
            Results.Load(SelectCommand.ExecuteReader());
            conn.Close();
            int NodeID = 0;
            Champion = new node(null);
            LoadRecursiveCall(ref NodeID, Results, Champion, Convert.ToInt32(Math.Log((Results.Rows.Count), 2)));
        }

        private void LoadRecursiveCall(ref int NodeID, DataTable Games, node current, int DepthRequired)
        {
            try
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
                if (current.Depth != DepthRequired - 1)
                {
                    current.Left = new node(current);
                    current.Right = new node(current);
                    LoadRecursiveCall(ref NodeID, Games, current.Left, DepthRequired);
                    LoadRecursiveCall(ref NodeID, Games, current.Right, DepthRequired);
                }
                current.Team1ID = Convert.ToInt32(Games.Rows[current.ID]["Team1ID"]);
                current.Team2ID = Convert.ToInt32(Games.Rows[current.ID]["Team2ID"]);
                current.TournamentGame = new Game(Convert.ToInt32(Games.Rows[current.ID]["GameID"]));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ViewGame(int TournamentGameID)
        {
            try
            {
                node Target = RecursiveSearch(Champion, ref TournamentGameID);
                if (Target != null && Target.TournamentGame != null)
                {
                    Console.WriteLine(Target.Team1ID + " vs. " + Target.Team2ID);
                    Target.TournamentGame.Print();
                }
                else
                {
                    Console.WriteLine("That does not exist");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private node RecursiveSearch(node Current,ref int TargetID)
        {
            try
            {
                if (Current.ID == TargetID)
                    return Current;
                if (Current.Left != null)
                {
                    node Temp = RecursiveSearch(Current.Left, ref TargetID);
                    if (Temp.ID == TargetID)
                        return Temp;
                }
                if (Current.Right != null)
                {
                    return RecursiveSearch(Current.Right, ref TargetID);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new node(null);
        }
    }
}
