using UnityEngine;
using UnityEngine.EventSystems;

public class LineMovement : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Vector2Int _direction;

    private LineController _lineController;

    private void Awake()
    {
        _lineController = LineController.singleton;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _lineController.MoveLine(_direction);
    }
}
