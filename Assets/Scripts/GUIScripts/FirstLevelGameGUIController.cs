using System.Collections;
using MobileInput;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstLevelGameGUIController : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _controls;
    [SerializeField] private GameObject _completeLevelMenu;
    [SerializeField] private Image _backgroundResetImage;
    [SerializeField] private AnimationCurve _backgroundAnimationCurve;
    [Space] 
    [SerializeField] private LineController _lineController;
    [SerializeField] private StartLevelInput startLevelInput;

    private bool _startLevelInputActive = true;

    public void MenuVisible()
    {
        _menu.SetActive(!_menu.activeSelf);
        _startLevelInputActive = startLevelInput.enabled;
        print(_startLevelInputActive);

        if (_menu.activeSelf == true) 
            startLevelInput.enabled = false;
        else
            startLevelInput.enabled = _startLevelInputActive;
    }

    public void CloseMenu()
    {
        _menu.SetActive(false);
        startLevelInput.enabled = _startLevelInputActive;
    }

    public void EnableControls() => _controls.SetActive(true);
    public void DisableControls() => _controls.SetActive(false);

    public void Reset() => StartCoroutine(ResetWithBackground());

    private IEnumerator ResetWithBackground()
    {
        _backgroundResetImage.gameObject.SetActive(true);
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            _backgroundResetImage.color = new Color(0, 0, 0, _backgroundAnimationCurve.Evaluate(i));
            if (i >= 0.5f && i <= 0.501f)
                _lineController.Reset();
            yield return new WaitForSeconds(0.01f);
        }
        _backgroundResetImage.gameObject.SetActive(false);
    }

    public void OpenCompleteLevelMenu() => _completeLevelMenu.SetActive(true);

    public void NextLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
