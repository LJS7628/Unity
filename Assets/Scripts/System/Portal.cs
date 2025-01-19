using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private void Update()
    {

        //몬스터가 다 안 죽었다면 비활성화
        if (SceneManager.GetActiveScene().name == "EnemyScene" && GameObject.FindGameObjectWithTag("Enemy"))
        { 
            this.gameObject.SetActive(false);
        }

        this.gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        //포탈을 씬 마다 연결할 씬 설정
        if (other.tag == "Player" && SceneManager.GetActiveScene().name == "MainScene")
        {
            SceneManager.LoadScene("EnemyScene");
        }
        else if (other.tag == "Player" && SceneManager.GetActiveScene().name == "EnemyScene")
        {  
            SceneManager.LoadScene("BossScene");
        }
        else if (other.tag == "Player" && SceneManager.GetActiveScene().name == "BossScene")
            SceneManager.LoadScene("MainScene");
    }
}
