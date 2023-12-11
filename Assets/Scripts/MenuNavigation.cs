using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
    
    // Start is called before the first frame update
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MoveIntoGUI()
    {
        
    }

    public void MoveOutOfGUI()
    {
        Debug.Log("4 Hours");
    }

}
