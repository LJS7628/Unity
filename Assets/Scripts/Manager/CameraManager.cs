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

    //���� �ó׸�ƽ �� �Կ�
    private IEnumerator PlayingCamera()
    {
        // 1�� ī�޶��� ����ġ�� 2.0 -> 0.4�� ����
        yield return StartCoroutine(LerpCameraWeight(2.0f, 0.4f, duration));

        // ��� ��� 
        yield return new WaitForSeconds(1.0f);

        // 1�� ī�޶��� ����ġ�� 0.4 -> 2.0���� ����
        yield return StartCoroutine(LerpCameraWeight(0.4f, 2.0f, duration));

        PlayerCam.SetActive(true);
        MixingCam.SetActive(false);
    }

    //ī�޶� ����ġ�� ������������ ó��
    private IEnumerator LerpCameraWeight(float from, float to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // ���� �����Ͽ� ����ġ ����
            float currentWeight = Mathf.Lerp(from, to, t);
            MixingRealCam.m_Weight0 = currentWeight;

            yield return null; // �� ������ ���
        }

    }

}
