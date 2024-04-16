using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TS
{
    public class RoadBuilder : MonoBehaviour
    {
        [SerializeField] private BuildManager buildManager;

        private Vector3 mousePositionOnClick;

        private Vector3 roadStartPoint;
        private Vector3 roadEndPoint;

        private UnityAction OnMouseClick;

        private void OnEnable()
        {
            mousePositionOnClick = Vector3.zero;
            roadStartPoint = Vector3.zero;
            roadEndPoint = Vector3.zero;

            OnMouseClick += GetStartPoint;
        }

        private void OnDisable()
        {
            OnMouseClick -= GetStartPoint;
            OnMouseClick -= GetEndPoint;
        }

        private void GetStartPoint()
        {
            roadStartPoint = mousePositionOnClick;
            OnMouseClick -= GetStartPoint;
            OnMouseClick += GetEndPoint;

            Debug.Log("Start point : " + roadStartPoint);
        }

        private void GetEndPoint()
        {
            roadEndPoint = mousePositionOnClick;
            OnMouseClick -= GetEndPoint;

            Debug.Log("End point : " + roadEndPoint);

            buildManager.ToggleBuildMode();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                mousePositionOnClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnMouseClick?.Invoke();
            }
        }
    }
}
