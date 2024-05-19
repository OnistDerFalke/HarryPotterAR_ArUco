using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GameManager
    {
        public static BoardManager BoardManager = new();
        public static int CurrentDiceThrownNumber;
        public static int PlayerNumber;
        public static int ChosenIndex;
        public static List<Player> Players;
        public static List<string> DebugLogs;
        //TODO2: change to ArUco
        public static Dictionary<int, Vector3> CurrentTrackedObjects;
        public static bool setup = false;

        public static void Setup()
        {
            if (!setup)
            {
                BoardManager.Setup();
                Players = new();
                DebugLogs = new();
                CurrentDiceThrownNumber = -1;
                PlayerNumber = 0;
                //TODO2: change to ArUco
                CurrentTrackedObjects = new();
                setup = true;
            }
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
