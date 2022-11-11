using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class FakePlayer : MonoBehaviour
    {
        const string TAG_PLAYER = "Player";

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Debug.Log("Someth hit!");

            if (hit.collider.CompareTag(TAG_PLAYER))
            {
                Debug.Log("Hit " + gameObject.name);
            }
        }

        private bool isImmortal;

        public bool IsImmortal => isImmortal;

        public void TakeDamage()
        {
            isImmortal = true;
            StartCoroutine(EndImmortality());
        }

        private IEnumerator EndImmortality()
        {
            yield return new WaitForSeconds(1);

            isImmortal = false;
        }
    }
}
