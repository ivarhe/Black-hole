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
        edgeCollider = GetValidCollider();
        StartCoroutine(SetColliderPointsFromTrail(trail, edgeCollider));
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
        validCollider.tag = "trail";
        return validCollider;

    }

    void setColliderPointsFromTrail(TrailRenderer trail, EdgeCollider2D edgeCollider)
    {
        Vector3[] trailPositions = new Vector3[trail.positionCount];
        trail.GetPositions(trailPositions);
        Vector2[] colliderPoints = new Vector2[trailPositions.Length];
        for (int i = 0; i < trailPositions.Length; i++)
        {
            colliderPoints[i] = trailPositions[i];
        }
        edgeCollider.points = colliderPoints;

    }

    IEnumerator SetColliderPointsFromTrail(TrailRenderer trail, EdgeCollider2D edgeCollider)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // to avoid setting the points too often aka lag
            setColliderPointsFromTrail(trail, edgeCollider);
        }

    }


    /*
        void OnDestroy()
        {
            if (edgeCollider != null)
            {
                unusedColliders.Add(edgeCollider);
                edgeCollider.enabled = false;
            }
        }
    */

}
