using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    //Size of the grid
    public Vector3 size;
    public float cellSize;
    //public GameObject egoPrefab;

    Cell[,,] grid;

    
    public bool drawGrid;

    void Awake()
    {
        GenerateGrid();
    }

    void Update()
    {
    }

    private void OnDrawGizmos()
    {
        if (grid != null && drawGrid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    for (int k = 0; k < grid.GetLength(2); k++)
                    {
                        if (grid[i, j, k].transitable)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(grid[i, j, k].globalPosition, Vector3.one);
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawCube(grid[i, j, k].globalPosition, Vector3.one);
                        }
                    }

                }
            }
        }
    }

    public void GenerateGrid()
    {

        grid = new Cell[(int)size.x, (int)size.y, (int)size.z];
      


        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                for (int k = 0; k < grid.GetLength(2); k++)
                {
                    



                    //Get de world position where the cell will be created
                    Vector3 newCellPos = new Vector3(transform.position.x + i * cellSize,
                                                         transform.position.y + j * cellSize,
                                                         transform.position.z + k * cellSize);

                    //Check if exist an object in the cell
                    Collider[] colliders = Physics.OverlapSphere(newCellPos, cellSize * 0.5f);
                    bool isTransitable = true;
                    bool isBuildable = true;
                    bool isDestroyable = false;
                    bool isTrap = false;
                    bool isEmpty = false;
                    bool winPoint = false;
                    GameObject isGameobject = null;

                    for (int c = 0; c < colliders.Length; c++)
                    {

                        if (colliders[c].tag == "CUBE" || colliders[c].tag == "RAMP")
                        {
                            isTransitable = false;
                            isBuildable = false;
                            isDestroyable = true;
                            isTrap = false;
                            isGameobject = colliders[c].gameObject;
                            //Debug.Log("1");
                            // grid[i, j, k].destroyable = true;
                        }
                        else if (colliders[c].tag == "BuildableCube")
                        {
                            isTransitable = false;
                            isBuildable = false;
                            isDestroyable = true;
                            isTrap = false;
                            isGameobject = colliders[c].gameObject;
                        }
                        else if (colliders[c].tag == "Trap")
                        {
                            isTransitable = true;
                            isBuildable = false;
                            isDestroyable = false;
                            isTrap = true;
                            isGameobject = colliders[c].gameObject;
                        }
                        else if (colliders[c].tag == "Victory")
                        {
                            isTransitable = true;
                            isBuildable = false;
                            isDestroyable = false;
                            isTrap = false;
                            winPoint = true;

                        }
                        else if (colliders[c].tag == "Vacio")
                        {
                            isTransitable = true;
                            isBuildable = true;
                            isDestroyable = false;
                            isTrap = false;
                            isEmpty = true;
                            isGameobject = colliders[c].gameObject;
                        }
                    }

                    //Instantiate de cell
                    grid[i, j, k] = new Cell(isTransitable, isBuildable, isTrap, isDestroyable, winPoint, isEmpty, isGameobject, Cell.types.empty, new Vector3(i, j, k), newCellPos);

                    


                    //Instantiate a empty gameObject in the position of the cell (raycast Stuff)
                    //Not really related with the cells
                    //Instantiate(egoPrefab, newCellPos, Quaternion.identity);
                    

                }
            }
        }
    }
    public Cell[,,] ReturnGrid()
    {
        return grid;
    }

    public Cell GetCellInWorldPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - transform.position;

        int x = Mathf.FloorToInt(Mathf.Round(localPosition.x / cellSize));
        int y = Mathf.FloorToInt(Mathf.Round(localPosition.y / cellSize));
        int z = Mathf.FloorToInt(Mathf.Round(localPosition.z / cellSize));

        /*if (x < size.x-1 && x >= 0 && y < size.y-1 && y >= 0 && z < size.z-1 && z >= 0 )
        {
           
            return grid[x, y, z];
        }*/

        if (x < size.x && x >= 0 && y < size.y && y >= 0 && z < size.z && z >= 0)
        {

            return grid[x, y, z];
        }

        return null;
    }

    public List<Cell> GetNeighbours (Cell cell)
    {
        List<Cell> neighbours = new List<Cell>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    //if (Mathf.Abs(i) == Mathf.Abs(j) || Mathf.Abs(i) == Mathf.Abs(k) || Mathf.Abs(j) == Mathf.Abs(k))
                    //  continue;


                   

                       // if (!((i == 1 && k == 1) || (i == -1 && k == 1) || (i == 1 && k == -1) || (i == -1 && k == 1)))
                        


                            Cell neighbour = GetCell((int)cell.gridPosition.x + i, (int)cell.gridPosition.y + j, (int)cell.gridPosition.z + k);

                            
                                neighbours.Add(neighbour);
                            
                        
                    
                }
            }
        }
        return neighbours;
    }

    public Cell GetCell (int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 ||x > size.x || y > size.y || z > size.z  || x>9 || y > 9 || z >9 )
        {
            return null;
        }

        return grid[x, y, z];
    }

    public bool hasNeighboursOrWall(Cell cell)
    {
        List<Cell> neighbours = new List<Cell>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    Cell neighbour = GetCell((int)cell.gridPosition.x + i, (int)cell.gridPosition.y + j, (int)cell.gridPosition.z + k);

                    if(neighbour == null)
                    {
                        return true;
                    }

                    else if(neighbour.gameObject!=null)
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }
        }

        if(neighbours.Count>0)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
