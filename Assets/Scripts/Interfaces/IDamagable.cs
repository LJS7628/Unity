using UnityEngine;

public interface IDamagable
{
    // ���� �Լ�
    //Ÿ���� ĳ����, ���� ����, ���� ��ġ, ������ ��
    void OnDamage(GameObject attacker, Weapon causer, Vector3 hitPoint, DoActionData data);
}
