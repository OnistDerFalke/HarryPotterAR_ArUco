using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public static class TestManager
    {
        public static bool Detected;
        public static List<int> Indexes;
        public static Dictionary<int, List<Vector2>> CornersPositions;
        public static bool DetectingStarted;
        public static float Time;
        public static float MaxTime = 30f;
        public static string FilePath;
        public static string Logs;

        public static void Setup()
        {
            Detected = false;
            DetectingStarted = false;
            CornersPositions = new();
            Indexes = new();
            Time = 0;
            FilePath = Application.persistentDataPath + "/arucoData.txt";
            Debug.Log(FilePath);
            Logs += "Œcie¿ka: " + FilePath + "\n";
            File.Create(FilePath);
        }

        public static void UpdatePositions(int id, List<Vector2> positions)
        {
            CornersPositions[id] = positions;

            SaveToFile($"{Time}: Pozycje rogów znacznika {id} to [{positions[0]}; {positions[1]}; {positions[2]}; {positions[3]}]");
        }

        public static void UpdateDetected(int[] ids)
        {
            Detected = ids.Length > 0;

            foreach(var id in ids)
            {
                if (!Indexes.Contains(id))
                {
                    Debug.Log("Wykryto znacznik " + id);
                    SaveToFile($"{Time}: Wykryto znacznik {id}");
                    Indexes.Add(id);
                }   
            }

            for (int i = Indexes.Count - 1; i >= 0; i--)
            {
                var id = Indexes[i];
                if (!ids.Any(i => i == id))
                {
                    Debug.Log("Zgubiono znacznik " + id);
                    SaveToFile($"{Time}: Zgubiono znacznik {id}");
                    Indexes.RemoveAt(i);
                }
            }
        }

        private static void SaveToFile(string data)
        {
            File.AppendAllText(FilePath, data + "\n");
        }
    }
}
