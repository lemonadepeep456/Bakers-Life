using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform[] spawnPoints;

    private bool[] occupied;

    public float spawnTime = 5f;

    void Start()
    {
        occupied = new bool[spawnPoints.Length];

        InvokeRepeating(nameof(SpawnCustomer), 1f, spawnTime);
    }

    void SpawnCustomer()
    {
        int point;

        do
        {
            point = Random.Range(0, spawnPoints.Length);
        }
        while (occupied[point]);

        occupied[point] = true;

        GameObject customer = Instantiate(
            customerPrefab,
            spawnPoints[point].position,
            Quaternion.identity
        );

        customer.GetComponent<Customer>().spawnIndex = point;
    }
    public void ClearSpot(int index)
    {
        occupied[index] = false;

    }
}