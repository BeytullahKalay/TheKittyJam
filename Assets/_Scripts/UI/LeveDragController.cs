using UnityEngine;
using UnityEngine.EventSystems;


namespace MenuScripts
{
    public class LeveDragController : MonoBehaviour
    {
        [SerializeField] private LevelButtonController levelButtonController;
        [SerializeField] private float dragSpeed = 1;
        [SerializeField] private float dragLerpSpeed = 3;
        private float _lastMousePosY;
        private float _mouseChangeDelta;

        private Transform _firstLevelButtonObjectTransform;
        private Transform _lastLevelButtonObjectTransform;
        private Transform _currentLevelButtonObjTransform;

        private Vector3 _movePos = Vector3.zero;

        private bool _mouseInputStart = false;
        private bool _startInput;

        private Vector3 _lastPos;

        private void Start()
        {

            _firstLevelButtonObjectTransform = levelButtonController.GetFirstLevelButtonObjectTransform();
            _lastLevelButtonObjectTransform = levelButtonController.GetLastLevelButtonObjectTransform();
            _currentLevelButtonObjTransform = levelButtonController.GetLevelButtonObjWorldPos(PlayerPrefs.GetInt(PlayerPrefNames.Level, 0));

            var pos = transform.position;
            pos.z -= _currentLevelButtonObjTransform.position.z;

            transform.position = pos;
            _lastPos = pos;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            _lastMousePosY = Input.mousePosition.y;
            _mouseInputStart = true;
            _startInput = true;
        }

        private void OnMouseDrag()
        {
            if (!_mouseInputStart) return;

            _mouseChangeDelta = Input.mousePosition.y - _lastMousePosY;
            _lastMousePosY = Input.mousePosition.y;
            var pos = transform.position;
            pos.z += _mouseChangeDelta * dragSpeed * Time.deltaTime;
            _movePos = pos;
        }

        private void OnMouseUp()
        {
            _mouseInputStart = false;
        }

        private void Update()
        {
            if (!_startInput) return;

            var firstLevelWorldToScreenPos = Camera.main.WorldToViewportPoint(_firstLevelButtonObjectTransform.position);
            var lastLevelWorldToScreenPos = Camera.main.WorldToViewportPoint(_lastLevelButtonObjectTransform.position);

            // clamp scrolling action
            if (firstLevelWorldToScreenPos.y > .5f)
            {
                _movePos.z -= 1;
                transform.position = _lastPos;
            }
            if (lastLevelWorldToScreenPos.y < .5f)
            {
                _movePos.z += 1;
                transform.position = _lastPos;
            }

            _lastPos = transform.position;

            transform.position = Vector3.Lerp(transform.position, _movePos, Time.deltaTime * dragLerpSpeed);
        }

    }
}
