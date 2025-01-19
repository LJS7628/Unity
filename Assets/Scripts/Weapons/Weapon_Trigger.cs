using System;
using UnityEngine;

public class Weapon_Trigger : MonoBehaviour
{
    public event Action<Collider> OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(other);
    }
}
