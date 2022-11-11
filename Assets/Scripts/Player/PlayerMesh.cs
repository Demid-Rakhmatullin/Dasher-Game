using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PlayerMesh : MonoBehaviour
    {
        [SerializeField] Material normalMaterial;
        [SerializeField] Material damagedMaterial;

        private MeshRenderer meshRenderer;

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            SetNormalAppearance();          
        }

        public void SetNormalAppearance()
        {
            meshRenderer.material = normalMaterial;
        }

        public void SetDamagedAppearance()
        {
            meshRenderer.material = damagedMaterial;
        }
    }
}
