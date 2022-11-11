using UnityEngine;

public class DasherPlayerCamera : MonoBehaviour
{
    const string MOUSE_X = "Mouse X";
    const string MOUSE_Y = "Mouse Y";
    
    [SerializeField] float distance;
    [SerializeField] float sensitivity;

    [SerializeField] float startAngle;
    [SerializeField] float maxAngle;
    [SerializeField] float minAngle;

    private bool isActive;
    private Transform player;
    private Vector3 startDirection;
    private float rotateX;
    private float rotateY;
    private bool updatePosition;

    public void Activate(Transform player)
    {
        this.player = player;

        startDirection = -player.transform.forward;
        isActive = true;
        UpdateCamera(startAngle, 0f);
    }

    public void Deactivate()
    {
        player = null;
        startDirection = default;
        isActive = false;
    }

    public float GetDirectionAngle()
        => transform.eulerAngles.y;

    public void UpdatePosition()
        => updatePosition = true;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (!isActive) return;

        var mouseX = Input.GetAxis(MOUSE_X);
        var mouseY = Input.GetAxis(MOUSE_Y);

        if (mouseX != 0f || mouseY != 0f)
        {
            //no inversion: mouse up - look up, mouse down - look down, mouse left - look left, mouse right - look right
            rotateY += mouseX * sensitivity;
            rotateX += -mouseY * sensitivity;

            rotateY = ClampAngle(rotateY);
            rotateX = Mathf.Clamp(rotateX, minAngle, maxAngle);
            updatePosition = true;
        }
    }

    void LateUpdate()
    {
        if (updatePosition)
        {
            UpdateCamera(rotateX, rotateY);
            updatePosition = false;
        }
    }

    private void UpdateCamera(float x, float y)
    {
        var direction = startDirection * distance;
        var rotation = Quaternion.Euler(x, y, 0);

        transform.position = player.position + rotation * direction;
        transform.LookAt(player);
    }

    private float ClampAngle(float angle)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;

        return angle;
    }
}
