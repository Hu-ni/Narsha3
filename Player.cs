
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject character;
    public GameObject viewGraphic;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = gameObject.GetComponent<PlayerController>();
    }
}
