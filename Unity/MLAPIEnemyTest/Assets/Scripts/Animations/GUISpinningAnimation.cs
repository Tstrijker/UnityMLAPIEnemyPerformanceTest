using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUISpinningAnimation : MonoBehaviour
{
    [SerializeField] private float spinningSpeed = default;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, spinningSpeed * Time.deltaTime));
    }
}
