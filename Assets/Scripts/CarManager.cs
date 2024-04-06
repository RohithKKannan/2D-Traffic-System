using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS
{
    public class CarManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CarController carPrefab;
        [SerializeField] private Transform carParent;

        private List<CarController> cars = new();

        public GameManager GameManager => gameManager;

        private void Awake()
        {
            gameManager.UIManager.SpawnCarButton.onClick.AddListener(SpawnCar);
            gameManager.UIManager.RemoveCarButton.onClick.AddListener(RemoveCar);
        }

        private void OnDestroy()
        {
            gameManager.UIManager.SpawnCarButton.onClick.RemoveListener(SpawnCar);
            gameManager.UIManager.RemoveCarButton.onClick.RemoveListener(RemoveCar);
        }

        private void SpawnCar()
        {
            if (gameManager.Graph.IsGraphEmpty())
                return;

            CarController newCar = GameObject.Instantiate<CarController>(carPrefab, carParent);
            Debug.Log("Instantiating car!");

            cars.Add(newCar);

            newCar.SetCarManager(this);
            newCar.BeginRandomJourney();
        }

        private void RemoveCar()
        {
            if (cars.Count == 0)
                return;

            CarController car = cars[cars.Count - 1];
            cars.Remove(car);
            GameObject.Destroy(car.gameObject);
        }

        public void RemoveCar(CarController _car)
        {
            if (cars.Count == 0)
                return;

            cars.Remove(_car);
            GameObject.Destroy(_car.gameObject);
        }
    }
}
