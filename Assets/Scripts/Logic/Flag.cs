using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class Flag : BreakableObject
    {
        private FieldController fieldController;
        public GameObject flagExplosionPrefab;

        void Start()
        {
            fieldController = FindObjectOfType<FieldController>();
        }
        public override void DeleteObject()
        {
            fieldController.ToEndGame();
            GameObject explosion = Instantiate(flagExplosionPrefab, transform.position, flagExplosionPrefab.transform.rotation);
            Destroy(gameObject);
        }
    }
}