using UnityEngine;

public interface IDamagable
{
    // 가상 함수
    //타격한 캐릭터, 무기 종류, 맞은 위치, 데미지 값
    void OnDamage(GameObject attacker, Weapon causer, Vector3 hitPoint, DoActionData data);
}
