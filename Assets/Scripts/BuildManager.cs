using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS
{
    public class BuildManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private RoadBuilder roadBuilder;

        private bool buildModeEnabled;

        private void Awake()
        {
            gameManager.UIManager.BuildRoadButton.onClick.AddListener(ToggleBuildMode);
        }

        private void OnDestroy()
        {
            gameManager.UIManager.BuildRoadButton.onClick.RemoveListener(ToggleBuildMode);
        }

        public void ToggleBuildMode()
        {
            if (buildModeEnabled)
                DisableBuildMode();
            else
                EnableBuildMode();

            buildModeEnabled = !buildModeEnabled;
        }

        private void EnableBuildMode()
        {
            Cursor.visible = false;
            gridManager.EnableHoverIndicator();

            roadBuilder.enabled = true;
        }

        private void DisableBuildMode()
        {
            Cursor.visible = true;
            gridManager.DisableHoverIndicator();

            roadBuilder.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && buildModeEnabled)
            {
                ToggleBuildMode();
            }
        }
    }
}
