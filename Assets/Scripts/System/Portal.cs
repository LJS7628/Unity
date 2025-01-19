using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private void Update()
    {

        //���Ͱ� �� �� �׾��ٸ� ��Ȱ��ȭ
        if (SceneManager.GetActiveScene().name == "EnemyScene" && GameObject.FindGameObjectWithTag("Enemy"))
        { 
            this.gameObject.SetActive(false);
        }

        this.gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        //��Ż�� �� ���� ������ �� ����
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
