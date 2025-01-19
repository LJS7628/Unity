using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerCam;

    [SerializeField]
    public GameObject MixingCam;

    private CinemachineMixingCamera MixingRealCam;

    public float duration = 2.0f;

    private void Awake()
    {
        MixingRealCam = MixingCam.GetComponent<CinemachineMixingCamera>();
        PlayerCam.SetActive(false);
    }

    private void Start()
    {
        if (MixingCam == null)
            return;
        StartCoroutine(PlayingCamera());
    }

    //보스 시네마틱 씬 촬영
    private IEnumerator PlayingCamera()
    {
        // 1번 카메라의 가중치를 2.0 -> 0.4로 보간
        yield return StartCoroutine(LerpCameraWeight(2.0f, 0.4f, duration));

        // 잠시 대기 
        yield return new WaitForSeconds(1.0f);

        // 1번 카메라의 가중치를 0.4 -> 2.0으로 보간
        yield return StartCoroutine(LerpCameraWeight(0.4f, 2.0f, duration));

        PlayerCam.SetActive(true);
        MixingCam.SetActive(false);
    }

    //카메라 가중치를 선형보간으로 처리
    private IEnumerator LerpCameraWeight(float from, float to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // 선형 보간하여 가중치 설정
            float currentWeight = Mathf.Lerp(from, to, t);
            MixingRealCam.m_Weight0 = currentWeight;

            yield return null; // 한 프레임 대기
        }

    }

}
