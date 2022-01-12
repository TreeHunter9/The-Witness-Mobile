using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MobileInput;
using UnityEngine;
using Cysharp.Threading.Tasks;

[DefaultExecutionOrder(-1)]
public class LineController : MonoBehaviour
{
    [SerializeField] private StartLevelInput startLevelInput;
    [SerializeField] private GameGUIController _gameGUIController;
    [SerializeField] private AnimationCurve _animationCurve;
    
    private int _currentX, _currentY;
    private CancellationTokenSource _cancellationToken;
    private LineRenderer _currentLineRenderer { get; set; }

    public List<LineRenderer> LineRendererList = new List<LineRenderer>(30);

    public MatrixOfLevel matrixOfLevel;
    public static LineController singleton { get; private set; }

    public event Action onStartGame;

    private void Awake()
    {
        singleton = this;
        
        _cancellationToken = new CancellationTokenSource();
    }

    public void MoveLine(Vector2Int direction1)
    {
        _currentLineRenderer.enabled = true;
        _cancellationToken.Cancel();
        _cancellationToken.Dispose();
        _cancellationToken = new CancellationTokenSource();
        StopAllCoroutines();

        Vector2Int direction = direction1;
        LineRenderer newLineRenderer = null;
        MatrixOfLevel.ReturnType returnType = matrixOfLevel.Move(direction, out Vector3 nextPosition,
            ref _currentX, ref _currentY, ref newLineRenderer);
        nextPosition += Vector3.back;
        if (returnType == MatrixOfLevel.ReturnType.DontMove)
            return;

        if (returnType == MatrixOfLevel.ReturnType.Turn)
        {
            MoveAnimation(_currentLineRenderer.GetPosition(0),
                nextPosition, _currentLineRenderer, _cancellationToken.Token);
            
            
            _currentLineRenderer = newLineRenderer;
            SetOriginPositionToLineRenderer(_currentX, _currentY);
        }
        else if (returnType == MatrixOfLevel.ReturnType.ComeBack)
        {
            if (_currentLineRenderer == newLineRenderer)
            {
                MoveAnimation(_currentLineRenderer.GetPosition(1),
                    nextPosition, _currentLineRenderer, _cancellationToken.Token);
                return;
            }
            _currentLineRenderer.enabled = false;
            _currentLineRenderer = newLineRenderer;
            
            MoveAnimation(_currentLineRenderer.GetPosition(1),
                nextPosition, _currentLineRenderer, _cancellationToken.Token);
        }
        else if (returnType == MatrixOfLevel.ReturnType.DeadEnd)
        {
            MoveAnimation(_currentLineRenderer.GetPosition(1),
                nextPosition, _currentLineRenderer, _cancellationToken.Token);
        }
        else if (returnType == MatrixOfLevel.ReturnType.Finish)
        {
            MoveToFinishAnimation(_currentLineRenderer.GetPosition(0),
                nextPosition, _currentLineRenderer);
        }
    }

    private void SetOriginPositionToLineRenderer(int x, int y)
    {
        for (int i = 0; i < 2; i++)
        {
            _currentLineRenderer.SetPosition(i, matrixOfLevel[x, y].transform.position + Vector3.back);
        }
    }

    private async UniTask MoveAnimation(Vector3 oldPosition, Vector3 newPosition, 
        LineRenderer lineRenderer, CancellationToken token)
    {
        for (float i = 0f; i <= 1f; i += 0.2f)
        {
            if (token.IsCancellationRequested == true)
            {
                lineRenderer.SetPosition(1, newPosition);
                return;
            }

            Vector3 pos = Vector3.Lerp(oldPosition, newPosition, i);
            lineRenderer.SetPosition(1, pos);
            await UniTask.Delay(10);
        }
    }
    
    private async UniTask MoveToFinishAnimation(Vector3 oldPosition, Vector3 newPosition, 
        LineRenderer lineRenderer)
    {
        for (float i = 0f; i <= 1f; i += 0.2f)
        {
            Vector3 pos = Vector3.Lerp(oldPosition, newPosition, i);
            lineRenderer.SetPosition(1, pos);
            await UniTask.Delay(10);
        }
        
        Finish();
    }

    private void Finish()
    {
        bool isFinished = matrixOfLevel[_currentX, _currentY].transform.GetComponent<EndBlockTrigger>()
            .CheckConditions();
        if (isFinished == true)
        {
            _gameGUIController.OpenCompleteLevelMenu();
        }
        else
        {
            Reset();
        }
    }

    public void Reset()
    {
        foreach (LineRenderer lineRenderer in LineRendererList)
        {
            lineRenderer.enabled = false;
        }
        
        matrixOfLevel.Reset();
        _gameGUIController.DisableControls();
        
        startLevelInput.enabled = true;
    }

    public void SetLineRenderer((int x, int y) coords, LineRenderer lineRenderer)
    {
        onStartGame?.Invoke();
        
        (_currentX, _currentY) = coords;
        matrixOfLevel[_currentX, _currentY].blockType = MatrixOfLevel.BlockAtMatrix.BlockType.BlockedByLine;
        
        _currentLineRenderer = lineRenderer;

        Vector3 pos = matrixOfLevel[_currentX, _currentY].transform.position + Vector3.back;
        _currentLineRenderer.SetPosition(0, pos);
        _currentLineRenderer.SetPosition(1, pos);
        _currentLineRenderer.enabled = true;
    }
}
