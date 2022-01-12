using System;
using UnityEngine;

public class LevelsData : MonoBehaviour
{
    [SerializeField] private Information[] _informationAboutLevels;
    private int _currentID;
    private int _lastID;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Information");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        _lastID = PlayerPrefs.GetInt("lastID", 0);
    }

    private void Start()
    {
        OpenLevels();
    }

    public int GetLastID() => _lastID;

    public int CurrentID
    {
        get
        {
            return _currentID;
        }
        set
        {
            if (value >= _informationAboutLevels.Length) 
                return;
            if (value > _lastID)
            {
                _lastID = value;
                PlayerPrefs.SetInt("lastID", _lastID);
            }

            _currentID = value;
        }
    }

    public bool CheckAvailableByID(int id) => _informationAboutLevels[id].IsAvailable;

    public void GetCurrentInformation(out Texture2D levelMap, out Texture2D additionalLevelMap,
        out Material backgroundMaterial,
        out Material pathMaterial, out Material lineMaterial) =>
        _informationAboutLevels[_currentID].GetInformation(
            out levelMap, out additionalLevelMap,
            out backgroundMaterial,
            out pathMaterial, out lineMaterial);

    public void LevelComplete()
    {
        _informationAboutLevels[CurrentID].IsComplete = true;
        CurrentID++;
        if (CurrentID != _informationAboutLevels.Length)
        {
            _informationAboutLevels[CurrentID].IsAvailable = true;
        }
    }

    private void OpenLevels()
    {
        _informationAboutLevels[0].IsAvailable = true;
        for (int id = 0; id < _lastID; id++)
        {
            _informationAboutLevels[id].IsComplete = true;
            if (id + 1 != _informationAboutLevels.Length)
                _informationAboutLevels[id + 1].IsAvailable = true;
        }
    }
}

[Serializable]
public class Information
{
    [Header("Maps")]
    [SerializeField] private Texture2D _levelMapTexture;
    [SerializeField] private Texture2D _additionalLevelMapTexture;

    [Header("Materials")]
    [SerializeField] private Material _backgroundMaterial;
    [SerializeField] private Material _pathMaterial;
    [SerializeField] private Material _lineMaterial;

    public bool IsComplete { get; set; } = false;
    public bool IsAvailable { get; set; } = false;
    
    public void GetInformation(out Texture2D levelMap, out Texture2D additionalLevelMap,
        out Material backgroundMaterial,
        out Material pathMaterial, out Material lineMaterial)
    {
        levelMap = _levelMapTexture;
        additionalLevelMap = _additionalLevelMapTexture;
        backgroundMaterial = _backgroundMaterial;
        pathMaterial = _pathMaterial;
        lineMaterial = _lineMaterial;
    }
}
