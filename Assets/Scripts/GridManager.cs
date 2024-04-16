using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private Grid grid;
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private Transform hoverIndicator;

        private Vector3 mouseWorldPos;

        public void EnableHoverIndicator()
        {
            hoverIndicator.gameObject.SetActive(true);
        }

        public void DisableHoverIndicator()
        {
            hoverIndicator.gameObject.SetActive(false);
        }

        private void Update()
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int gridX = Mathf.FloorToInt(mouseWorldPos.x / grid.cellSize.x);
            int gridY = Mathf.FloorToInt(mouseWorldPos.y / grid.cellSize.y);

            gridX = Mathf.Clamp(gridX, -(gridSize.x - 1), gridSize.x - 1);
            gridY = Mathf.Clamp(gridY, -(gridSize.y - 1), gridSize.y - 1);

            hoverIndicator.position = GetCellCenter(gridX, gridY);
        }

        public Vector3 GetCellCenter(int gridX, int gridY)
        {
            float x = gridX * grid.cellSize.x + grid.cellSize.x / 2f;
            float y = gridY * grid.cellSize.y + grid.cellSize.y / 2f;

            return new Vector3(x, y, 0f);
        }
    }
}
