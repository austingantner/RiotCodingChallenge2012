using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Data.SQLite;

namespace L3tha1Achievements
{
    /// <summary>
    /// Class containing global variables such as Connection strings,
    /// </summary>
    static class gv
    {
        //I put rand as a global variable. Otherwise every function will have a random that will seed the same if run consecutively
        public static Random rand = new Random();

        private readonly static string LocationOfDBFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Remove(0,6) + "\\..\\..\\..\\..\\SQLITE\\LAData.db";
        /*If the database file can not be found by the program comment the previous line,
          Uncomment the following line and set it to the path of the dbfile.  It should be in CodingChallenge\SQLite\LAData.db */
        //private static string LocationOfDBFile = "C:\\*Where Ever The DB file is*\\CodingChallenge\\SQLite\\LAData.db";
        
        
        public readonly static string ConnectionString = "Data Source=" + LocationOfDBFile + ";Version=3;New=True;Compress=True;";

        /// <summary>
        /// Dictionary loaded from database at first use containing achievements and their ID's.
        /// </summary>
        public readonly static Dictionary<int, string> Achievements = GetAchievements();

        //load achievments at beginning of program
        private static Dictionary<int,string> GetAchievements()
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(gv.ConnectionString);
                SQLiteCommand SelectCommand = new SQLiteCommand("SELECT ID, Name FROM AchievementMaster", conn);
                conn.Open();
                DataTable Results = new DataTable();
                Results.Load(SelectCommand.ExecuteReader());
                conn.Close();
                Dictionary<int, string> temp = new Dictionary<int,string>();
                foreach (DataRow D in Results.Rows)
                {
                    temp.Add(Convert.ToInt32(D[0].ToString()), D[1].ToString());
                }
                return temp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Dictionary<int, string>();
            }
            
        }
    }
}
