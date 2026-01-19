using System.Collections.Generic;
using UnityEngine;

public class PerceptionComponent : MonoBehaviour
{
    [SerializeField]
    private float distance = 5.0f;

    [SerializeField]
    private float angle = 45.0f;

    [SerializeField]
    private float lostTime = 2.0f;

    [SerializeField]
    private LayerMask layerMask;

    private void Reset()
    {
        layerMask = 1 << LayerMask.NameToLayer("Character");
    }

    private Dictionary<GameObject, float> percievedTable;

    private void Awake()
    {
        percievedTable = new Dictionary<GameObject, float>();
    }


    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance, layerMask);

        Vector3 forward = transform.forward;

        List<Collider> candidateList = new List<Collider>();

        //1. 감시 조건에 맞는 대상자 선정

        foreach (Collider collider in colliders) 
        {
            Vector3 direction = collider.transform.position - transform.position;
            float signedAngle = Vector3.SignedAngle(forward, direction.normalized, Vector3.up);

            if(Mathf.Abs(signedAngle) < angle)
                candidateList.Add(collider);    
        }

        //2. 감시 대상 등록 및 시간 업데이트

        foreach(Collider candidate in candidateList) 
        {
            //2-1. 최초 감시
            if (percievedTable.ContainsKey(candidate.gameObject) == false) 
            {
                percievedTable.Add(candidate.gameObject, Time.realtimeSinceStartup);

                continue;
            }

            //2-2. 기존에 감시되어 있는 객체
            percievedTable[candidate.gameObject] = Time.realtimeSinceStartup;
        }

        //3. 시간 초과 대상자 선정 및 삭제
        List<GameObject> removeList = new List<GameObject>();
        foreach(var item in percievedTable) 
        {
            if((Time.realtimeSinceStartup - item.Value) >=lostTime)
                removeList.Add(item.Key);

        }

        removeList.RemoveAll(remove => percievedTable.Remove(remove));
    }

    public GameObject GetPercievedPlayer() 
    {
        foreach (var item in percievedTable)
        { 
            if(item.Key.CompareTag("Player"))
                return item.Key;
        }

        return null;
    }

    //감지 대상이 다수인 경우
    public int GetPercievedObjects(GameObject[] objs)
    {
        percievedTable.Keys.CopyTo(objs, percievedTable.Keys.Count);

        return percievedTable.Keys.Count;
    }

    //감지 대상이 1개인 경우
    public GameObject[] GetPercievedObject()
    {
        GameObject[] objs = new GameObject[percievedTable.Count];
        percievedTable.Keys.CopyTo (objs, percievedTable.Count);

        return objs;
    }

    //감지 범위 감지 대상 보여주기 용
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distance);

        Gizmos.color =Color.blue;

        Vector3 direction = Vector3.zero;
        Vector3 forward = transform.forward;

        direction = Quaternion.AngleAxis(+angle, Vector3.up) * forward;
        Gizmos.DrawLine(transform.position, transform.position+direction.normalized*distance);

        direction = Quaternion.AngleAxis(-angle, Vector3.up) * forward;
        Gizmos.DrawLine(transform.position, transform.position + direction.normalized * distance);

        GameObject player = GetPercievedPlayer();

        if (player == null)
        {
            player = GameObject.Find("Player");
            Gizmos.color = Color.blue;

            Vector3 position = transform.position;
            position.y += 1.0f;

            Vector3 playerPosition = player.transform.position;
            playerPosition.y += 1.0f;

            Gizmos.DrawLine(position, playerPosition);
            Gizmos.DrawWireSphere(playerPosition, 0.25f);

        }
        else if(player != null)
        {
            Gizmos.color = Color.red;

            Vector3 position = transform.position;
            position.y += 1.0f;

            Vector3 playerPosition = player.transform.position;
            playerPosition.y += 1.0f;

            Gizmos.DrawLine(position, playerPosition);
            Gizmos.DrawWireSphere(playerPosition, 0.25f);
        }


    }
#endif
}
