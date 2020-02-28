﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D)), DisallowMultipleComponent]
public class Deadzone : MonoBehaviour
{
    public LayerMask destroyMask;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (destroyMask == (destroyMask | (1 << other.gameObject.layer)))
        {
            Debug.Log("POP");
            Destroy(other.gameObject);
        }

        Debug.Log("POW");
    }
}