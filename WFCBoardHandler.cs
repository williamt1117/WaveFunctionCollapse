using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCBoardHandler : MonoBehaviour
{
    enum Direction
    {
        up = 0,
        right = 1,
        down = 2,
        left = 3
    }

    public Transform cameraTransform;
    public Camera cameraComponent;

    public Sprite[] tileMapSprites;
    public int[] tileWeights;

    public int gridSizeX;
    public int gridSizeY; 
    public int seed;
    public int waitTimeMilliseconds;

    private int[,] tileAdjacencyRuleset; //Dimension 1: Tile, Dimension 2: Direction (enum) 
    //0 represents a yellow connection edge, 1 represents a blue empty edge

    private WFCTile[,] board;


    void Start()
    {
        if (seed != -1)
            Random.InitState(seed);

        SetCamera();

        Initialize();
        StartCoroutine(IterateBoard());  
    }

    IEnumerator IterateBoard()
    {
        for (int i = 0; i < gridSizeX*gridSizeY; i++)
        {
            UpdateTilePossibilities();
            Collapse();
            yield return new WaitForSeconds((float)waitTimeMilliseconds/1000.0f);
        }
    }

    void SetCamera()
    {
        cameraTransform.position = new Vector3((float)gridSizeX/2.0f,(float)gridSizeY/2.0f - 0.5f, -10);
        cameraComponent.orthographicSize = (float)gridSizeX/2.0f + 1;
    }

    void UpdateTilePossibilities()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                UpdateTile(x,y);
            }
        }
    }

    void UpdateTile(int x, int y)
    {
        List<int> deleteList = new List<int>(); //since tiles can not be deleted during foreach loop

        foreach (int tileNum in board[x,y].possibleTiles)
        {
            bool[] match = new bool[4];
            for (int i = 0; i < 4; i++)
                match[i] = false;

            //check neighboring tiles (if they exist) and see if they contain a possible match
            for (int dir = (int)Direction.up; dir <= (int)Direction.left; dir++)
            {
                //if the tile exists within bounds
                int neighborx = x;
                int neighbory = y;
                switch (dir) {
                    case 0:
                        neighbory++;
                        break;
                    case 1:
                        neighborx++;
                        break;
                    case 2:
                        neighbory--;
                        break;
                    case 3:
                        neighborx--;
                        break;
                }
                if (neighborx < 0 || neighborx >= gridSizeX || neighbory < 0 || neighbory >= gridSizeY)
                {
                    //invalid bounds
                    match[dir] = true;
                }
                else
                {
                    //if one of the neighbor tiles contains a tile with the same edgetype, set match[dir] to true
                    int edgetype = tileAdjacencyRuleset[tileNum, dir];
                    int neighbordir = (int)ReverseDirection((Direction)dir);
                    foreach (var neighbortile in board[neighborx,neighbory].possibleTiles)
                    {
                        int neighboredgetype = tileAdjacencyRuleset[neighbortile, neighbordir];
                        if (edgetype == neighboredgetype)
                            match[dir] = true;
                    }
                }
            } 

            if (match[0] != true || match[1] != true || match[2] != true || match[3] != true)
            {
                deleteList.Add(tileNum);
            }
        }

        foreach (int tileToRemove in deleteList)
            board[x,y].RemovePossibility(tileToRemove);
    }

    Direction ReverseDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.up:
                return Direction.down;
            case Direction.right:
                return Direction.left;
            case Direction.down:
                return Direction.up;
            case Direction.left:
                return Direction.right;
        }
        return Direction.up; //this should never be reached, just to make the compiler happy
    }

    void Collapse()
    {
        int lowesttilecount = 9999;
        List<Vector2> lowestcoords = new List<Vector2>();

        //Find tile with highest entropy (lowest amount of possible tiles) that hasn't been collapsed yet
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if ((board[x,y].Possibilities() < lowesttilecount) && board[x,y].IsCollapsed() == false)
                {
                    lowestcoords = new List<Vector2>();
                    lowestcoords.Add(new Vector2(x, y));
                    lowesttilecount = board[x,y].Possibilities();
                }
                else if (board[x,y].Possibilities() == lowesttilecount)
                {
                    lowestcoords.Add(new Vector2(x, y));
                }
            }
        }

        //selects random set of coords from list of high-entropy tile coords
        int index = Random.Range(0, lowestcoords.Count);
        Vector2 selectedTile = lowestcoords[index];
        int lowestx = (int)selectedTile.x;
        int lowesty = (int)selectedTile.y;

        //select random tile from list using weights
        int totalWeight = 0;
        foreach (int tile in board[lowestx,lowesty].possibleTiles)
        {
            totalWeight += tileWeights[tile];
        }

        int selected = -1;
        int randomWeight = Random.Range(0, totalWeight);
        for (int idx = 0; idx < board[lowestx,lowesty].Possibilities(); idx++)
        {
            //if randomWeight is less than the sum of weights up to item 'idx'
            int partialWeightSum = 0;
            for (int j = 0; j <= idx; j++)
            {
                partialWeightSum += tileWeights[board[lowestx,lowesty].possibleTiles[j]];
            }
            if (randomWeight < partialWeightSum)
            {
                selected = board[lowestx,lowesty].possibleTiles[idx];
                break;
            }
        }

        //spawn tile
        board[lowestx,lowesty].CollapseTile(new Vector2(lowestx,lowesty), tileMapSprites[selected], selected, transform);
    }

    //initializes tile adjacencies and creates super position
    void Initialize()
    {
        board = new WFCTile[gridSizeX,gridSizeY];
        tileAdjacencyRuleset = new int[tileMapSprites.Length, 4];

        //create superposition (add all tiles to possible list)
        for (int x = 0; x < gridSizeX; x++){
            for (int y = 0; y < gridSizeY; y++){
                board[x,y] = new WFCTile();
                for (int z = 0; z < tileMapSprites.Length; z++){
                    board[x,y].AddPossibility(z);
                }
            }
        }

        //initialize all assuming yellow connection edges and correct others
        for (int x = 0; x < 15; x++)
            for (int y = 0; y < 4; y++)
                tileAdjacencyRuleset[x,y]=0;  

        //in future this could be done by analzying sprites directly as opposed to manually correcting
        tileAdjacencyRuleset[1,1] = 1;
        tileAdjacencyRuleset[1,3] = 1;
        tileAdjacencyRuleset[2,0] = 1;
        tileAdjacencyRuleset[2,2] = 1;
        tileAdjacencyRuleset[3,0] = 1;
        tileAdjacencyRuleset[3,1] = 1;
        tileAdjacencyRuleset[3,2] = 1;
        tileAdjacencyRuleset[3,3] = 1;
        tileAdjacencyRuleset[4,3] = 1;
        tileAdjacencyRuleset[5,2] = 1;
        tileAdjacencyRuleset[6,1] = 1;
        tileAdjacencyRuleset[7,0] = 1;
        tileAdjacencyRuleset[8,2] = 1;
        tileAdjacencyRuleset[8,3] = 1;
        tileAdjacencyRuleset[9,1] = 1;
        tileAdjacencyRuleset[9,2] = 1;
        tileAdjacencyRuleset[10,0] = 1;
        tileAdjacencyRuleset[10,1] = 1;
        tileAdjacencyRuleset[11,3] = 1;
        tileAdjacencyRuleset[11,0] = 1;
        tileAdjacencyRuleset[12,1] = 1;
        tileAdjacencyRuleset[12,2] = 1;
        tileAdjacencyRuleset[12,3] = 1;
        tileAdjacencyRuleset[13,0] = 1;
        tileAdjacencyRuleset[13,1] = 1;
        tileAdjacencyRuleset[13,2] = 1;
        tileAdjacencyRuleset[14,3] = 1;
        tileAdjacencyRuleset[14,0] = 1;
        tileAdjacencyRuleset[14,1] = 1;
        tileAdjacencyRuleset[15,2] = 1;
        tileAdjacencyRuleset[15,3] = 1;
        tileAdjacencyRuleset[15,0] = 1;
    }
}
