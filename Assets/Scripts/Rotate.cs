using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float speed; 

    void Update()
    {
        float rotation = transform.eulerAngles.z;

        rotation += Time.deltaTime * speed;

        if (rotation >= 360)
        {
            rotation -= 360; 
        }
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

}
