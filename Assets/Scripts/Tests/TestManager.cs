using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public static class TestManager
    {
        public static Dictionary<int, List<Vector2>> CurrentTrackedObjects;
        public static bool DetectingStarted;
        public static float Time;
        public static float MaxTime = 10f;
        public static int FrameCount = 0;
        public static string Logs;
        public static string FolderName = "positions";
        private static string FilePath;

        public static void Setup()
        {
            DetectingStarted = false;
            CurrentTrackedObjects = new();
            Time = 0;
            FilePath = Application.persistentDataPath + $"/{FolderName}";
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);
        }

        public static void SetFilePath(string fileName)
        {
            FilePath = Application.persistentDataPath + $"/{FolderName}/{fileName}.txt";
            File.Create(FilePath);
        }

        public static void UpdateDetected(Dictionary<int, List<Vector2>> markers)
        {
            var i = 0;

            foreach(var m in markers)
            {
                if (!CurrentTrackedObjects.ContainsKey(m.Key))
                {
                    CurrentTrackedObjects.Add(m.Key, m.Value);
                    SaveToFile($"{Time}: Wykryto znacznik {m.Key} \t Obecnie wykrytych: {CurrentTrackedObjects.Count()}");
                    Debug.Log("Wykryto znacznik " + m.Key + $" \t Obecnie wykrytych: {CurrentTrackedObjects.Count()}");
                }
                else if (DifferentPosition(m.Value, CurrentTrackedObjects[m.Key]))
                {
                    CurrentTrackedObjects[m.Key] = m.Value;
                    SaveToFile($"{Time}: Nowe pozycje rogów znacznika {m.Key} to [{m.Value[0]}; {m.Value[1]}; {m.Value[2]}; {m.Value[3]}]");
                }
                i++;
            }

            for (i = CurrentTrackedObjects.Count - 1; i >= 0; i--)
            {
                var id = CurrentTrackedObjects.Keys.ElementAt(i);
                if (!markers.Any(i => i.Key == id))
                {
                    CurrentTrackedObjects.Remove(id);
                    SaveToFile($"{Time}: Zgubiono znacznik {id} \t Obecnie wykrytych: {CurrentTrackedObjects.Count()}");
                    Debug.Log("Zgubiono znacznik " + id + $" \t Obecnie wykrytych: {CurrentTrackedObjects.Count()}");
                }
            }
        }

        public static void SaveToFile(string data)
        {
            File.AppendAllText(FilePath, data + "\n");
        }

        private static bool DifferentPosition(List<Vector2> corners1, List<Vector2> corners2)
        {
            for (int i = 0; i < 4; i++)
                if (corners1[i].x != corners2[i].x || corners1[i].y != corners2[i].y)
                    return true;

            return false;
        }
    }
}
