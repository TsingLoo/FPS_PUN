using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private List<RopeSegment> segments = new List<RopeSegment>();
    private float ropeSegLen = 0.25f;
    private float segmentLength = 35;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < segmentLength; i++)
        {
            segments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }

    }

    // Update is called once per frame
    void Update()
    {



    }

    private void UpdateRope() 
    {
        
    }

    private class RopeSegment
    {
        public Vector2 last;
        public Vector2 cur;

        public RopeSegment(Vector2 startPos)
        {
            this.last = startPos;
            this.cur = startPos;
        }
    }
}
