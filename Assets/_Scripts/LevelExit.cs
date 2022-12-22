using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float loadTimeDelay = 0.5f;
    IEnumerator LoadNextLevel() {
        yield return new WaitForSecondsRealtime(loadTimeDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        int totalSceneCount = SceneManager.sceneCountInBuildSettings;
        if (nextSceneIndex == totalSceneCount)
        {
            nextSceneIndex = 0;
        }

        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(nextSceneIndex);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
        {
            StartCoroutine(LoadNextLevel());
        }
    }
}
