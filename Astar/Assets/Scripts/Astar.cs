using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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
    /// 

    float previousTime;
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        DateTime startTime = DateTime.Now;
        bool foundPath = false;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        List<Node> allNodes = new List<Node>();
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();
        List<Node> neighbourList = new List<Node>();
        Node[,] nodeGrid = new Node[width, height];
        Dictionary<Vector2Int, Node> allNodesDic = new Dictionary<Vector2Int, Node>();

        //make a node grid
        nodeGrid = createVoid(height, width, allNodes, allNodesDic);

        // ik bekijk het pad vanaf het perspectief vanaf de currentNode, dus ik loop eigenlijk het pad met een test node
        Node currentNode = nodeGrid[startPos.x, startPos.y];
        currentNode.GScore = 0;

        // add de begin node to the open list
        Node beginNode = new Node(startPos);
        beginNode.GScore = 0;
        beginNode.HScore = Vector2.Distance(beginNode.position, endPos);
        closedNodes.Add(beginNode);

        Node endNode = nodeGrid[endPos.x, endPos.y];


        int counter = 0;
        //pathfinding loop
        while (foundPath == false)
        {

            if (currentNode.position == endPos)
            {
                foundPath = true;
                Debug.Log("foundPath");
                List<Vector2Int> path = RetracePath(beginNode, endNode);
                DateTime endTime = DateTime.Now;

                TimeSpan timePast = endTime - startTime;
                Debug.Log(String.Format("Time Spent: {0} Milliseconds", timePast.TotalMilliseconds));
                return path;
            }
            else
            {
                neighbourList = GetNeighbours(currentNode, allNodes, openNodes, grid, closedNodes, endPos);
                calculateNeighbourValues(neighbourList, currentNode, endPos, closedNodes);
                Vector2Int newDestination = GetNewPos(neighbourList, closedNodes, openNodes, currentNode);
                MoveCurrentNode(currentNode, newDestination);

                counter++;

            }
        }

        return null;
    }

    private void MoveCurrentNode(Node _currentNode, Vector2Int _newDestination)
    {
        _currentNode.position = _newDestination;
    }

    private Vector2Int GetNewPos(List<Node> _neighbourList, List<Node> _closedList, List<Node> _openList, Node _currentNode)
    {
        // deze moet bij het begin op true, daardoor krijgt de besteFScore de waarde van de eerste neighbhour
        bool isBestFScoreNull = true;
        float bestFScore = Mathf.Infinity;
        Node bestNode = new Node();

        // getting de Fscore Data from the neigbours and comparing them
        foreach (Node openNode in _openList)
        {
            if (isBestFScoreNull == true)
            {
                bestFScore = openNode.FScore;
                bestNode = openNode;
                isBestFScoreNull = false;
            }

            if (isBestFScoreNull == false && openNode.FScore < bestFScore)
            {
                bestFScore = openNode.FScore;
                bestNode = openNode;
            }
        }

        _openList.Remove(bestNode);
        _closedList.Add(bestNode);

        return bestNode.position;
    }

    private List<Node> GetNeighbours(Node currentNode, List<Node> _allNodesList, List<Node> _openList, Cell[,] _grid, List<Node> _closedNode, Vector2Int _endPos)
    {
        Vector2Int upperNeigbour = new Vector2Int(currentNode.position.x, currentNode.position.y + 1);
        Vector2Int lowerNeigbour = new Vector2Int(currentNode.position.x, currentNode.position.y - 1);
        Vector2Int rightNeibour = new Vector2Int(currentNode.position.x + 1, currentNode.position.y);
        Vector2Int leftNeigbour = new Vector2Int(currentNode.position.x - 1, currentNode.position.y);


        List<Node> neighbourList = new List<Node>();

        foreach (Node neighbour in _allNodesList)
        {

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

    private List<Node> GetNeighbours2(Node currentNode, List<Node> _allNodesList, List<Node> _openList, Cell[,] _grid, List<Node> _closedNode, Vector2Int _endPos, Dictionary<Vector2Int, Node> _allNodesDic)
    {
        Vector2Int upperNeigbour = new Vector2Int(currentNode.position.x, currentNode.position.y + 1);
        Vector2Int lowerNeigbour = new Vector2Int(currentNode.position.x, currentNode.position.y - 1);
        Vector2Int rightNeigbour = new Vector2Int(currentNode.position.x + 1, currentNode.position.y);
        Vector2Int leftNeigbour = new Vector2Int(currentNode.position.x - 1, currentNode.position.y);


        List<Node> neighbourList = new List<Node>();


        if (_allNodesDic.ContainsKey(upperNeigbour))
        {

            if (_grid[upperNeigbour.x, upperNeigbour.y].HasWall(Wall.DOWN) != true && _closedNode.Contains(_allNodesDic[upperNeigbour]) != true)
            {
                neighbourList.Add(_allNodesDic[upperNeigbour]);
                _openList.Add(_allNodesDic[upperNeigbour]);
            }
        }

        if (_allNodesDic.ContainsKey(lowerNeigbour))
        {

            if (_grid[lowerNeigbour.x, lowerNeigbour.y].HasWall(Wall.UP) != true && _closedNode.Contains(_allNodesDic[lowerNeigbour]) != true)
            {
                neighbourList.Add(_allNodesDic[lowerNeigbour]);
                _openList.Add(_allNodesDic[lowerNeigbour]);
            }

        }

        if (_allNodesDic.ContainsKey(rightNeigbour))
        {
            if (_grid[rightNeigbour.x, rightNeigbour.y].HasWall(Wall.LEFT) != true && _closedNode.Contains(_allNodesDic[rightNeigbour]) != true)
            {
                neighbourList.Add(_allNodesDic[rightNeigbour]);
                _openList.Add(_allNodesDic[rightNeigbour]);
            }
        }

        if (_allNodesDic.ContainsKey(leftNeigbour))
        {
            if (_grid[leftNeigbour.x, leftNeigbour.y].HasWall(Wall.RIGHT) != true && _closedNode.Contains(_allNodesDic[leftNeigbour]) != true)
            {
                neighbourList.Add(_allNodesDic[leftNeigbour]);
                _openList.Add(_allNodesDic[leftNeigbour]);
            }

        }

        return neighbourList;
    }
    private Node[,] createVoid(int _height, int _width, List<Node> _allNodesList, Dictionary<Vector2Int, Node> _allNodeDic)
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
                //_allNodeDic.Add(node.position, node);

            }
        }
        return nodeGrid;
    }

    private void calculateNeighbourValues(List<Node> _neigbourlist, Node _currentNode, Vector2Int _endPos, List<Node> _closedList)
    {
        int neigbourId = 1;
        foreach (Node neighbour in _neigbourlist)
        {
            SetParent(_currentNode, neighbour, _closedList);
            CalculateValueH(_currentNode, _endPos, neighbour);
            CalculateValueG(_currentNode, neighbour);

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

        _neigbour.GScore = _neigbour.parent.GScore + 1;
        //_currentNode.GScore = _currentNode.GScore + 1;

    }
    private void SetParent(Node _currentNode, Node _neighbour, List<Node> _closedList)
    {
        Node parent = null;

        foreach (Node node in _closedList)
        {
            if (node.position == _currentNode.position)
            {
                parent = node;
            }
        }

        if (parent != null)
        {
            _neighbour.parent = parent;
        }
        else
        {
            //Debug.Log(" parent = null");
        }




    }
    private List<Vector2Int> RetracePath(Node _beginNode, Node _endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = _endNode;

        while (currentNode.position != _beginNode.position)
        {

            path.Add(currentNode.position);
            currentNode = currentNode.parent;

        }

        path.Reverse();
        return path;
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
