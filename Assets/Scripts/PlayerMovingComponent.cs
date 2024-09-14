using UnityEngine;

public class PlayerMovingComponent : MonoBehaviour
{
	[SerializeField]
	private float walkSpeed = 2;

	[SerializeField]
	private float runSpeed = 4;

	[SerializeField]
	private Vector2 limitPitchAngle = new Vector2(20, 340);

	[SerializeField]
	private float mouseSpeed = 0.25f;

	[SerializeField]
	private string followTargetName = "FollowTarget"; 

	private Animator animator;

	private bool bCanMove = true;
	private Vector3 direction;

	private Transform followTargetTransform;

    public void Move()
    {
		bCanMove = true;
    }

	public void Stop()
	{
		bCanMove = false;
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start ()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		followTargetTransform = transform.FindChildByName(followTargetName);
	}


	private Quaternion rotation;

	private void Update()
    {
		float x = Input.GetAxis("Mouse X");
		float y = Input.GetAxis("Mouse Y");

		rotation *= Quaternion.AngleAxis(x, Vector3.up);
		rotation *= Quaternion.AngleAxis(-y, Vector3.right);
		followTargetTransform.rotation = rotation;

		Vector3 angle = followTargetTransform.localEulerAngles;
		angle.z = 0.0f;


        float xAngle = followTargetTransform.localEulerAngles.x;

        if (xAngle < 180.0f && xAngle > limitPitchAngle.x)
            angle.x = limitPitchAngle.x;
        else if (xAngle > 180.0f && xAngle < limitPitchAngle.y)
            angle.x = limitPitchAngle.y;

        followTargetTransform.localEulerAngles = angle;


		rotation = Quaternion.Lerp(followTargetTransform.rotation, rotation, mouseSpeed * Time.deltaTime);

		transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
		followTargetTransform.localEulerAngles = new Vector3(angle.x, 0, 0);


		if (bCanMove == false)
			return;


		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		bool bRun = Input.GetButton("Run");

		float speed = bRun ? runSpeed : walkSpeed;


		direction = (Vector3.forward * vertical) + (Vector3.right * horizontal);
		direction = direction.normalized * speed;

		transform.Translate(direction * Time.deltaTime);

		animator.SetFloat("SpeedX", horizontal * speed);
		animator.SetFloat("SpeedY", vertical * speed);
		animator.SetFloat("SpeedZ", direction.magnitude);
	}

    private void OnGUI()
    {
		GUI.color = Color.red;
		GUILayout.Label(followTargetTransform.localEulerAngles.ToString("F6"));

	}
}