using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelaySceneChanger : MonoBehaviour {

private float delay = 15f;

    void Start()
    {
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");  
    }
}