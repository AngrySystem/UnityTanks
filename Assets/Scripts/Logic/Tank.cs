using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class Tank : MonoBehaviour
    {
        public bool isMoving { get; private set; }

        public BulletController bulletPrefab;
        public GameObject tankExplosionPrefab;
        public FieldController fieldController;

        private float moveSpeed;
        private Cell[,] cells;

        private Vector2Int direction = Vector2Int.up;

        public void Initialize(float moveSpeed, Cell[,] cells, FieldController fieldController)
        {
            this.moveSpeed = moveSpeed;
            this.cells = cells;
            this.fieldController = fieldController;
        }

        public IEnumerator TryMove(Vector2Int delta)
        {
            if (isMoving)
            {
                yield break;
            }

            isMoving = true;
            direction = delta;

            var rotationY = Vector2.SignedAngle(Vector2.up, delta * new Vector2Int(-1, 1));
            rotationY = rotationY < 0 ? rotationY + 360 : rotationY;

            var from = GetCoords();
            var tc = from + delta;

            var targetCell = cells[tc.x, tc.y];
            if (targetCell.Occupant == null && targetCell.Space == CellSpace.Empty)
            {
                cells[tc.x, tc.y].Occupy(this);
                cells[from.x, from.y].Occupy(null);
                var currentPosition = new Vector3(from.x, 1, from.y);
                var targetPosition = new Vector3(tc.x, 1, tc.y);

                var currentRotation = gameObject.transform.eulerAngles;

                bool temp = currentRotation[1] == 270f && rotationY == 0;
                bool temp2 = ((currentRotation[1] >= 0f && currentRotation[1] < 0.1f) || currentRotation[1] == 360f) && rotationY == 270;

                var targetRotation = new Vector3(0, temp == true ? 360 : (temp2 == true ? -90 : rotationY), 0);

                //var targetRotation = new Vector3(0, temp == true ? 360 : rotationY, 0);
                Debug.Log(temp2);
                Debug.Log(currentRotation);
                Debug.Log(targetRotation);
                var moveTime = 1f / moveSpeed;
                float t = 0;
                while (t < moveTime)
                {
                    t += Time.deltaTime;
                    gameObject.transform.position = currentPosition + (t / moveTime) * (targetPosition - currentPosition);

                    var f = Mathf.Min(1, 2 * t / moveTime); // вращение в 2 раза бестрее скорости перемещения
                    gameObject.transform.eulerAngles = currentRotation + f * (targetRotation - currentRotation);
                    yield return null;
                }

                gameObject.transform.position = targetPosition;
                gameObject.transform.eulerAngles = temp2 == true ? new Vector3(0, 270, 0) : targetRotation;
            }
            else
            {
                var currentRotation = gameObject.transform.eulerAngles;
                bool temp = currentRotation[1] == 270f && rotationY == 0;
                bool temp2 = ((currentRotation[1] >= 0f && currentRotation[1] < 0.1f) || currentRotation[1] == 360f) && rotationY == 270;

                var targetRotation = new Vector3(0, temp == true ? 360 : (temp2 == true ? -90 : rotationY), 0);
                Debug.Log(temp2);
                Debug.Log(currentRotation);
                Debug.Log(targetRotation);
                var moveTime = 1f / moveSpeed;
                float t = 0;
                while (t < moveTime)
                {
                    t += Time.deltaTime;

                    var f = Mathf.Min(1, 2 * t / moveTime); // вращение в 2 раза бестрее скорости перемещения
                    gameObject.transform.eulerAngles = currentRotation + f * (targetRotation - currentRotation);
                    yield return null;
                }

                gameObject.transform.eulerAngles = temp2 == true ? new Vector3(0, 270, 0) : targetRotation;
            }

            isMoving = false;
        }

        private Vector2Int GetCoords()
        {
            Vector2Int p = default;
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].Occupant == this)
                    {
                        p = new Vector2Int(x, y);
                    }
                }
            }
            return p;
        }

        public void Fire()
        {
            if (!isMoving)
            {
                var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                bullet.Initialize(cells);
                bullet.Fire(direction);
            }
        }

        public void Die()
        {
            fieldController.TanksDieCounter();
            StopAllCoroutines();
            var p = GetCoords();
            cells[p.x, p.y].Occupy(null);
            GameObject explosion = Instantiate(tankExplosionPrefab, transform.position, tankExplosionPrefab.transform.rotation);
            Destroy(gameObject);
        }
    }
}