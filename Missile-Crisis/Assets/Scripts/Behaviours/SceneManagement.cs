using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagementBehaviour : MonoBehaviour
{
    public void LoadScene( int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }
}
