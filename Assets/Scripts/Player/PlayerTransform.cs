using GameCamera;
using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerTransform : MonoBehaviour
    {
        const string HORIZONTAL_AXIS = "Horizontal";
        const string VERTICAL_AXIS = "Vertical";
        const string TAG_PLAYER = "Player";

        [SerializeField] float moveSpeed;
        [SerializeField] float dashSpeed;
        [SerializeField] float turnSmoothTime;

        private float dashDistance;
        private CharacterController controller;
        private ThirdPersonCamera playerCamera;
        private float turnSmoothVelocity;
        private bool isDashing;
        private float currentDashDistance;

        public event EventHandler<ControllerColliderHit> OnDashingHit;

        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        public void Activate(float dashDistance)
        {
            this.dashDistance = dashDistance;

            playerCamera = Camera.main.GetComponent<ThirdPersonCamera>();
            playerCamera.Activate(transform);
        }

        public void Deactivate()
        {
            playerCamera.Deactivate();
        }

        public void ProcessInput()
        {
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
                    OnDashingHit?.Invoke(this, hit);
                    //CmdProcessHit(hit.gameObject.GetComponent<DasherPlayer>());
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
                var cameraAngle = playerCamera.GetDirectionAngle();
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
            playerCamera.UpdatePosition();

            return collisions;
        }
    }
}
