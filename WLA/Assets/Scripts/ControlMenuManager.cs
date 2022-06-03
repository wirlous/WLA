using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlMenuManager : MonoBehaviour
{
    public void GoToMainMenu(int index)
    {
        SceneManager.LoadScene(0);
    }
}
