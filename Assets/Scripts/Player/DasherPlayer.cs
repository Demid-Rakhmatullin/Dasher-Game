using Controllers;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerTransform), typeof(PlayerMesh))]
    public class DasherPlayer : NetworkBehaviour
    {
        [SerializeField] float dashDistance;
        [SerializeField] float immortalitySeconds;

        private PlayerTransform playerTransform;
        private PlayerMesh playerMesh;
        private bool isImmortal;
        private int currentScore;

        [SyncVar] private bool controlsDisactive;

        public bool IsImmortal { get => isImmortal; set => isImmortal = value; }

        public int CurrentScore => currentScore;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            playerTransform.Activate(dashDistance);
        }

        public override void OnStopLocalPlayer()
        {
            base.OnStopLocalPlayer();

            playerTransform.Deactivate();
        }

        void Awake()
        {
            playerTransform = GetComponent<PlayerTransform>();
            playerMesh = GetComponent<PlayerMesh>();

            playerTransform.OnDashingHit += OnDashedHit;
        }

        void Update()
        {
            if (!isLocalPlayer || controlsDisactive)
                return;

            playerTransform.ProcessInput();
        }

        private void OnDashedHit(object sender, ControllerColliderHit hit)
        {
            CmdProcessHit(hit.gameObject.GetComponent<DasherPlayer>());
        }
               

        [Command]
        private void CmdProcessHit(DasherPlayer otherPlayer)
        {
            if (!otherPlayer.IsImmortal)
            {
                otherPlayer.IsImmortal = true;
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

            StartCoroutine(RoutineEndImmortality());
        }

        [ClientRpc]
        private void RpcBecomeImmortal()
        {
            isImmortal = true;
            playerMesh.SetDamagedAppearance();
        }

        [ClientRpc]
        private void RpcEndImmortality()
        {
            isImmortal = false;
            playerMesh.SetNormalAppearance();
        }

        private IEnumerator RoutineEndImmortality()
        {
            yield return new WaitForSeconds(immortalitySeconds);

            RpcEndImmortality();
        }
    }
}
