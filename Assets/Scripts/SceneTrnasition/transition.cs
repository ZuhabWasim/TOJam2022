using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class transition : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(loadingScene(2));
    }
    IEnumerator loadingScene(int sceneName)
    {
        //fadeBlack;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(1);
        //fade white;
        //sceneIndex++;
    }
}
