using UnityEngine;
using static MatrixOfLevel.BlockAtMatrix;

[RequireComponent(typeof(MainPuzzleCreator))]
public class AdditionalPuzzleCreator : MonoBehaviour
{
    [Header("Puzzle Blocks")]
    [SerializeField] private PuzzleBlock _blackPointBlock;
    [SerializeField] private PuzzleBlock _squareBlock;
    [SerializeField] private PuzzleBlock _starBlock;

    private Texture2D _tempLevelMapTexture;

    private readonly Color _blackPointColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1);  //Юнити кал!!!
    private const float StarAlphaChanel = 0.5019608f;

    private float _blockSize;
    
    private GameObject _parentObj;
    private GameObject _tempObj;
    private GameObject[,] _matrixOfBlocks;
    private MatrixOfLevel _matrixOfLevel;

    private ConditionsOfFinish _conditionsOfFinish;

    public void GenerateAdditionalLevelMap(GameObject parentObj, Texture2D additionalMap, GameObject[,] matrixOfBlocks, 
        MatrixOfLevel matrixOfLevel, ConditionsOfFinish conditionsOfFinish, float blockSize)
    {
        _parentObj = parentObj;
        _matrixOfBlocks = matrixOfBlocks;
        _matrixOfLevel = matrixOfLevel;
        _conditionsOfFinish = conditionsOfFinish;
        _blockSize = blockSize;
        
        _tempLevelMapTexture = new Texture2D(additionalMap.width, additionalMap.height);
        _tempLevelMapTexture.SetPixels(additionalMap.GetPixels());

        float zAxisSpace = 0;
        float yAxisSpace = 0;
        for (int x = 0; x < _tempLevelMapTexture.width; x++)
        {
            yAxisSpace = 0;
            for (int y = 0; y < _tempLevelMapTexture.height; y++)
            {
                GenerateTile(x, y, zAxisSpace, yAxisSpace);
                yAxisSpace += blockSize;
            }
            zAxisSpace += blockSize;
        }
    }

    private void GenerateTile(int x, int y, float zAxisSpace, float yAxisSpace)
    {
        Color pixelColor = _tempLevelMapTexture.GetPixel(x, y);
        
        if (pixelColor.a == 0f)
        {
            return;
        }
        else if (pixelColor == _blackPointColor)
        {
            SetCondition(x, y, _blackPointBlock, _blackPointColor);
            _tempObj.GetComponent<BlackPointTrigger>().SetBlockAtMatrix(_matrixOfLevel[x, y]);
        }
        else if (pixelColor.a == 1f)
        {
            _matrixOfLevel.ConvertToMatrix(x, y, BlockType.ColoredSquare, pixelColor);
            
            SetCondition(x, y, _squareBlock, pixelColor);
            SaveMatrixPosition(x, y, pixelColor);
            ChangeScale(x, y);
        }
        else if (pixelColor.a == StarAlphaChanel)
        {
            _matrixOfLevel.ConvertToMatrix(x, y, BlockType.ColoredStar, pixelColor);
            
            SetCondition(x, y, _starBlock, pixelColor);
            SaveMatrixPosition(x, y, pixelColor);
            ChangeScale(x, y);
        }
    }

    private GameObject SpawnBlock(PuzzleBlock block, Vector3 position)
    {
        position.x += 0.0001f;
        return Instantiate(block.blockGameObject, position, block.transform.rotation, _parentObj.transform);
    }

    private void SetCondition(int x, int y, PuzzleBlock block, Color color)
    {
        _tempObj = SpawnBlock(block, _matrixOfBlocks[x, y].transform.position);
        Condition conditionScript = _tempObj.GetComponent<Condition>();
        _conditionsOfFinish.ListAdd(conditionScript);
        
        ChangeColor(color, conditionScript);
    }

    private void SaveMatrixPosition(int x, int y, Color color)
    {
        ColoredCondition coloredConditionScript = _tempObj.GetComponent<ColoredCondition>();
        coloredConditionScript.SetOwnBlockColor(color);
        coloredConditionScript.SetMatrixOfConditions(_matrixOfLevel, _conditionsOfFinish);
        coloredConditionScript.SetPositionAtMatrix(x, y);
    }

    private void ChangeColor(Color color, Condition script)
    {
        if (script is ColoredCondition)
        {
            _tempObj.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 1f);
        }
    }

    private void ChangeScale(int x, int y)
    {
        int xCount = 1;
        int yCount = 1;
        
        int tempX = x;

        Color color = _tempLevelMapTexture.GetPixel(tempX, y);
        while (true)
        {
            tempX++;
            if (_tempLevelMapTexture.GetPixel(tempX, y) == color)
            {
                xCount++;
                _tempLevelMapTexture.SetPixel(tempX, y, Color.clear);
            }
            else
                break;
        }

        while (true)
        {
            y++;
            if (_tempLevelMapTexture.GetPixel(x, y) == color)
            {
                yCount++;
                _tempLevelMapTexture.SetPixel(x, y, Color.clear);
            }
            else
                break;
        }

        _tempObj.transform.localScale = new Vector3(_tempObj.transform.localScale.x * xCount,
            _tempObj.transform.localScale.y * yCount,
            _tempObj.transform.localScale.z);
        _tempObj.transform.localPosition += new Vector3(0f, 
            _blockSize * ((yCount - 1) / 2f), 
            _blockSize * ((xCount - 1) / 2f));
    }
}
