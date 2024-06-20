using Assets.Scripts;
using UnityEngine;

public class BoardInitializer : MonoBehaviour
{
    private void Awake()
    {
        if (!GameManager.setup)
        {
            GameManager.Setup();
        }
    }
}
