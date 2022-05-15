using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager: MonoBehaviour
{
    [Header("PlayerPrefab")]
    public GameObject Player;

    [Header("Starting Transforms")]public static int sceneIndex;
    public Transform StartingPosition1;
    public Transform StartingPosition2;
    

    private void Update()
    {
        if(sceneIndex == 0)
        {
            return;
        }
        if(sceneIndex == 1)
        {
            StartCoroutine(loadingScene(1));
            Player.transform.position = StartingPosition1.position;
        }

        if (sceneIndex == 2)
        {
            StartCoroutine(loadingScene(1));
            Player.transform.position = StartingPosition2.position;
        }
    }

    public static IEnumerator loadingScene(int index)
    {
        sceneIndex = index;
        //fadeBlack;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(index);
        yield return new WaitForSeconds(1);
        //fade white;
  
    }





}
