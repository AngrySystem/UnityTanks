using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class EnemyAI : Tank
    {
        public IEnumerator Think()
        {
            var whereToMove = new List<Vector2Int>()
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };

            yield return TryMove(whereToMove[UnityEngine.Random.Range(0, whereToMove.Count)]);
        }
    }
}
