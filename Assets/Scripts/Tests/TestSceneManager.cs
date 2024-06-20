using System;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class TestSceneManager : MonoBehaviour
    {
        [SerializeField] private Button MainButton;
        [SerializeField] private Text Status;
        [SerializeField] private InputField FileNameInput;

        void Start()
        {
            TestManager.Setup();
        }

        void Update()
        {
            Status.text = $"Obecnie wykrytych: {TestManager.CurrentTrackedObjects.Count}";
            MainButton.GetComponentInChildren<Text>().text = TestManager.DetectingStarted ? 
                $"Zatrzymaj wykrywanie\nCzas wykrywania: {Math.Round(TestManager.Time, 3)}" : "Rozpocznij wykrywanie";
            if (TestManager.DetectingStarted)
                TestManager.Time += Time.deltaTime;
            if (TestManager.Time >= TestManager.MaxTime && TestManager.DetectingStarted)
            {
                TestManager.DetectingStarted = false;
                TestManager.CurrentTrackedObjects = new();
                TestManager.SaveToFile($"Liczba klatek: {TestManager.FrameCount}");
                TestManager.Time = 0;
                TestManager.FrameCount = 0;
                FileNameInput.text = "";
            }

            FileNameInput.interactable = !TestManager.DetectingStarted;
            MainButton.interactable = FileNameInput.text != "" || TestManager.DetectingStarted;
        }

        public void OnMainButtonClick()
        {
            TestManager.DetectingStarted = !TestManager.DetectingStarted;
            if (!TestManager.DetectingStarted)
            {
                TestManager.Time = 0;
                TestManager.CurrentTrackedObjects = new();
                TestManager.FrameCount = 0;
                FileNameInput.text = "";
            }
            else
                TestManager.SetFilePath(FileNameInput.text);
        }
    }
}
