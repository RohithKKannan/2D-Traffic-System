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

        public UIManager UIManager => uIManager;
        public Graph Graph => graph;
        public CarManager CarManager => carManager;
    }
}
