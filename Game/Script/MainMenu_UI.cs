using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class MainMenu_UI : MonoBehaviour
{
    public void Botton_Start()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void Button_Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
