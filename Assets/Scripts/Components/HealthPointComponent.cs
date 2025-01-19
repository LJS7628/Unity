using UnityEngine;
using UnityEngine.UI;

public class HealthPointComponent : MonoBehaviour
{
    [SerializeField]
    private float maxHealthPoint = 100;
    private float currentHealthPoint;

    [Header(" - Enemy")]
    [SerializeField]
    private string uiEnemyResourceName = "Enemy_HealthBar";

    [Header(" - Boss")]
    [SerializeField]
    private string uiBossResourceName = "Boss_HealthBar";

    [Header(" - Player")]
    [SerializeField]
    private string uiPlayerObjectName = "UI_Player";
    public bool Dead => currentHealthPoint <= 0.0f;

    private Canvas uiEnemyCanvas;
    private Image uiImage;

    //UI 세팅
    private void Start()
    {
        currentHealthPoint = maxHealthPoint;
        if (GetComponent<Enemy>() != null)
        {
            GameObject prefab = Resources.Load<GameObject>(uiEnemyResourceName);
            GameObject obj = GameObject.Instantiate<GameObject>(prefab, transform);

            uiEnemyCanvas = obj.GetComponent<Canvas>();
            uiEnemyCanvas.worldCamera = Camera.main;

            uiImage = uiEnemyCanvas.transform.FindChildByName("HP_line").GetComponent<Image>();

        }

        if (GetComponent<Boss>() != null)
        {
            GameObject prefab = Resources.Load<GameObject>(uiBossResourceName);
            GameObject obj = GameObject.Instantiate<GameObject>(prefab, transform);

            uiEnemyCanvas = obj.GetComponent<Canvas>();
            uiEnemyCanvas.worldCamera = Camera.main;

            uiImage = uiEnemyCanvas.transform.FindChildByName("HP_line").GetComponent<Image>();

        }
    }

    //데미지 처리
    public void Damage(float amount) 
    {
        if(amount <1.0f)
            return;

        currentHealthPoint += (amount * -1.0f);
        currentHealthPoint = Mathf.Clamp(currentHealthPoint, 0.0f, maxHealthPoint);

        if(uiImage != null) 
            uiImage.fillAmount = currentHealthPoint/maxHealthPoint;
    }

    //데미지 처리 및 UI 갱신
    private void Update()
    {
        if(uiEnemyCanvas != null)
            uiEnemyCanvas.transform.rotation = Camera.main.transform.rotation;

        if (GetComponent<Player>() != null)
        {
            GameObject ui = GameObject.Find(uiPlayerObjectName);
            Debug.Assert(ui != null);

            uiImage = ui.transform.FindChildByName("HP_Line").GetComponent<Image>();
            Debug.Assert(uiImage != null);

            uiImage.fillAmount = currentHealthPoint / maxHealthPoint;
        }
    }
}
