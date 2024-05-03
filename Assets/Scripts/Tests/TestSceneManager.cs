using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts
{
    public class TestSceneManager : MonoBehaviour
    {
        [SerializeField] private Button mainButton;
        [SerializeField] private Text mainButtonText;
        [SerializeField] private Text detectingStatus;


        void Start()
        {
            TestManager.Setup();
        }

        void Update()
        {
            detectingStatus.text = TestManager.Detected ? "Wykryto" : "Nie wykryto";
            mainButtonText.text = TestManager.DetectingStarted ? $"Zatrzymaj wykrywanie\nCzas wykrywania: {TestManager.Time}" : "Rozpocznij wykrywanie";
            if (TestManager.DetectingStarted)
                TestManager.Time += Time.deltaTime;
            if (TestManager.Time >= TestManager.MaxTime && TestManager.DetectingStarted)
            {
                TestManager.DetectingStarted = false;
                TestManager.Time = 0;
            }    
        }

        public void OnStartDetectingClick()
        {
            TestManager.DetectingStarted = !TestManager.DetectingStarted;
            if (!TestManager.DetectingStarted)
                TestManager.Time = 0;
        }
    }
}
