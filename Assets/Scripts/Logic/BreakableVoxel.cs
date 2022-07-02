using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class BreakableVoxel : BreakableObject
    {
        public GameObject voxelExplosionPrefab;
        public override void DeleteObject()
        {
            GameObject explosion = Instantiate(voxelExplosionPrefab, transform.position, voxelExplosionPrefab.transform.rotation);
            Destroy(gameObject);
        }
    }
}