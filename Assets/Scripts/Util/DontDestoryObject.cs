using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryObject : MonoBehaviour
{
    private void Awake()
    {
        var obj = FindObjectsOfType<DontDestoryObject>();

        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject); //찾은 객체가 1개라면 DontDestory씬에 등록 (중복 방지)
        }
        else
        {
            Destroy(gameObject); //씬을 이동할 때마다 객체가 쌓이는 것 방지 (객체 삭제)
        }

    }
}
