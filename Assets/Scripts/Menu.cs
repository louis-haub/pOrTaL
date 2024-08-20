using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string firstScene;
    
    public void Play()
    {
        SceneManager.LoadScene(firstScene);
    }

    public void Options()
    {
        
    }

    public void Credits()
    {
        
    }

    public void Exit()
    {
        Application.Quit();
    }
}
