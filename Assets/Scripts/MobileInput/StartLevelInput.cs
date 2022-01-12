using ExtensionMethods;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace MobileInput
{
    public class StartLevelInput : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;
        [Space]
        [SerializeField] private GameGUIController _GUIController;

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    StartLevel(touch.position);
                }
            }
            #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                StartLevel(Input.mousePosition);
            }
            #endif
        }
            

        private void StartLevel(Vector2 posTouch)
        {
            Ray ray = _camera.ScreenPointToRay(posTouch.ToVector3());
            if (Physics.Raycast(ray, out var hit, 10, _layerMask))
            {
                LineController.singleton.SetLineRenderer(
                    hit.transform.GetComponent<StartBlockPosition>().GetPositionAtMatrix(),
                    hit.transform.GetComponent<LineRenderer>());
                _GUIController.EnableControls();
                this.enabled = false;
            }
        }
    }
}
