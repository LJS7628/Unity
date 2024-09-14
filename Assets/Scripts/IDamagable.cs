using UnityEngine;

public interface IDamagable
{
	void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, DoActionData data);
}