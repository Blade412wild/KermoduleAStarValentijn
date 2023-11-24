using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics.Tracing;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        bool foundPath = false;
        int width = 300;
        int height = 300;

        List<Node> allNodes = new List<Node>();
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();
        List<Node> neighbourList = new List<Node>();
        Node[,] nodeGrid = new Node[width, height];

        //make a node grid
        nodeGrid = createVoid(height, width, allNodes);

        // ik bekijk het pad vanaf het perspectief vanaf de currentNode, dus ik loop eigenlijk het pad met een test node
        Node currentNode = nodeGrid[0, 0];
        currentNode.GScore = 0;

        // add de begin node to the open list
        //Node beginNode = nodeGrid[0, 0];
        //beginNode.GScore = 0;
        //beginNode.HScore = Vector2.Distance(beginNode.position, endPos);
        //openNodes.Add(beginNode);


        


        int counter = 0;
        //pathfinding loop
        while (foundPath == false)
        {

            Debug.Log("iteratie : " +  counter);
            Debug.Log("currentNode postion = " + currentNode.position);

            // Get neigbours
            neighbourList = GetNeighbours(currentNode, allNodes, openNodes, grid, closedNodes);
            calculateNeighbourValues(neighbourList, currentNode, endPos);

            Vector2Int newDestination = CalculaterNewPos(neighbourList, closedNodes, openNodes);
            MoveCurrentNode(currentNode, newDestination);

            GameObject blok = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blok.transform.position = new Vector3(currentNode.position.x, 1, currentNode.position.y);



            if (currentNode.position == endPos)
            {
                foundPath = true;
            }


            counter++;
        }

        return null;
    }


    private void MoveCurrentNode(Node _currentNode, Vector2Int _newDestination)
    {
        _currentNode.position = _newDestination;
    }

    private Vector2Int CalculaterNewPos(List<Node> _neighbourList, List<Node> _closedList, List<Node> _openList)
    {
        // deze moet bij het begin op true, daardoor krijgt de besteFScore de waarde van de eerste neighbhour
        bool isBestFScoreNull = true;
        float bestFScore = 10000000.0f;
        Vector2Int newPos = new Vector2Int(0, 0);

        Debug.Log("openlist count : " + _openList.Count);
        // getting de Fscore Data from the neigbours and comparing them
        foreach (Node openNode in _openList)
        {
            Debug.Log("bestFScore = " + openNode.FScore);
            if (isBestFScoreNull == true)
            {
                bestFScore = openNode.FScore;
                isBestFScoreNull = false;
            }

            if (isBestFScoreNull == false && bestFScore > openNode.FScore)
            {
                bestFScore = openNode.FScore;
            }
        }

        Node nodeWithBestFScore = null;
        // matching 
        foreach (Node openNode in _openList)
        {
            if (bestFScore == openNode.FScore)
            {
                nodeWithBestFScore = openNode;
                newPos = openNode.position;
                break;
            }
        }
        _openList.Remove(nodeWithBestFScore);
        Debug.Log("removed node form open list with position : " + nodeWithBestFScore.position);
        _closedList.Add(nodeWithBestFScore);
        Debug.Log("Added node to closed list with position : " + nodeWithBestFScore.position);


        //foreach(Node openNode in _openList)
        //{
        //    if(openNode.position == new Vector2Int(0, 1))
        //    {
        //        Debug.Log("ja ik besta nog : " + openNode.position);
        //    }
        //}

        //if (_openList.Contains(nodeWithBestFScore))
        //{
        //    Debug.Log("hey hey ik zit er nog in");
        //}


        return newPos;
    }

    private List<Node> GetNeighbours(Node currentNode, List<Node> _allNodesList, List<Node> _openList, Cell[,] _grid, List<Node> _closedNode)
    {
        Vector2Int upperNeigbour = new Vector2Int(currentNode.position.x, currentNode.position.y + 1);
        Vector2Int lowerNeigbour = new Vector2Int(currentNode.position.x, currentNode.position.y - 1);
        Vector2Int rightNeibour = new Vector2Int(currentNode.position.x + 1, currentNode.position.y);
        Vector2Int leftNeigbour = new Vector2Int(currentNode.position.x - 1, currentNode.position.y);


        List<Node> neighbourList = new List<Node>();

        foreach (Node neighbour in _allNodesList)
        {
            //if (neighbour.position == leftNeigbour || neighbour.position == rightNeibour || neighbour.position == upperNeigbour || neighbour.position == lowerNeigbour)
            //{
            //    // check for walls
            //    neighbourList.Add(neighbour);
            //    _openList.Add(neighbour);
            //}

            if (neighbour.position == rightNeibour && _grid[rightNeibour.x, rightNeibour.y].HasWall(Wall.LEFT) != true && _closedNode.Contains(neighbour) != true)
            {
                neighbourList.Add(neighbour);
                _openList.Add(neighbour);
            }

            if (neighbour.position == upperNeigbour && _grid[upperNeigbour.x, upperNeigbour.y].HasWall(Wall.DOWN) != true && _closedNode.Contains(neighbour) != true)
            {
                neighbourList.Add(neighbour);
                _openList.Add(neighbour);
            }

            if (neighbour.position == lowerNeigbour && _grid[lowerNeigbour.x, lowerNeigbour.y].HasWall(Wall.UP) != true && _closedNode.Contains(neighbour) != true) 
            {
                neighbourList.Add(neighbour);
                _openList.Add(neighbour);

            }

            if (neighbour.position == leftNeigbour && _grid[leftNeigbour.x, leftNeigbour.y].HasWall(Wall.RIGHT) != true && _closedNode.Contains(neighbour) != true) 
            {
                neighbourList.Add(neighbour);
                _openList.Add(neighbour);

            }


        }

        return neighbourList;
    }
    private Node[,] createVoid(int _height, int _width, List<Node> _allNodesList)
    {
        Node[,] nodeGrid = new Node[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {

                Node node = new Node();
                node.position = new Vector2Int(x, y);
                nodeGrid[x, y] = node;

                _allNodesList.Add(node);
            }
        }
        return nodeGrid;
    }

    private void calculateNeighbourValues(List<Node> _neigbourlist, Node _currentNode, Vector2Int _endPos)
    {
        int neigbourId = 1;
        foreach (Node neighbour in _neigbourlist)
        {
            CalculateValueH(_currentNode, _endPos, neighbour);
            CalculateValueG(_currentNode, neighbour);

            Debug.Log("neighbour " + neigbourId + " postion " + neighbour.position + " HScore = " + neighbour.HScore + " GScore = " + neighbour.GScore + " FScore = " + neighbour.FScore);
            neigbourId++;

        }

        neigbourId = 1;

    }

    private void CalculateValueH(Node _currentNode, Vector2Int _endPos, Node _neigbour)
    {
        _neigbour.HScore = Vector2.Distance(_neigbour.position, _endPos);
    }
    private void CalculateValueG(Node _currentNode, Node _neigbour)
    {
        _neigbour.GScore = _currentNode.GScore + 1;
    }
    private void CheckForWalls(Node _neigbour, Cell[,] _grid)
    {
        _grid[100, 100].HasWall(Wall.RIGHT);
    }



    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }

        public Node(Vector2Int _position)
        {
            this.position = _position;
        }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }

    }
}
