using System.Collections;
using UnityEngine;
using System.IO;

public class ScreenshotCamera : MonoBehaviour {

	[Header("Timers")]
	[SerializeField] float m_changeCooldown = 3;

	[Header("Speed")]
	[SerializeField] float m_moveSpeed = 5;
	[SerializeField] float m_rotateSpeed = 5;

	[Header("Sensitivity")]
    [SerializeField] float sensitivity = 1.0f;
    [SerializeField] float smoothing = 5.0f;

	[Header("Transform")]
	[SerializeField] Transform m_rotate;

	Vector2 mouseLook;
    Vector2 smoothV;

	float m_hAxis_Button;
	float m_vAxis_Button;

	Vector3 m_moveDirection;
	public Vector3 MoveDirection{
        get{
            return m_moveDirection;
        }
        set{
            m_moveDirection = value;
        }
    }

	Vector2 md;
	bool m_freeCamActivate = false;
	bool m_canChangeCamera = true;
	Camera m_camera;
	PlayerManager m_playerManager;

	bool m_takeScreenshotOnNextFrame = false;

	void Start(){
		m_camera = GetComponentInChildren<Camera>();
		m_playerManager = PlayerManager.Instance;
	}

    void Update(){
		m_hAxis_Button = Input.GetAxis("Horizontal");
		m_vAxis_Button = Input.GetAxis("Vertical");

		md = new Vector2(Input.GetAxisRaw("CameraX"), Input.GetAxisRaw("CameraY"));

		// if(m_canChangeCamera && Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.M)){
		if(m_canChangeCamera && Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.N)){
			StartCoroutine(StartCooldown());
			m_freeCamActivate =! m_freeCamActivate;
			ActivateCamera(m_freeCamActivate);
			ChangePlayerSettings();
		}

		if(m_freeCamActivate && Input.GetKey(KeyCode.Space)){
			TakeScreenshot();
		}
	}

	void ChangePlayerSettings(){
		m_playerManager.m_playerDebugs.m_playerCanMove = !m_freeCamActivate;
		if(m_freeCamActivate){
			m_playerManager.m_sM.ChangeState(9);
		}else{
			m_playerManager.m_sM.ChangeState(0);
		}
	}

	void FixedUpdate(){
		if(!m_freeCamActivate)
			return;

		MoveDirection = new Vector3(m_hAxis_Button, 0, m_vAxis_Button);
		transform.Translate(m_moveDirection * m_moveSpeed, m_rotate);

        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;

		m_rotate.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
		transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Vector3.up);
	}

	void ActivateCamera(bool isActivate){
		m_camera.enabled = isActivate;
	}

	IEnumerator StartCooldown(){
		m_canChangeCamera = false;
		yield return new WaitForSeconds(m_changeCooldown);
		m_canChangeCamera = true;
	}

#region Screenshot

	void TakeScreenshot(){
		string currentDate = System.DateTime.Now.ToString();
		currentDate = currentDate.Replace("/", "-");
		currentDate = currentDate.Replace(":", "-");
		currentDate = currentDate.Replace(" ", "-");

		ScreenCapture.CaptureScreenshot("Screenshots/Screenshot-" + currentDate + ".png", 1);
	}
#endregion Screenshot


}

 
