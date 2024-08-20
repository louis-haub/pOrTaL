using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    public bool JumpToMenu = false;
    
    private void OnTriggerEnter(Collider other)
    {
        LoadLevel();
    }
    
    private void LoadLevel()
    {
        // load the nextlevel
        if (JumpToMenu)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }

    }
}
