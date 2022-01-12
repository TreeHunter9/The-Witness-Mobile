public class BlackPointTrigger : Condition
{
    private MatrixOfLevel.BlockAtMatrix _blockAtMatrix;

    public void SetBlockAtMatrix(MatrixOfLevel.BlockAtMatrix blockAtMatrix) => _blockAtMatrix = blockAtMatrix;

    private void OnEnable()
    {
        LineController.singleton.onStartGame += base.StopPingPongColor;
    }

    private void OnDisable()
    {
        LineController.singleton.onStartGame -= base.StopPingPongColor;
    }

    public override bool CheckCondition() =>
        _blockAtMatrix.blockType == MatrixOfLevel.BlockAtMatrix.BlockType.BlockedByLine;
}
