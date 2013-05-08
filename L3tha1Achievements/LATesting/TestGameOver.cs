using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using L3tha1Achievements;
using System.Data.SQLite;


namespace LATesting
{
    /// <summary>
    /// Summary description for TestGameOver
    /// </summary>
    [TestClass]
    public class TestGameOver
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {

        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestConnection()
        {
            Program.TestConnection();
        }

        [TestMethod]
        public void RunBlankGame()
        {
            try
            {
                GameOver.Run(new Game());
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RunBlankGameWithBlankPlayers()
        {
            try
            {
                Game TestGame = new Game();
                for (int i = 0; i < 4; i++)
                {
                    TestGame.Team1.Add(new Player());
                    TestGame.Team2.Add(new Player());
                }
                GameOver.Run(TestGame);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RunRandomGame()
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    GameOver.Run(Simulate.RandomGame());
                }
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestAchievements()
        {
            try
            {
                Player Perfect = new Player();
                Perfect.ID = 0;
                Perfect.Attacks = 100;
                Perfect.Hits = 75;
                Perfect.Damage = 500;
                Perfect.SpellDamage = 500;
                Perfect.TotalWins = 199;
                Perfect.TotalLosses = 800;
                Perfect.TotalPlayTime = TimeSpan.FromHours(499);
                Perfect.Kills = 20;
                Perfect.TotalKills = 4990;
                Perfect.Assists = 20;
                Perfect.TotalAssists = 7990;
                Perfect.LastHits = 300;
                Perfect.Deaths = 5;
                Perfect.Escapes = 22;

                Player Leaver = new Player();
                Leaver.Leave = true;
                Leaver.TotalLeaves = 99;

                Game TestGame = new Game();
                TestGame.Team1.Add(new Player());
                TestGame.Team1.Add(Leaver);
                TestGame.Team2.Add(Perfect);
                TestGame.Win = false;
                TestGame.RunningTime = TimeSpan.FromHours(1);
                GameOver.Run(TestGame);
                Assert.IsTrue(TestGame.Team1[0].Achievements.Count == 0,"1");
                Assert.IsTrue(TestGame.Team1[1].Achievements[0] == 10,"2");
                Assert.IsTrue(TestGame.Team2[0].Achievements.Count == 11,"3");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
