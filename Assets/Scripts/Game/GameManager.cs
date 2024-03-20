using System.Collections.Generic;
using System.Linq;
using Game;

namespace Assets.Scripts
{
    public static class GameManager
    {
        public static BoardManager BoardManager = new();
        public static int CurrentDiceThrownNumber;
        public static int PlayerNumber;
        public static List<Player> Players;
        public static List<string> DebugLogs;
        //TODO: change to ArUco
        //public static Dictionary<string, VuMarkBehaviour> CurrentTrackedObjects;
        public static bool setup = false;

        public static void Setup()
        {
            BoardManager.Setup();
            Players = new();
            DebugLogs = new();
            CurrentDiceThrownNumber = -1;
            PlayerNumber = 0;
            //TODO: change to ArUco
            //CurrentTrackedObjects = new();
            setup = true;
        }

        public static Player GetMyPlayer()
        {
            return Players.Where(player => player.IsMyPlayer).FirstOrDefault();
        }

        public static void DebugLog(string log)
        {
            DebugLogs.Add(log);
        }
    }
}
