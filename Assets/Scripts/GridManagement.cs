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

    private bool ButtonPressed = false;
    public static int points = 0;
    public TMP_Text pointTrackerText;



    public Tilemap objectMapping;
    public Vector3 startRoot;
    private Vector3 currentRoot;
    private string Angle;

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

    public string[] tileNames;

    public Vector3Int position;



    void Start()
    {
        //Debug.Log("The right side is an illusion.");
        Tilemap mapping = GetComponent<Tilemap>();

        bounds = mapping.cellBounds;
        allTiles = mapping.GetTilesBlock(bounds);
        currentRoot = startRoot;
        scorePerLevel = new List<int>();


        
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

    string CheckTile(TileBase tile, Tilemap backgroundMap, Vector3Int newPosition)
    {


        if (newPosition.x > position.x)
        {
            // It is going to the right

            Angle = "RIGHT";
        } else if (newPosition.x < position.x)
        {
            Angle = "LEFT";
        } else if (newPosition.y > position.y)
        {
            Angle = "UP";
        } else if (newPosition.y < position.y)
        {
            Angle = "DOWN";
        }

        Debug.Log("ANGLE " + Angle);
        
        Debug.Log("Checking tile: " + tile.name + " at " + newPosition);
        
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
                Debug.Log("Root sprout at: " + newPosition);
                if (Angle == "RIGHT")
                {
                    
                    backgroundMap.SetTile(newPosition, leftToRightRoot);
                    
                } else if (Angle == "LEFT")
                {  
                    backgroundMap.SetTile(newPosition, rightToLeftRoot);
                    
                } else if (Angle == "UP" || Angle == "DOWN")
                {
                    
                    backgroundMap.SetTile(newPosition, straightRoot);
                } else
                {
                    Debug.Log("Tested out");
                }

            }
            else if (tile.name == "RightDefaultTileWall")
            {
                if (Angle == "UP" || Angle == "DOWN")    
                {
                    backgroundMap.SetTile(newPosition, straightRoot);
                } else { 
                    backgroundMap.SetTile(newPosition, rightWallRoot);
                }
                
                
            }
            else if (tile.name == "LeftDefaultWall")
            {
                
                if (Angle == "UP" || Angle == "DOWN")
                {
                    backgroundMap.SetTile(newPosition, straightRoot);
                }
                else
                {
                    backgroundMap.SetTile(newPosition, leftWallRoot);
                }
            } 
            else if (tile.name == "DefaultTileRightTopCorner")
            {
                backgroundMap.SetTile(newPosition, topRightCornerRoot);
               
            }
            else if (tile.name == "DefaultTileRightBottomCorner")
            {
                backgroundMap.SetTile(newPosition, bottomRightCornerRoot);
                
            }
            else if (tile.name == "DefaultTileLeftTopCorner" || tile.name ==  "DefaultTileCorner")
            {
                Debug.Log("What is position? It is " + position);
                backgroundMap.SetTile(newPosition, topLeftCornerRoot);
                
 
            } 
            else if (tile.name == "DefaultTileLeftBottomCorner")
            {
                backgroundMap.SetTile(newPosition, bottomLeftCornerRoot);
                
            }

            
            return "NewRoot";
            //startRoot = position;
            //Debug.Log("START ROOT: " + startRoot);
        }
        
        ButtonPressed = true;
        return "Void";
    }
    

    Vector3Int DetermineCondition(Tilemap mapping, string result, Vector3Int newPosition)
    {
        if (result == "Stone")
        {

            
            if (Angle == "LEFT")
            {
                 
                mapping.SetTile(newPosition, leftWallRoot);
            }
            else if (Angle == "RIGHT")
            {
                
                mapping.SetTile(newPosition, rightWallRoot);
            }

  
            
        }
        else if (result == "Win")
        {
            // Move to Next Level
            NextLevel();
            
        }
        else if (result == "Reset")
        {

            ResetLevel();
            
        }
        else
        {
            Debug.Log(result);
            
        }

        Debug.Log("In DetermineCondition... " + newPosition);
        return newPosition;
    }

    Vector3Int NewPositionMethod(Tilemap mapping, int x, int y)
    {
        // It adds one to the current position.
        Vector3Int newPotentialPosition;
        newPotentialPosition = position + new Vector3Int(x, y, 0);

        TileBase unknownObject = objectMapping.GetTile(newPotentialPosition);
        TileBase backgroundTile = mapping.GetTile(newPotentialPosition);
        // It checks to see if there is a tile present.

        if (unknownObject)
        {
            // An object is found, then check what kind it is.

            string objectResult = CheckTile(unknownObject, mapping, newPotentialPosition);
            position = DetermineCondition(mapping, objectResult, newPotentialPosition);
        }

        if (backgroundTile)
        {
            // Background tile found.

            string backgroundResult = CheckTile(backgroundTile, mapping, newPotentialPosition);
            position = DetermineCondition(mapping, backgroundResult, newPotentialPosition);

            
        }

        
        return position;
    }

    //Vector3Int GetRow(Tilemap mapping, Vector3Int position, int start, int end, bool add, bool vert, bool rootTime)
    Vector3Int GetRow(Tilemap mapping, Vector3Int position, string command)
    {
        StartCoroutine(ButtonPressedMethod());
 
        Debug.Log(position);
        Vector3Int lastPosition = position;
        /*
         * The code takes in a position, then it uses a while loop to keep checking the new position. 
         * If the tile is empty, then it adds to it.
         */
        bool growRoot = true;

        while (growRoot)
        {
            switch(command)
            {
                case "up":
                    position = NewPositionMethod(mapping, 0, 1);
                    break;
                case "down":
                    position = NewPositionMethod(mapping, 0, -1);
                    break;
                case "left":
                    position = NewPositionMethod(mapping, -1, 0);
                    break;
                case "right":
                    position = NewPositionMethod(mapping, 1, 0);
                    break;

               
            }

            if (lastPosition == position)
            {
                // If the position stayed the same, then it stopped at a border.
                break;
            }
            lastPosition = position;
            /*if (command == "up")
            {
                
                Debug.Log("New position: " + position);

                
            }  */
        }
        

        return position;

        /*if (add == true)
        {
   
            for (int value = start; value < end; value++) // UP AND RIGHT CODE
            {
                
                if (vert)
                {
                    // This is going to head up 
                    
                    newPosition = position + new Vector3Int(0, value, 0);
                    Angle = "UP";
                    
                }
                else
                {
                    // This is going to the right
                    newPosition = position + new Vector3Int(value, 0, 0);
                    Angle = "RIGHT";

                }
                

                TileBase unknownTile = objectMapping.GetTile(newPosition);
                TileBase backgroundTile = mapping.GetTile(newPosition);


                if (unknownTile)
                {
                    
                    string result = CheckTile(unknownTile, mapping, newPosition);

                    if (result == "Stone")
                    {
                        
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
                        
                        passedPosition = newPosition;

                        return passedPosition;
                    } else if (result == "Win")
                    {
                        // Move to Next Level
                        NextLevel();
                    } else if (result == "Reset")
                    {
                
                        ResetLevel();
                    }
                }

                if (backgroundTile)
                {
                    string result = CheckTile(backgroundTile, mapping, newPosition);
                    if (result == "NewRoot")
                    {
                        
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

           
            for (int value = start; value < end; value++)
            {
  
                if (vert)
                {
                    // This is going to head down 
                    newPosition = position - new Vector3Int(0, value, 0);
                    Angle = "DOWN";
                    
                    
                }
                else
                {
                    // This is going to the left
                    newPosition = position - new Vector3Int(value, 0, 0);
                    Angle = "LEFT";
                    
                    
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
                    
                    string result = CheckTile(backgroundTile, mapping, newPosition);
                    if (result == "NewRoot")
                    {
                       
                        passedPosition = newPosition;
                    }

                } else
                {
                    //objectMapping.SetTile(position, pointer);
                    return passedPosition;
                }
            }
        }
        */
        //return passedPosition;

    }

    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float yAxisValue = Input.GetAxis("Vertical");

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);

        Tilemap mapping = GetComponent<Tilemap>();
        position = mapping.WorldToCell(currentRoot);

        pointTrackerText.text = points.ToString();

        if (ButtonPressed == false)
        {

            
            
            
            //objectMapping.SetTile(position, pointer);
            if (xAxisValue > 0)
            { // Go Right

                Debug.Log("Right");

                currentRoot = GetRow(mapping, position, "right");

                //currentRoot = GetRow(mapping, position, position.x, bounds.size.x, true, false, false);
            }

            else if (xAxisValue < 0)
            { // Go Left
                Debug.Log("Left");

                currentRoot = GetRow(mapping, position, "left");
                //currentRoot = GetRow(mapping, position, bounds.size.x, position.x, false, false, false);
            }

            else if (yAxisValue > 0)
            { // Go Up
                Debug.Log("Up");

                currentRoot = GetRow(mapping, position, "up");

                //currentRoot = GetRow(mapping, position, position.y, bounds.size.y, true, true, false);
            }

            else if (yAxisValue < 0)
            { // Go Down
                Debug.Log("Down");

                currentRoot = GetRow(mapping, position, "down");
                //currentRoot = GetRow(mapping, position, position.y, bounds.size.y, false, true, false);

            }
            
            
        }
        

        
    }
}
