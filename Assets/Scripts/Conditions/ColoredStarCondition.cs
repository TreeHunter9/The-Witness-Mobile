using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MatrixOfLevel.BlockAtMatrix;

public class ColoredStarCondition : ColoredCondition
{
    private BlockType _ownBlockType = BlockType.ColoredStar;
    private int _count = 0;
    
    private void OnEnable()
    {
        LineController.singleton.onStartGame += base.StopPingPongColor;
        LineController.singleton.onStartGame += base.Reset;
        LineController.singleton.onStartGame += ResetCount;
    }

    private void OnDisable()
    {
        LineController.singleton.onStartGame -= base.StopPingPongColor;
        LineController.singleton.onStartGame -= base.Reset;
        LineController.singleton.onStartGame -= ResetCount;
    }

    private void ResetCount() => _count = 0;

    protected override bool FinalCheck()
    {
        return _count == 1;
    }

    protected override bool CheckBlockTypeAndColor(MatrixOfLevel.BlockAtMatrix block)
    {
        if ((block.blockType == _ownBlockType && block.blockColor == base.ownBlockColor)
            || (block.blockType == BlockType.ColoredSquare
                && block.blockColor.grayscale == base.ownBlockColor.grayscale)) //grayscale не учитывает alpha
        {
            _count++;
        }
        return _count < 2;
    }
}
