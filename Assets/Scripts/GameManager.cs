using UnityEngine;
using TMPro;

public class GameManager :MonoBehaviour
{ 

    [SerializeField]
    private TextMeshProUGUI Text;

    [SerializeField]
    private GameObject enemy;
    private void Start()
    {
        Text.text = "";
    }
    private void Update()
    {
        if (Player.isPlayerDead)
            GameOver();

        else if (enemy == null)
            Clear();
    }
    public void Clear() 
    {
        Text.text = "Clear!";

    }
    public void GameOver() 
    {
        Text.text = "Game Over!";
    }


}
