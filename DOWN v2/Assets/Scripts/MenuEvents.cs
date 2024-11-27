using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEvents : MonoBehaviour
{
    public void loadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}
