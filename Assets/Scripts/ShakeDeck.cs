using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShakeDeck : MonoBehaviour
{
    //shitty chatgpt code
    //
    //
    //
    public float shakeIntensity = 10f; // Maximum tilt angle in degrees
    public float shakeSpeed = 5f; // Speed of the shake

    private bool shaking = false;

    void OnMouseUpAsButton()
    {
        StartShaking();
    }

    public GameObject[] objectsToShake;


    public void StartShaking()
    {
        if (!shaking)
        {
            shaking = true;
            foreach (GameObject go in objectsToShake)
            {
                StartCoroutine(ShakeSingle(go));
            }
        }
    }

    private IEnumerator ShakeSingle(GameObject go)
    {
        Quaternion startRotation = go.transform.rotation;
        float elapsedTime = 0f;
        int direction = UnityEngine.Random.Range(0, 2);

        while (elapsedTime < (1f + (UnityEngine.Random.Range(1, 10) / 10f)))
        {
            float shakeAmount = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            if (direction == 1)
            {
                shakeAmount = shakeAmount * -1;
            }
            go.transform.rotation = startRotation * Quaternion.Euler(0f, 0f, shakeAmount);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        go.transform.rotation = startRotation;
        shaking = false;
    }
}
