using Mirror;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(MeshRenderer))]
public class DasherPlayer : NetworkBehaviour
{
    const string HORIZONTAL_AXIS = "Horizontal";
    const string VERTICAL_AXIS = "Vertical";
    const string TAG_PLAYER = "Player";

    [SerializeField] Material normalMaterial;
    [SerializeField] Material damagedMaterial;
    [SerializeField] float moveSpeed;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDistance;
    [SerializeField] float turnSmoothTime;
    [SerializeField] float immortalitySeconds;

    private CharacterController controller;
    private MeshRenderer meshRenderer;
    private DasherPlayerCamera mainCamera;
    private float turnSmoothVelocity;
    private bool isDashing;
    private float currentDashDistance;
    private bool isImmortal;
    private int currentScore;

    [SyncVar] private bool controlsDisactive;

    public bool IsImmortal => isImmortal;

    public int CurrentScore => currentScore;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        mainCamera = Camera.main.GetComponent<DasherPlayerCamera>();
        mainCamera.Activate(transform);           
    }

    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();

        mainCamera.Deactivate();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        meshRenderer = GetComponent<MeshRenderer>();

        meshRenderer.material = normalMaterial;
    }

    void Update()
    {
        if (!isLocalPlayer || controlsDisactive)
            return;

        if (!isDashing)
        {
            if (Input.GetMouseButton(0))
            {
                currentDashDistance = 0f;
                isDashing = true;
            }
            else
            {
                ProcessMoveInput();
            }           
        }
        else
        {
            ProcessDash();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDashing)
        {
            if (hit.collider.CompareTag(TAG_PLAYER))
            {
                CmdProcessHit(hit.gameObject.GetComponent<DasherPlayer>());         
            }
            else
            {
                isDashing = false;
            }
        }
    }
  
    private void ProcessMoveInput()
    {
        var input = new Vector3(Input.GetAxis(HORIZONTAL_AXIS), 0, Input.GetAxis(VERTICAL_AXIS));
        if (input != Vector3.zero)
        {
            input = input.normalized;
            var cameraAngle = mainCamera.GetDirectionAngle();
            Rotate(input.x, input.z, cameraAngle);
            MoveSelf(input, Quaternion.AngleAxis(cameraAngle, Vector3.up), speed: moveSpeed);
        }
    }

    private void ProcessDash()
    {
        var nextDistance = dashSpeed * Time.deltaTime;

        if (currentDashDistance + nextDistance >= dashDistance)
        {
            nextDistance = dashDistance - currentDashDistance;
            isDashing = false;
        }
        else
        {
            currentDashDistance += nextDistance;
        }

        MoveSelf(transform.forward, Quaternion.identity, distance: nextDistance);
    }

    [Command]
    private void CmdProcessHit(DasherPlayer otherPlayer)
    {
        if (!otherPlayer.IsImmortal)
        {
            TargetUpdateScore(++currentScore);
            controlsDisactive = MapController.Instance.CheckWin(this);

            otherPlayer.TakeDamage();
        }
    }

    [TargetRpc]
    private void TargetUpdateScore(int newScore)
    {
        currentScore = newScore;
        MapController.Instance.UpdateScoreCounter(this);
    }

    [Server]
    private void TakeDamage()
    {
        RpcBecomeImmortal();

        StopAllCoroutines();
        StartCoroutine(RoutineEndImmortality());
    }

    [ClientRpc]
    private void RpcBecomeImmortal()
    {
        isImmortal = true;
        meshRenderer.material = damagedMaterial;
    }

    private IEnumerator RoutineEndImmortality()
    {
        yield return new WaitForSeconds(immortalitySeconds);

        RpcEndImmortality();
    }

    [ClientRpc]
    private void RpcEndImmortality()
    {
        isImmortal = false;
        meshRenderer.material = normalMaterial;
    }

    private void Rotate(float x, float z, float cameraAngle)
    {
        var targetAngle = Mathf.Atan2(x, z) * Mathf.Rad2Deg
                + cameraAngle;
        var smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
            ref turnSmoothVelocity, turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
    }

    private CollisionFlags MoveSelf(Vector3 direction, Quaternion rotation, float speed = 0f, float distance = 0f)
    {
        var moveDir = (rotation * direction).normalized;

        var moveDistance = distance > 0f
            ? distance
            : speed * Time.deltaTime;

        var collisions = controller.Move(moveDistance * moveDir);
        mainCamera.UpdatePosition();

        return collisions;
    }
}
