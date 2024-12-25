using UnityEngine;

public class Exit : MonoBehaviour
{
    public void ExitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Exit");
#else
        Application.Quit();
#endif
    }
}
