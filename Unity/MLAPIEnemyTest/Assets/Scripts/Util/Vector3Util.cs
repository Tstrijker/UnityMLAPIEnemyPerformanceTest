using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Util
{
    public static Vector3 GetRandomVector3(this Bounds rangeBounds)
    {
        return new Vector3(
            Random.Range(rangeBounds.min.x, rangeBounds.max.x),
            Random.Range(rangeBounds.min.y, rangeBounds.max.y),
            Random.Range(rangeBounds.min.z, rangeBounds.max.z));
    }
}
