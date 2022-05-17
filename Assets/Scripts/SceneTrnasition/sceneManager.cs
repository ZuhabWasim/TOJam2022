using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager: MonoBehaviour
{
  
    public GameObject Player;

    
    public static int sceneIndex;
    private void Start()
    {
       // sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    private void Update()
    {
        Debug.Log(sceneIndex);
     
 

    }
    public static IEnumerator loadingScene(int index)
    {
        sceneIndex = index;
        //fadeBlack;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(index);
        yield return new WaitForSeconds(1);
        //fade white;
        //sceneIndex++;
    }



}
