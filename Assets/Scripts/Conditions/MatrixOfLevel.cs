using System;
using System.Collections.Generic;
using UnityEngine;
using static MatrixOfLevel.BlockAtMatrix;

public class MatrixOfLevel
{
    public class BlockAtMatrix
    {
        public enum BlockType : byte
        {
            Empty,
            Blocked,
            BlockedByLine,
            Finish,
            ColoredSquare,
            ColoredStar
        }

        public BlockAtMatrix()
        {
            blockType = default;
            blockColor = default;
            isTurn = default;
            transform = null;
            lineRenderer = null;
        }

        public BlockType blockType;
        public Color blockColor;
        public bool isTurn;
        public Transform transform;
        public LineRenderer lineRenderer;
    }

    public enum ReturnType
    {
        DontMove,
        DeadEnd,
        Finish,
        Turn,
        ComeBack
    }

    private readonly Texture2D _level;

    private BlockAtMatrix[,] _matrixOfBlockTypes;
    private BlockType[,] _originalMatrixOfTypes;

    private Stack<Vector2Int> _directionStack;

    public MatrixOfLevel(Texture2D level)
    {
        _level = level;
        _directionStack = new Stack<Vector2Int>(30);
    }

    public BlockAtMatrix this[int x, int y] => _matrixOfBlockTypes[x, y];

    public int Length => _matrixOfBlockTypes.Length;

    public void GenerateMatrix()
    {
        _matrixOfBlockTypes = new BlockAtMatrix[_level.width, _level.height];
        for (int x = 0; x < _level.width; x++)
            for (int y = 0; y < _level.height; y++)
                _matrixOfBlockTypes[x, y] = new BlockAtMatrix();
    }

    public void SetTurnBlock(int x, int y, Transform transform, LineRenderer lineRenderer)
    {
        _matrixOfBlockTypes[x, y].isTurn = true;
        _matrixOfBlockTypes[x, y].transform = transform;
        _matrixOfBlockTypes[x, y].lineRenderer = lineRenderer;
    }

    public void ConvertToMatrix(int x, int y, BlockType type, Color color)
    {
        _matrixOfBlockTypes[x, y].blockType = type;
        _matrixOfBlockTypes[x, y].blockColor = color;
    }

    public void SaveOriginalMatrix()
    {
        AddBorders();
        _originalMatrixOfTypes = new BlockType[_level.width, _level.height];
        for (int x = 0; x < _level.width; x++)
            for (int y = 0; y < _level.height; y++)
                _originalMatrixOfTypes[x, y] = _matrixOfBlockTypes[x, y].blockType;
    }

    public void Reset()
    {
        for (int x = 0; x < _level.width; x++)
            for (int y = 0; y < _level.height; y++)
                _matrixOfBlockTypes[x, y].blockType = _originalMatrixOfTypes[x, y];
        _directionStack = new Stack<Vector2Int>(30);
    }

    public ReturnType Move(Vector2Int direction, out Vector3 newPos, ref int x, ref int y, ref LineRenderer lineRenderer)
    {
        int startX = x;
        int startY = y;
        
        if (_directionStack.Count != 0 && direction == _directionStack.Peek())  //возвращение
        {
            while (true)
            {
                _matrixOfBlockTypes[x, y].blockType = BlockType.Empty;
                
                x += direction.x;
                y += direction.y;
                if (_matrixOfBlockTypes[x, y].isTurn == true)
                {
                    _directionStack.Pop();
                    lineRenderer = _matrixOfBlockTypes[x, y].lineRenderer;
                    newPos = _matrixOfBlockTypes[x, y].transform.position;
                    return ReturnType.ComeBack;
                }
            }
        }
        
        _directionStack.Push(new Vector2Int(0 - direction.x, 0 - direction.y)); // запись пути
        while (true)
        {
            x += direction.x;
            y += direction.y;
            if (_matrixOfBlockTypes[x, y].blockType == BlockType.Blocked 
                ||_matrixOfBlockTypes[x, y].blockType == BlockType.BlockedByLine)
            {
                x -= direction.x;
                y -= direction.y;
                newPos = _matrixOfBlockTypes[x, y].transform.position;
                if ((startX, startY) == (x, y))
                {
                    _directionStack.Pop();
                    return ReturnType.DontMove;
                }

                return ReturnType.DeadEnd;
            }
            if (_matrixOfBlockTypes[x, y].blockType == BlockType.Finish)
            {
                newPos = _matrixOfBlockTypes[x, y].transform.position;
                return ReturnType.Finish;
            }
            
            _matrixOfBlockTypes[x, y].blockType = BlockType.BlockedByLine;    //Блокируем пройденный путь по ходу

            if (_matrixOfBlockTypes[x, y].isTurn)
            {
                lineRenderer = _matrixOfBlockTypes[x, y].lineRenderer;
                newPos = _matrixOfBlockTypes[x, y].transform.position;
                return ReturnType.Turn;
            }
        }
    }

    private void AddBorders()
    {
        for (int x = 0; x < _level.width; x++)
        {
            for (int y = 0; y < _level.height; y++)
            {
                if (_matrixOfBlockTypes[x, y].blockType == BlockType.Empty
                    || _matrixOfBlockTypes[x, y].blockType == BlockType.Finish
                    || _matrixOfBlockTypes[x, y].blockType == BlockType.BlockedByLine)
                {
                    _matrixOfBlockTypes[x, y - 1].blockType = BlockType.BlockedByLine;
                    break;
                }
            }
            for (int y = _level.height - 1; y > 0; y--)
            {
                if (_matrixOfBlockTypes[x, y].blockType == BlockType.Empty
                    || _matrixOfBlockTypes[x, y].blockType == BlockType.Finish
                    || _matrixOfBlockTypes[x, y].blockType == BlockType.BlockedByLine)
                {
                    _matrixOfBlockTypes[x, y + 1].blockType = BlockType.BlockedByLine;
                    break;
                }
            }
        }
        for (int y = 0; y < _level.height; y++)
        {
            for (int x = 0; x < _level.width; x++)
            {
                if (_matrixOfBlockTypes[x, y].blockType == BlockType.Empty
                    || _matrixOfBlockTypes[x, y].blockType == BlockType.Finish
                    || _matrixOfBlockTypes[x, y].blockType == BlockType.BlockedByLine)
                {
                    _matrixOfBlockTypes[x - 1, y].blockType = BlockType.BlockedByLine;
                    break;
                }
            }
            for (int x = _level.width - 1; x > 0; x--)
            {
                if (_matrixOfBlockTypes[x, y].blockType == BlockType.Empty
                    || _matrixOfBlockTypes[x, y].blockType == BlockType.Finish
                    || _matrixOfBlockTypes[x, y].blockType == BlockType.BlockedByLine)
                {
                    _matrixOfBlockTypes[x + 1, y].blockType = BlockType.BlockedByLine;
                    break;
                }
            }
        }
    }
}
