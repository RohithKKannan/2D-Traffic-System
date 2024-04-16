using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UIManager uIManager;
        [SerializeField] private Graph graph;
        [SerializeField] private CarManager carManager;
        [SerializeField] private BuildManager buildManager;

        public UIManager UIManager => uIManager;
        public Graph Graph => graph;
        public CarManager CarManager => carManager;
        public BuildManager BuildManager => buildManager;
    }
}
