using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StartBlockPosition : MonoBehaviour
{
    private int _x;
    private int _y;

    [SerializeField] private LineRenderer lineRendererStart;

    private async UniTask StartAnimation()
    {
        lineRendererStart.enabled = true;
        for (float i = 0.5f; i <= 1f; i += 0.05f)
        {
            lineRendererStart.startWidth = i;
            await UniTask.Delay(10);
        }
    }

    public void SetPositionAtMatrix(int x, int y) => (_x, _y) = (x, y);
    public (int x, int y) GetPositionAtMatrix()
    {
        StartAnimation();
        return (_x, _y);
    }

    public LineRenderer GetLineRenderer() => lineRendererStart;
}
