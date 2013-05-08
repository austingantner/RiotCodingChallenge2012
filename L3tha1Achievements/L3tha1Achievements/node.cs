using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L3tha1Achievements
{
    public class node
    {
        public node Parent;
        public node Left = null;
        public node Right = null;
        public int Depth = 0;
        public int ID = 0;
        public int Team1ID = 0;
        public int Team2ID = 0;
        public Game TournamentGame;

        public node(node parent)
        {
            Parent = parent;
            if (parent != null)
            {
                Depth = 1 + Parent.Depth;
            }
        }

        public void Print(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\---");
                indent += "  ";
            }
            else
            {
                Console.Write("|---");
                indent += "| ";
            }
            Console.WriteLine("Game ID: " + ID + " Teams: " + Team1ID + " vs. " + Team2ID);
            if (Left != null)
            {
                Left.Print(indent, false);
                Right.Print(indent, true);
            }
        }

    }
}
