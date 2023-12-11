using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class GridManagement : MonoBehaviour
{
    Vector2 worldPoint;
    RaycastHit2D hit;
    
    TileBase[] allTiles;
    string[,] coordinates;
    List<int> scorePerLevel;
    BoundsInt bounds;

    private string Angle;
    private bool ButtonPressed = false;
    public static int points = 0;
    public TMP_Text pointTrackerText;



    public Tilemap objectMapping;
    public Vector3 startRoot;
    private Vector3 currentRoot;

    public TileBase straightRoot;
    public TileBase leftToRightRoot;
    public TileBase rightToLeftRoot;
    public TileBase topLeftCornerRoot;
    public TileBase topRightCornerRoot;
    public TileBase bottomRightCornerRoot;
    public TileBase bottomLeftCornerRoot;
    public TileBase leftWallRoot;
    public TileBase rightWallRoot;
    public TileBase pointer;

   

    void Start()
    {
        //Debug.Log("The right side is an illusion.");
        Tilemap mapping = GetComponent<Tilemap>();

        bounds = mapping.cellBounds;
        allTiles = mapping.GetTilesBlock(bounds);
        currentRoot = startRoot;
        scorePerLevel = new List<int>();

        for (int n = tileMap.cellBounds.xMin; n < tileMap.cellBounds.xMax; n++)
        {
            for (int p = tileMap.cellBounds.yMin; p < tileMap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)tileMap.transform.position.y));
                Vector3 place = tileMap.CellToWorld(localPlace);
                if (tileMap.HasTile(localPlace))
                {
                    //Tile at "place"
                    
                }
                else
                {
                    //No tile at "place"
                }
            }
        }

    }

    // Update is called once per frame

    IEnumerator ButtonPressedMethod()
    {
        ButtonPressed = true;
        yield return new WaitForSeconds(1);
        ButtonPressed = false;
    }

    void ResetLevel()
    {

        points = points - 500;
        if (points < 0)
        {
            points = 0;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    void NextLevel()
    {
        currentRoot = new Vector3Int(1, 0, 0);
        scorePerLevel.Add(points);
        points = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    string CheckTile(TileBase tile, Tilemap backgroundMap, Vector3Int position)
    {
        
        
        Debug.Log("Checking tile: " + tile.name + " at " + position);
        
        if (tile.name == "StoneWallTopBottom")
        {
            Debug.Log("Hit stone wall");
            
            return "Stone";
        } else if (tile.name == "WaterDropletPoints")
        {
            points = points + 100;
            Debug.Log("Water points added!");
            objectMapping.SetTile(position, null);
        } else if (tile.name == "WaterPool")
        {
            points = points + 500;
            Debug.Log("Found water source!");

            return "Win";
        } else if (tile.name == "RockSalt")
        {
            Debug.Log("Root killed!");
            return "Reset";
        } else if (tile.name == "FlowerIncomplete")
        {
            Debug.Log("Flower found");
        }

        if (backgroundMap != null)
        {
            if (tile.name == "DefaultTile" || tile.name == "DefaultTileWall" || tile.name == "TopDefaultTileWall" || tile.name == "BottomDefaultTileWall")
            {
                Debug.Log("Root sprout at: " + position);
                if (Angle == "RIGHT")
                {
                    
                    backgroundMap.SetTile(position, leftToRightRoot);
                    
                } else if (Angle == "LEFT")
                {  
                    backgroundMap.SetTile(position, rightToLeftRoot);
                    
                } else if (Angle == "UP" || Angle == "DOWN")
                {
                    
                    backgroundMap.SetTile(position, straightRoot);
                } else
                {
                    Debug.Log("Tested out");
                }

            }
            else if (tile.name == "RightDefaultTileWall")
            {
                if (Angle == "UP" || Angle == "DOWN")    
                {
                    backgroundMap.SetTile(position, straightRoot);
                } else { 
                    backgroundMap.SetTile(position, rightWallRoot);
                }
                
                
            }
            else if (tile.name == "LeftDefaultWall")
            {
                
                if (Angle == "UP" || Angle == "DOWN")
                {
                    backgroundMap.SetTile(position, straightRoot);
                }
                else
                {
                    backgroundMap.SetTile(position, leftWallRoot);
                }
            } 
            else if (tile.name == "DefaultTileRightTopCorner")
            {
                backgroundMap.SetTile(position, topRightCornerRoot);
               
            }
            else if (tile.name == "DefaultTileRightBottomCorner")
            {
                backgroundMap.SetTile(position, bottomRightCornerRoot);
                
            }
            else if (tile.name == "DefaultTileLeftTopCorner" || tile.name ==  "DefaultTileCorner")
            {
                Debug.Log("What is position? It is " + position);
                backgroundMap.SetTile(position, topLeftCornerRoot);
                
 
            } 
            else if (tile.name == "DefaultTileLeftBottomCorner")
            {
                backgroundMap.SetTile(position, bottomLeftCornerRoot);
                
            }

            
            return "NewRoot";
            //startRoot = position;
            //Debug.Log("START ROOT: " + startRoot);
        }
        
        ButtonPressed = true;
        return "Void";
    }

    
    Vector3Int GetRow(Tilemap mapping, Vector3Int position, int start, int end, bool add, bool vert, bool rootTime)
    {
        StartCoroutine(ButtonPressedMethod());
        Vector3Int newPosition = position;
        Vector3Int passedPosition = position;
        //bool foundNewSprout = false;

        

        if (add == true)
        {
   
            for (int value = start; value < end; value++) // UP AND RIGHT CODE
            {
                
                if (vert)
                {
                    // This is going to head up 
                    
                    newPosition = position + new Vector3Int(0, value, 0);
                    Angle = "UP";
                    Debug.Log("GET ROW | Going up");
                }
                else
                {
                    // This is going to the right
                    newPosition = position + new Vector3Int(value, 0, 0);
                    Angle = "RIGHT";
                    Debug.Log("GET ROW | Going to the right");
                    
                    
                }
                

                TileBase unknownTile = objectMapping.GetTile(newPosition);
                TileBase backgroundTile = mapping.GetTile(newPosition);


                if (unknownTile)
                {
                    Debug.Log("Found object tile");
                    string result = CheckTile(unknownTile, mapping, newPosition);

                    if (result == "Stone")
                    {
                        Debug.Log("Stopping");
                        //return newPosition;
                        if (Angle == "LEFT")
                        {
                            newPosition = newPosition + new Vector3Int(1, 0, 0);
                            mapping.SetTile(newPosition, leftWallRoot);
                        }
                        else if (Angle == "RIGHT")
                        {
                            newPosition = newPosition - new Vector3Int(1, 0, 0);
                            mapping.SetTile(newPosition, rightWallRoot);
                        }
                        Debug.Log(passedPosition);
                        passedPosition = newPosition;

                        return passedPosition;
                    } else if (result == "Win")
                    {
                        // Move to Next Level
                        NextLevel();
                    } else if (result == "Reset")
                    {
                        Debug.Log("Test");
                        ResetLevel();
                    }
                }

                if (backgroundTile)
                {
                    string result = CheckTile(backgroundTile, mapping, newPosition);
                    if (result == "NewRoot")
                    {
                        Debug.Log("Found wall");
                        passedPosition = newPosition;
                    }
                }
                else
                {
                    
                    return passedPosition;
                }
            }
        } else if (add == false) // DOWN AND LEFT CODE
        {

            Debug.Log("Do method");
            for (int value = start; value < end; value++)
            {
  
                if (vert)
                {
                    // This is going to head down 
                    newPosition = position - new Vector3Int(0, value, 0);
                    Angle = "DOWN";
                    Debug.Log("Down angle");
                    
                }
                else
                {
                    // This is going to the left
                    newPosition = position - new Vector3Int(value, 0, 0);
                    Angle = "LEFT";
                    Debug.Log("Going left");
                    
                }

                TileBase unknownTile = objectMapping.GetTile(newPosition);
                TileBase backgroundTile = mapping.GetTile(newPosition);

 

                if (unknownTile)
                {
                    string result = CheckTile(unknownTile, mapping, newPosition);
                    if (result == "Stone")
                    {
                        //return newPosition;
                        return passedPosition;
                    }
                    else if (result == "Win")
                    {
                        // Move to Next Level
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                    else if (result == "Reset")
                    {
                        ResetLevel();
                    } else
                    {
                        
                    }
                }

                if (backgroundTile)
                {
                    Debug.Log("Found background tile");
                    string result = CheckTile(backgroundTile, mapping, newPosition);
                    if (result == "NewRoot")
                    {
                        Debug.Log("Found wall");
                        passedPosition = newPosition;
                    }

                } else
                {
                    //objectMapping.SetTile(position, pointer);
                    return passedPosition;
                }
            }
        }

        
        Debug.Log("Passed position is " + passedPosition);
        return passedPosition;
       

        
        
    }

    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float yAxisValue = Input.GetAxis("Vertical");

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);

        Tilemap mapping = GetComponent<Tilemap>();
        Vector3Int position = mapping.WorldToCell(currentRoot);

        pointTrackerText.text = points.ToString();

        if (ButtonPressed == false)
        {

            
            
            //Debug.Log(position);
            
            //objectMapping.SetTile(position, pointer);
            if (xAxisValue > 0)
            { // Go Right
                //currentRoot = GetRow(mapping, position, 0, bounds.size.x, true, false, false);
                Debug.Log("Pressed right");
                currentRoot = GetRow(mapping, position, position.x, bounds.size.x, true, false, false);
            }

            else if (xAxisValue < 0)
            { // Go Left
                //currentRoot = GetRow(mapping, position, bounds.size.x, 0, false, false, false);
                currentRoot = GetRow(mapping, position, bounds.size.x, position.x, false, false, false);
            }

            else if (yAxisValue > 0)
            { // Go Up
                Debug.Log("Pressed up");
                //currentRoot = GetRow(mapping, position, 0, bounds.size.y, true, true, false);
                currentRoot = GetRow(mapping, position, position.y, bounds.size.y, true, true, false);
            }

            else if (yAxisValue < 0)
            { // Go Down
                
                //currentRoot = GetRow(mapping, position, bounds.size.y, 0, false, true, false);
                currentRoot = GetRow(mapping, position, position.y, bounds.size.y, false, true, false);
                
            }
            
            
        }
        

        
    }
}
