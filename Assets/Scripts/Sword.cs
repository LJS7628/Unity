using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DoActionData
{
    public float Power;
    public int StopFrame;
    public float Distance;

    public float ShakeDuration;
    public Vector3 ShakeDirection;

    public GameObject Particle;
    public Vector3 ParticleOffset;
    public Vector3 ParticleScale = Vector3.one;
}

public class Sword : MonoBehaviour
{
    [SerializeField]
    private DoActionData[] doActionDatas;
    
    private new Collider collider;
    private GameObject rootObject;

    private List<GameObject> hittedList;
    

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rootObject = transform.root.gameObject;

        hittedList = new List<GameObject>();
    }

    private void Start ()
	{
        End_Collision();
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == rootObject)
            return;

        
        foreach(GameObject obj in hittedList)
        {
            if (obj == other.gameObject)
                return;
        }

        hittedList.Add(other.gameObject);


        IDamagable damage = other.gameObject.GetComponent<IDamagable>();
        Player player = rootObject.GetComponent<Player>();

        if (player != null && damage != null)
        {
            DoActionData data = doActionDatas[player.ComboIndex];

            CinemachineImpulseSource source = GetComponent<CinemachineImpulseSource>();
            if(source != null && data.ShakeDuration > 0.0f && data.ShakeDirection.magnitude > 0.0f)
            {
                source.m_ImpulseDefinition.m_ImpulseDuration = data.ShakeDuration;
                source.m_DefaultVelocity = data.ShakeDirection;

                source.GenerateImpulse();
            }


            Vector3 hitPoint = collider.ClosestPoint(other.transform.position); //World
            hitPoint = other.transform.InverseTransformPoint(hitPoint);

            damage.Damage(rootObject.gameObject, this, hitPoint, data);
        }
            
    }

    public void Begin_Collision()
    {
        collider.enabled = true;
    }

    public void End_Collision()
    {
        collider.enabled = false;

        hittedList.Clear();
    }
}