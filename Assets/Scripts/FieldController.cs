using Assets.Scripts.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public GroundChessBoard fielBbedrockVoxel;

    public BreakableObject bedrockVoxel;
    public BreakableObject breakableVoxelPrefab;
    public BreakableObject flagPrefab;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Camera mainCamera;
    private RestartMenu menu;

    public float playerSpeed = 3;
    public float enemySpeed = 2;
    public int dieCounter = 0;

    private Cell[,] cells;
    private int width;
    private int height;
    private Player player;
    private List<Vector2Int> enemyPositions = new List<Vector2Int>();

    private string[] map = new[] {
        "....*F*....",
        ".*..***P.*.",
        "...........",
        "*###***....",
        "...........",
        "....*.*..#.",
        "*##.*.#..#.",
        ".........#.",
        ".....**....",
        "..**....#..",
        "E.**....#.E"
    };

    // Start is called before the first frame update
    void Start()
    {
        height = map.Length;
        width = map[0].Length;

        menu = FindObjectOfType<RestartMenu>();

        cells = new Cell[width + 2, height + 2];

        for (int i = 0; i < height; i++)
        {
            if (map[i].Length != width) throw new System.Exception("Invalid map");

            for (int j = 0; j < width; j++)
            {
                // cells[j + 1, i + 1] = new Cell(map[i][j] == '#' ? CellSpace.Bedrock : CellSpace.Empty);

                if (map[i][j] == '#')
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Bedrock);
                } 
                else if (map[i][j] == '*') 
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Breakable);
                }
                else if (map[i][j] == 'F')
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Flag);
                }
                else
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Empty);
                }

                if (map[i][j] == 'P')
                {
                    var playerGO = Instantiate(playerPrefab, new Vector3(j + 1, 1, i + 1), Quaternion.identity, transform);
                    player = playerGO.GetComponent<Player>();
                    player.Initialize(playerSpeed, cells, this);
                    cells[j + 1, i + 1].Occupy(player);
                }

                if (map[i][j] == 'E')
                {
                    enemyPositions.Add(new Vector2Int(j + 1, i + 1));
                }
            }
        }
        
        for (int i = 0; i < width + 2; i++)
        {
            cells[i, 0] = new Cell(CellSpace.Bedrock);
            cells[i, height + 1] = new Cell(CellSpace.Bedrock);
        }

        for (int i = 0; i < height + 2; i++)
        {
            cells[0, i] = new Cell(CellSpace.Bedrock);
            cells[width + 1, i] = new Cell(CellSpace.Bedrock);
        }
        
        for (int x = 0; x < width + 2; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                
                var c = Instantiate(fielBbedrockVoxel, new Vector3(x, 0, y), Quaternion.identity, transform);
                c.SetColor((x + y) % 2 == 0);
                
                if (cells[x, y].Space == CellSpace.Bedrock)
                {
                    cells[x, y].breakableObject = Instantiate(bedrockVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                }
                if (cells[x, y].Space == CellSpace.Breakable)
                {
                    cells[x, y].breakableObject = Instantiate(breakableVoxelPrefab, new Vector3(x, 1, y), Quaternion.identity, transform);
                }
                if (cells[x, y].Space == CellSpace.Flag)
                {
                    cells[x, y].breakableObject = Instantiate(flagPrefab, new Vector3(x, 1, y), Quaternion.identity, transform);
                }
                
            }
        }

        mainCamera.transform.position = new Vector3((width + 2) / 2, 15, (height + 2) / 2);
        mainCamera.transform.eulerAngles = new Vector3(90, 0, 0);
        mainCamera.orthographicSize = height/1.7f;

        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2Int current = enemyPositions[UnityEngine.Random.Range(0, enemyPositions.Count)];

            if (cells[current.x, current.y].Occupant != null)
            {
                i--;
                yield return new WaitForSeconds(1);
            }
            else
            {
                var enemyGO = Instantiate(enemyPrefab, new Vector3(current.x, 1, current.y), Quaternion.identity, transform);
                var e = enemyGO.GetComponent<EnemyAI>();
                e.Initialize(enemySpeed, cells, this);
                cells[current.x, current.y].Occupy(e);
                yield return new WaitForSeconds(5);
            }
        }
    }

    public void ToEndGame()
    {
        menu.ShowMenu(true);
    }

    public void TanksDieCounter()
    {
        dieCounter++;
        if (dieCounter == 5)
        {
            menu.ShowMenu(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StartCoroutine(player.TryMove(Vector2Int.right));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StartCoroutine(player.TryMove(Vector2Int.left));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(player.TryMove(Vector2Int.up));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(player.TryMove(Vector2Int.down));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Fire();
        }

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (cells[x, y].Occupant is EnemyAI enemy)
                {
                    enemy.StartCoroutine(enemy.Think());
                }
            }
        }
    }
}
