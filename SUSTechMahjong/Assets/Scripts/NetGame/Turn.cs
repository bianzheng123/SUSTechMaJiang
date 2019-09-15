using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour {
    [SerializeField]
    private bool isStart = false;
    [SerializeField]
    private int lightOnIndex = 0;
    [SerializeField]
    private GameObject[] son;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartTurnOn();
            Debug.Log("start");
        }
        if (isStart)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeLight();
            }
        }
    }
    private void StartTurnOn()
    {
        isStart = true;
        son[0].SetActive(true);
    }
    private void ChangeLight()
    {
        son[lightOnIndex].SetActive(false);
        lightOnIndex = (lightOnIndex + 1) % 4;
        son[lightOnIndex].SetActive(true);
        Debug.Log("changeLight");
    }
}
