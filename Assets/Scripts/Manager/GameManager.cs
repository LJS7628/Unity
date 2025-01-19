using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameObject Player;
    private GameObject Boss;

    private GameObject Ending;

    private GameObject CameraTarget;

    [SerializeField]
    private GameObject Camera;
    private CinemachineVirtualCamera VirtualCamera;
    private void Awake()
    {
        VirtualCamera =Camera.GetComponent<CinemachineVirtualCamera>();

        Player = SetObject("Player");

        CameraTarget = SetObject("CameraTarget");

        //�� ������ �÷��̾� ��ġ ����, ������ ��� ���� ���� ����
        if(SceneManager.GetActiveScene().name == "MainScene")
            Player.transform.position = new Vector3(40,0,20); 

        if(SceneManager.GetActiveScene().name == "EnemyScene")
            Player.transform.position = new Vector3(40, 0, 20);

        if (SceneManager.GetActiveScene().name == "BossScene") 
        {
            Player.transform.position = new Vector3(35, 0, 20);
            Boss = SetObject("Boss");
            Ending = GameObject.Find("Ending");
            Ending.SetActive(false);
        }


        VirtualCamera.Follow = CameraTarget.transform; //�÷��̾�� ī�޶� ����
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "BossScene" && Boss == null)
        {
            Ending.SetActive(true);
        }

    }


    //������Ʈ ���� �� ����
    private GameObject SetObject(string name) 
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }

}
