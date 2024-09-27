using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager :MonoBehaviour
{ 
    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private TextMeshProUGUI Text;

    bool isPaused = false;

    private void Start() 
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (Player.isPlayerDead)
        {
            GameOver();
        }
        else 
        {
            panel.SetActive(false);
        }
           
    }
    public void Clear() 
    {
        panel.SetActive(true);
        Text.text = "Clear!";
    }
    public void GameOver() 
    {
        panel.SetActive(true);
        Text.text = "Game Over!";

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        OnApplicationPause(true);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");  
    }
}
