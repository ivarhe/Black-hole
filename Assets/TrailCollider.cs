using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailCollider : MonoBehaviour
{
    TrailRenderer trail;
    EdgeCollider2D edgeCollider;

    static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        GameObject colliderObject = new GameObject("TrailCollider", typeof(EdgeCollider2D));
        edgeCollider = GetValidCollider();
    }

    EdgeCollider2D GetValidCollider()
    {
        EdgeCollider2D validCollider;
        if (unusedColliders.Count > 0)
        {
            validCollider = unusedColliders[0];
            validCollider.enabled = true;
            unusedColliders.RemoveAt(0);
        }
        else
        {
            GameObject colliderObject = new GameObject("TrailCollider", typeof(EdgeCollider2D));
            validCollider = colliderObject.GetComponent<EdgeCollider2D>();
        }
        validCollider.tag = "hand";
        return validCollider;

    }

    void setColliderPointsFromTrail(TrailRenderer trail, EdgeCollider2D edgeCollider)
    {
        List<Vector2> points = new List<Vector2>();
        for (int position = 0; position < trail.positionCount; position++)
        {
            points.Add(trail.GetPosition(position));
        }
        edgeCollider.SetPoints(points);
        
    }

    void Update()
    {
        setColliderPointsFromTrail(trail, edgeCollider);
    }

    void OnDestroy()
    {
        if (edgeCollider != null)
        {
            Destroy(edgeCollider.gameObject);
        }
    }
   
}
