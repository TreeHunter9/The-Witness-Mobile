using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuGUIController : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _selectLevelMenu;
    [SerializeField] private TextMeshProUGUI _startGameButton;

    private LevelsData _levelsData;

    private void Awake()
    {
        _levelsData = GameObject.FindGameObjectWithTag("Information").GetComponent<LevelsData>();
        _startGameButton.text = _levelsData.GetLastID() != 0 ? "Continue" : "Start";
    }

    public void StartGame()
    {
        _levelsData.CurrentID = _levelsData.GetLastID();
        if (_levelsData.CurrentID == 0)
        {
            SceneManager.LoadScene(2);
            return;
        }
        SceneManager.LoadScene(1);
    }
    
    public void ToSelectLevelMenu()
    {
        _mainMenu.SetActive(false);
        _selectLevelMenu.SetActive(true);
    }

    public void ToMainMenu()
    {
        _mainMenu.SetActive(true);
        _selectLevelMenu.SetActive(false);
    }
}
