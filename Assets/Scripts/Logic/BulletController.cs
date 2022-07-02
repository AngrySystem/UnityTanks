using Assets.Scripts.Logic;
using UnityEngine;

internal class BulletController : MonoBehaviour
{
    private Vector2Int direction;
    public float speed = 10;

    private Cell[,] cells;

    public void Initialize(Cell[,] cells)
    {
        this.cells = cells;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;

        var ourCell = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

        if (cells[ourCell.x, ourCell.y].Space != CellSpace.Empty)
        {
            if (cells[ourCell.x, ourCell.y].TryToDel())
            {
                cells[ourCell.x, ourCell.y].breakableObject.DeleteObject();
            }

            Destroy(gameObject);
        }
        
        if (cells[ourCell.x, ourCell.y].Occupant != null)
        {
            var enemy = cells[ourCell.x, ourCell.y].Occupant.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.Die();
                Destroy(gameObject);
            }   
        }
    }

    public void Fire(Vector2Int direction)
    {
        this.direction = direction;
    }
}