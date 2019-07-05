using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Character character;

    public GameObject viualGraphic;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = gameObject.GetComponent<PlayerController>();
    }
}
