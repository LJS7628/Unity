using UnityEngine;

public class HealthPointComponent : MonoBehaviour
{
	[SerializeField]
	private float maxHealthPoint = 100;

	private float healthPoint;

	public bool IsDead { get => healthPoint <= 0.0f; }

	private void Start ()
	{
		healthPoint = maxHealthPoint;
	}

	public void Damage(float amount)
    {
		healthPoint += (amount * -1.0f);
		healthPoint = Mathf.Clamp(healthPoint, 0.0f, maxHealthPoint);
	}
}