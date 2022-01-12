using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToLevel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private int _id;
    [SerializeField] private Sprite _levelPreviewSprite;
    [SerializeField] private Sprite _levelCloseSprite;

    [SerializeField] private Image _levelImage;

    private LevelsData _levelsData;
    private bool _levelIsAvailable;

    private void Awake()
    {
        _levelsData = GameObject.FindGameObjectWithTag("Information").GetComponent<LevelsData>();
    }

    private void Start()
    {
        _levelIsAvailable = _levelsData.CheckAvailableByID(_id);
        _levelImage.sprite = _levelIsAvailable ? _levelPreviewSprite : _levelCloseSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.dragging == true)
            return;
        
        if (_levelIsAvailable == false)
            return;
        _levelsData.CurrentID = _id;
        if (_id == 0)
        {
            SceneManager.LoadScene(2);
            return;
        }
        SceneManager.LoadScene(1);
    }

    public void OnPointerDown(PointerEventData eventData) { }
}
