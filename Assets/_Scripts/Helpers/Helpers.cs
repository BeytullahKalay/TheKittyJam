using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Pandoras.Helper
{
    public static class Helpers
    {
        private static Camera _camera;

        public static Camera MainCamera
        {
            get
            {
                if (_camera == null) _camera = Camera.main;
                return _camera;
            }
        }

        /// <summary>
        ///  Returns world position of pointer.
        /// </summary>
        public static Vector2 GetWorldPositionOfPointer(Camera rayCam)
        {
            var pos = rayCam.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(pos.x, pos.y);
        }


        /// <summary>
        ///  Returns world position of canvas element.
        /// </summary>
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element, Camera camera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, camera, out var result);
            return result;
        }


        /// <summary>
        ///  Returns screen position of world position.
        /// </summary>
        public static Vector2 GetScreenPositionOfWorldPosition(RectTransform canvasRectTransform, Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, null, out var result);
            return result;
        }

        #region Mouse Over UI

        /// <summary>
        ///Returns 'true' if pointer touched or hovering on Unity UI element.
        /// </summary>
        public static bool IsPointerOverUIElement(LayerMask searchUILayer = default)
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults(), searchUILayer);
        }


        //Returns 'true' if we touched or hovering on Unity UI element.
        private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults, LayerMask searchLayer)
        {
            for (int index = 0; index < eventSystemRaycastResults.Count; index++)
            {
                var curRaycastResult = eventSystemRaycastResults[index];
                if (curRaycastResult.gameObject.layer == searchLayer)
                    return true;
            }

            return false;
        }


        //Gets all event system raycast results of current mouse or touch position.
        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults;
        }

        #endregion


        /// <summary>
        /// Set sprite renderer color alpha
        /// </summary>
        public static SpriteRenderer SetColorAlpha(this SpriteRenderer renderer, float alphaValue)
        {
            var c = renderer.color;
            c.a = alphaValue;
            renderer.color = c;
            return renderer;
        }

        /// <summary>
        /// Set TMP_Text color alpha
        /// </summary>
        public static TMP_Text SetColorAlpha(this TMP_Text renderer, float alphaValue)
        {
            var c = renderer.color;
            c.a = alphaValue;
            renderer.color = c;
            return renderer;
        }

        /// <summary>
        /// Get mouse position for given tilemap parameter
        /// </summary>
        public static Vector3Int GetMousePositionOnTilemap(Tilemap tilemap, Camera cam)
        {
            var mousePosVec2 = GetWorldPositionOfPointer(cam);


            //var tileAnchor = tilemap.tileAnchor;
            //var mousePosVector3 = new Vector3(mousePosVec2.x, mousePosVec2.y, 0) - new Vector3(tileAnchor.x, tileAnchor.y, 0);
            //return Vector3Int.RoundToInt(mousePosVector3);

            var mousePosVector3 = new Vector3(mousePosVec2.x, mousePosVec2.y, 0);

            var gridPosition = tilemap.WorldToCell(mousePosVector3);

            return gridPosition;
        }


        /// <summary>
        /// A shorter way of testing if a game object has a component
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="obj">The object to check on</param>
        /// <returns></returns>
        public static bool Has<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T>() != null;
        }

        public static List<Transform> GetAllChildren(this Transform transform)
        {
            var children = new List<Transform>();

            foreach (Transform child in transform)
            {
                children.Add(child);
            }

            return children;
        }


        /// <summary>
        ///  Get angle for given Vector3 direction
        /// </summary>
        public static float GetAngleFromVectorFloatForRotZ(Vector3 dir)
        {
            dir = dir.normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
            return angle;
        }

        /// <summary>
        ///  Get angle for given Vector3 direction
        /// </summary>
        public static float GetAngleFromVectorFloatForRotY(Vector3 dir)
        {
            var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
            return angle;
        }


        /// <summary>
        ///  Remapping given float to desired number range
        ///  Debug.Log(2.Remap(1, 3, 0, 10));    // 5
        /// </summary>
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}