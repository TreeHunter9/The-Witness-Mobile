using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MatrixOfLevel.BlockAtMatrix;

public class ColoredSquareCondition : ColoredCondition
{
    private BlockType _ownBlockType = BlockType.ColoredSquare;

    private void OnEnable()
    {
        LineController.singleton.onStartGame += base.StopPingPongColor;
        LineController.singleton.onStartGame += base.Reset;
    }

    private void OnDisable()
    {
        LineController.singleton.onStartGame -= base.StopPingPongColor;
        LineController.singleton.onStartGame -= base.Reset;
    }

    protected override bool CheckBlockTypeAndColor(MatrixOfLevel.BlockAtMatrix block)
    {
        if (block.blockType == _ownBlockType && block.blockColor != ownBlockColor)
            return false;
        return true;
    }
}
