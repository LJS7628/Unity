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
            DontDestroyOnLoad(gameObject); //ã�� ��ü�� 1����� DontDestory���� ��� (�ߺ� ����)
        }
        else
        {
            Destroy(gameObject); //���� �̵��� ������ ��ü�� ���̴� �� ���� (��ü ����)
        }

    }
}
