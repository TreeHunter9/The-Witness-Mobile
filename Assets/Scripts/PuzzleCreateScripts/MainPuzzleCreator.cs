using UnityEngine;

using static MatrixOfLevel.BlockAtMatrix;

public class MainPuzzleCreator : MonoBehaviour
{
    [Header("Maps")]
    [SerializeField] private Texture2D _levelMapTexture;
    [SerializeField] private Texture2D _additionalLevelMapTexture;

    [Header("Materials")]
    [SerializeField] private Material _backgroundMaterial;
    [SerializeField] private Material _pathMaterial;
    [SerializeField] private Material _lineMaterial;
    
    [Header("Puzzle Blocks")]
    [SerializeField] private PuzzleBlock _pathBlock;
    [SerializeField] private PuzzleBlock _pathBlockWithoutCorner;   //правый нижний угол закруглён
    [SerializeField] private PuzzleBlock _backgroundBlock;
    [SerializeField] private PuzzleBlock _startBlock;
    [SerializeField] private PuzzleBlock _endBlock;

    [SerializeField] private MeshRenderer _backgroundWallRenderer;

    private GameObject _parentObj;
    private Vector3 _panelSize;
    private Vector3 _blockSize;
    
    private readonly Color _pathColor = Color.black;
    private readonly Color _startColor = Color.green;
    private readonly Color _endColor = Color.red;
    private const float AlphaBlockForMatrix = 0.5019608f;

    private Vector2 _extraLineOnLevelMapTexture = new Vector2(0f, 0f);
    
    private GameObject[,] _matrixOfGameObjects;
    private MatrixOfLevel _matrixOfLevel;

    private ConditionsOfFinish _conditionsOfFinish;
    private LevelsData _levelsData;

    private void Awake()
    {
        DestroyChildrens();
        
        _levelsData = GameObject.FindGameObjectWithTag("Information").GetComponent<LevelsData>();
        _levelsData.GetCurrentInformation(out _levelMapTexture, out _additionalLevelMapTexture,
            out _backgroundMaterial, out _pathMaterial, out _lineMaterial);
        
        _parentObj = new GameObject("LvlContainer");
        _parentObj.transform.position = transform.position;
        Vector3 panelMeshRendererSize = GetComponent<BoxCollider>().size;
        Vector3 blockMeshRendererSize = _backgroundBlock.blockGameObject.GetComponent<MeshRenderer>().bounds.size;
        Vector3 panelLossyScale = transform.lossyScale;
        _panelSize = new Vector3(panelMeshRendererSize.x * panelLossyScale.x,
            panelMeshRendererSize.y * panelLossyScale.y, panelMeshRendererSize.z * panelLossyScale.z);
        _blockSize = new Vector3(blockMeshRendererSize.x,
            blockMeshRendererSize.y, blockMeshRendererSize.z);

        _matrixOfGameObjects = new GameObject[_levelMapTexture.width, _levelMapTexture.height];
        _levelMapTexture.GetPixels32();

        ChangeMaterial(gameObject, _backgroundMaterial);

        _conditionsOfFinish = new ConditionsOfFinish();

        _matrixOfLevel = new MatrixOfLevel(_levelMapTexture);
        _matrixOfLevel.GenerateMatrix();

        LineController.singleton.matrixOfLevel = _matrixOfLevel;

        CreateLevel();
    }

    public void CreateWholeLevel()
    {
        DestroyChildrens();
        
        _parentObj = new GameObject("LvlContainer");
        _parentObj.transform.position = transform.position;
        Vector3 panelMeshRendererSize = GetComponent<BoxCollider>().size;
        Vector3 blockMeshRendererSize = _backgroundBlock.blockGameObject.GetComponent<MeshRenderer>().bounds.size;
        Vector3 panelLossyScale = transform.lossyScale;
        _panelSize = new Vector3(panelMeshRendererSize.x * panelLossyScale.x,
            panelMeshRendererSize.y * panelLossyScale.y, panelMeshRendererSize.z * panelLossyScale.z);
        _blockSize = new Vector3(blockMeshRendererSize.x,
            blockMeshRendererSize.y, blockMeshRendererSize.z);

        _matrixOfGameObjects = new GameObject[_levelMapTexture.width, _levelMapTexture.height];
        _levelMapTexture.GetPixels32();

        ChangeMaterial(gameObject, _backgroundMaterial);

        _conditionsOfFinish = new ConditionsOfFinish();

        _matrixOfLevel = new MatrixOfLevel(_levelMapTexture);
        _matrixOfLevel.GenerateMatrix();

        CreateLevel();
    }

    private void DestroyChildrens()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    
    private void CreateLevel()
    {
        float zAxisSpace = 0;
        float yAxisSpace = 0;
        for (int x = 0; x < _levelMapTexture.width; x++)
        {
            yAxisSpace = 0;
            for (int y = 0; y < _levelMapTexture.height; y++)
            {
                GenerateTile(x, y, zAxisSpace, yAxisSpace);
                yAxisSpace += _blockSize.z;
            }
            zAxisSpace += _blockSize.z;
        }

        _backgroundWallRenderer.material = _backgroundMaterial;

        GenerateAdditionalLevelMap();
                
        _matrixOfLevel.SaveOriginalMatrix();
        
        float scale = FillAllPanel(yAxisSpace, zAxisSpace);
        _parentObj.transform.rotation = transform.rotation;
        _parentObj.transform.parent = transform;

        _parentObj.transform.localPosition -= new Vector3(0f,
            (yAxisSpace - _extraLineOnLevelMapTexture.y - _blockSize.z * scale) * 0.5f * _parentObj.transform.localScale.y,
            (zAxisSpace - _extraLineOnLevelMapTexture.x - _blockSize.z * scale) * 0.5f * _parentObj.transform.localScale.z);
    }

    private void GenerateAdditionalLevelMap()
    {
        if (_additionalLevelMapTexture != null)
        {
            AdditionalPuzzleCreator additionalPuzzleCreatorScript = GetComponent<AdditionalPuzzleCreator>();
            additionalPuzzleCreatorScript.GenerateAdditionalLevelMap(_parentObj, _additionalLevelMapTexture,
                _matrixOfGameObjects, _matrixOfLevel, _conditionsOfFinish, _blockSize.z);
        }
    }

    private float FillAllPanel(float yAxisSpace, float zAxisSpace)
    {
        float yScale = _panelSize.y / yAxisSpace * 0.9f;
        float zScale = _panelSize.z / zAxisSpace * 0.9f;
        float scale = zScale < yScale ? zScale : yScale;
        
        _parentObj.transform.localScale = new Vector3(1f, scale, scale);
        return scale;
    }

    private void GenerateTile(int x, int y, float zAxisSpace, float yAxisSpace)
    {
        Color pixelColor = _levelMapTexture.GetPixel(x, y);

        //The pixel is transparent
        if (pixelColor.a == 0f)
        {
            _matrixOfLevel[x, y].blockType = BlockType.Blocked;
            _matrixOfGameObjects[x, y] = SpawnBlock(_backgroundBlock, zAxisSpace, yAxisSpace);
            ChangeMaterial(_matrixOfGameObjects[x, y], _backgroundMaterial);
            return;
        }
        else if (pixelColor == _pathColor)
        {
            if (CheckPathBlockWithoutCorner(x, y, out var angle) == true)
            {
                _matrixOfGameObjects[x, y] = SpawnBlock(_pathBlockWithoutCorner, zAxisSpace, yAxisSpace);
                LineRenderer lineRenderer = _matrixOfGameObjects[x, y].GetComponent<LineRenderer>();
                LineController.singleton.LineRendererList.Add(lineRenderer);
                _matrixOfLevel.SetTurnBlock(x, y, _matrixOfGameObjects[x, y].transform, 
                    lineRenderer);
                ChangeAngleOfBlock(_matrixOfGameObjects[x, y], angle);
                ChangeMaterial(_matrixOfGameObjects[x, y], _pathMaterial);
                ChangeLineRenderMaterial(_matrixOfGameObjects[x, y]);
                return;
            }

            _matrixOfGameObjects[x, y] = SpawnBlock(_pathBlock, zAxisSpace, yAxisSpace);
            if (CheckTurnBlock(x, y) == true)
            {
                LineRenderer lineRenderer = _matrixOfGameObjects[x, y].GetComponent<LineRenderer>();
                LineController.singleton.LineRendererList.Add(lineRenderer);
                _matrixOfLevel.SetTurnBlock(x, y, _matrixOfGameObjects[x, y].transform, 
                    lineRenderer);
                ChangeLineRenderMaterial(_matrixOfGameObjects[x, y]);
            }
        }
        else if (pixelColor == _startColor)
        {
            _matrixOfGameObjects[x, y] = SpawnBlock(_startBlock, zAxisSpace, yAxisSpace, 0.00001f);
            StartBlockPosition startBlockPosition = _matrixOfGameObjects[x, y].GetComponent<StartBlockPosition>();
            startBlockPosition.SetPositionAtMatrix(x, y);
            LineController.singleton.LineRendererList.Add(startBlockPosition.GetLineRenderer());
            ChangeLineRenderMaterial(startBlockPosition.GetLineRenderer().gameObject);
            ChangeLineRenderMaterial(_matrixOfGameObjects[x, y]);
            LineRenderer lineRenderer = _matrixOfGameObjects[x, y].GetComponent<LineRenderer>();
            LineController.singleton.LineRendererList.Add(lineRenderer);
            _matrixOfLevel.SetTurnBlock(x, y, _matrixOfGameObjects[x, y].transform, 
                lineRenderer);
        }
        else if (pixelColor == _endColor)
        {
            _matrixOfGameObjects[x, y] = SpawnBlock(_endBlock, zAxisSpace, yAxisSpace);
            TurnEndBlock(x, y);
            _matrixOfGameObjects[x, y].GetComponent<EndBlockTrigger>().Init(_conditionsOfFinish, _levelsData);
            _matrixOfLevel[x, y].blockType = BlockType.Finish;
            
            RemoveExtraLine(x, y);
        }
        else if (pixelColor.a == AlphaBlockForMatrix)
        {
            _matrixOfGameObjects[x, y] = SpawnBlock(_backgroundBlock, zAxisSpace, yAxisSpace);
            _matrixOfLevel[x, y].blockType = BlockType.BlockedByLine;
            ChangeMaterial(_matrixOfGameObjects[x, y], _backgroundMaterial);
            return;
        }

        _matrixOfLevel[x, y].transform = _matrixOfGameObjects[x, y].transform;
        ChangeMaterial(_matrixOfGameObjects[x, y], _pathMaterial);
    }

    private GameObject SpawnBlock(PuzzleBlock block, float zAxisSpace, float yAxisSpace, float additionalParameter = 0f)
    {
        Vector3 pos = new Vector3(_parentObj.transform.position.x + _panelSize.x / 2f + 0.0001f + additionalParameter, 
            _parentObj.transform.position.y + yAxisSpace,
            _parentObj.transform.position.z + zAxisSpace);
        return Instantiate(block.blockGameObject, pos, block.transform.rotation, _parentObj.transform);
    }

    private void ChangeMaterial(GameObject obj, Material material)
    {
        obj.GetComponent<Renderer>().material = material;
    }

    private void ChangeLineRenderMaterial(GameObject obj)
    {
        obj.GetComponent<LineRenderer>().material = _lineMaterial;
    }

    private void RemoveExtraLine(int x, int y)
    {
        for (int i = -1; i < 2; i += 2)
        {
            if (_levelMapTexture.GetPixel(x + i, y) == _pathColor) 
                _extraLineOnLevelMapTexture.x = _blockSize.z;
        }
        
        for (int i = -1; i < 2; i += 2)
        {
            if (_levelMapTexture.GetPixel(x, y + i) == _pathColor)
                _extraLineOnLevelMapTexture.y = _blockSize.z;
        }
    }

    private bool CheckTurnBlock(int x, int y)
    {
        if (_levelMapTexture.GetPixel(x - 1, y).a != 0f && 
            (_levelMapTexture.GetPixel(x, y - 1).a != 0f ||
        _levelMapTexture.GetPixel(x, y + 1).a != 0f))
        {
            return true;
        }
        return _levelMapTexture.GetPixel(x + 1, y).a != 0f && 
               (_levelMapTexture.GetPixel(x, y - 1).a != 0f ||
                _levelMapTexture.GetPixel(x, y + 1).a != 0f);
    }

    private bool CheckPathBlockWithoutCorner(int x, int y, out float angle)
    {
        if (_levelMapTexture.GetPixel(x + 1, y).a == 0f && _levelMapTexture.GetPixel(x, y - 1).a == 0f
        && _levelMapTexture.GetPixel(x - 1, y).a != 0f && _levelMapTexture.GetPixel(x, y + 1).a != 0f )
        {
            angle = 0f;
            return true;
        }
        if (_levelMapTexture.GetPixel(x, y + 1).a == 0f && _levelMapTexture.GetPixel(x + 1, y).a == 0f
        && _levelMapTexture.GetPixel(x - 1, y).a != 0f && _levelMapTexture.GetPixel(x, y - 1).a != 0f)
        {

            angle = 90f;
            return true;
        }
        if (_levelMapTexture.GetPixel(x, y + 1).a == 0f && _levelMapTexture.GetPixel(x - 1, y).a == 0f 
        && _levelMapTexture.GetPixel(x + 1, y).a != 0f && _levelMapTexture.GetPixel(x, y - 1).a != 0f)
        {
            angle = 180f;
            return true;
        }
        if (_levelMapTexture.GetPixel(x, y - 1).a == 0f && _levelMapTexture.GetPixel(x - 1, y).a == 0f
        && _levelMapTexture.GetPixel(x + 1, y).a != 0f && _levelMapTexture.GetPixel(x, y + 1).a != 0f)
        {
            angle = 270f;
            return true;
        }

        angle = 0f;
        return false;
    }

    private void ChangeAngleOfBlock(GameObject block, float angle)
    {
        block.transform.Rotate(Vector3.forward, angle, Space.Self);
    }

    private void TurnEndBlock(int x, int y)
    {
        if (_levelMapTexture.GetPixel(x, y - 1) == _pathColor)
        {
            ChangeAngleOfBlock(_matrixOfGameObjects[x, y], 0f);
        }
        else if (_levelMapTexture.GetPixel(x - 1, y) == _pathColor)
        {
            ChangeAngleOfBlock(_matrixOfGameObjects[x, y], 90f);
        }
        else if (_levelMapTexture.GetPixel(x, y + 1) == _pathColor)
        {
            ChangeAngleOfBlock(_matrixOfGameObjects[x, y], 180f);
        }
        else if (_levelMapTexture.GetPixel(x + 1, y) == _pathColor)
        {
            ChangeAngleOfBlock(_matrixOfGameObjects[x, y], 270f);
        }
    }
}
