using UnityEngine;

public class TrapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] traps;

    private int index=0;

    private float timer=0.0f;
    private float spawnTime = 10.0f;
    private void Awake()
    {
        foreach (GameObject obj in traps) 
        {
            obj.SetActive(false);
        }
    }

    //10초마다 불장판 생성
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnTime) 
        {
            traps[index].SetActive(true);

            if(index < traps.Length -1)
                index++;

            timer = 0.0f;
        }

    }

}
