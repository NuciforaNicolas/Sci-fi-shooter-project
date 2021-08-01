using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static void Move(this Transform trans, float speed)
    {
        trans.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
