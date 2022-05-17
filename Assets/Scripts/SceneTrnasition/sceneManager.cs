using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager: MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(loadingScene("Boss Area"));
    }


    IEnumerator loadingScene(string index)
    {
        SceneManager.LoadScene(index);
        yield return new WaitForSeconds(1);
        //fade white;
    }
 




}
