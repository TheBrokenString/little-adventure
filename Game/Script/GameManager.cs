using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public  Character playerCharacter;
    private bool gameIsOver=false;
    public GameUI_Manager gameUI_Manager;
    public View view;
    private void Awake()
    {
        
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();//获取玩家控件属性
        
    }

    private void GameOver()
    {
        gameUI_Manager.ShowGameOverUI();
        //Debug.Log("Game Over");
    }

    public void GameIsFinished()
    {
        gameUI_Manager.ShowGameIsFinishedUI();
        //Debug.Log("Game Is Finished");
    }

    void Update()
    {
        if(gameIsOver)
        {
            return;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameUI_Manager.TogglePauseUI();
        }
        if(playerCharacter.CurrentState==Character.CharacterState.Dead)
        {
            gameIsOver = true;
            GameOver();
        }
    }

    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void Restart()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

}
