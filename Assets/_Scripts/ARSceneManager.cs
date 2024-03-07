using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class ARSceneManager : MonoBehaviour
{
    private ARSession arSession_;

    private void Awake()
    {
        arSession_ = FindObjectOfType<ARSession>();
    }

    public void RestartScene()
    {
        // Stop the AR session
        arSession_.Reset();

        // Unload the current scene
        // SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);

        // Load the current scene again
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}