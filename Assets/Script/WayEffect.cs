using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayEffect : MonoBehaviour
{
    private LineRenderer lr;
    private Vector2[] wayPoint;
    private float during;
    private float duringOffset = 8f;
    private float fadeSpeed = 1.5f;
    private int pointCount;

    public static bool Rendering = false;


    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void RenderBy(List<Cell> path)
    {
        Rendering = true;
        ///Clone data
        lr.positionCount = path.Count;
        pointCount = path.Count;
        wayPoint = new Vector2[path.Count];
        during = path.Count / duringOffset;

        for (int i = 0; i < lr.positionCount; i++)
        {
            wayPoint[i] = path[i].WorldPos;
        }
        if (MazeSystem.renderWithEffect)
        {
            StartCoroutine(RenderWithEffect());
        }
        else
        {
            RenderNormal();
        }
        
    }

    private void RenderNormal()
    {
        for (int i = 0; i < wayPoint.Length; i++)
        {
            lr.SetPosition(i, wayPoint[i]);
        }
    }

    private IEnumerator RenderWithEffect()
    {
        float segmentDuration = during / pointCount;
        lr.SetPosition(0, wayPoint[0]);
        for (int i = 0; i < pointCount -1 ; i++)
        {
            float startTime = Time.time;

            Vector2 startPos = wayPoint[i];
            Vector2 endPos = wayPoint[i + 1];

            Vector2 pos = startPos;
            while(pos != endPos)
            {
                float t = (Time.time - startTime) / segmentDuration;
                pos = Vector2.Lerp(startPos, endPos, t);
                for (int j = i + 1; j < pointCount; j++)
                {
                    lr.SetPosition(j, pos);
                }
                yield return null;
            }
        }
        Rendering = false;
        StartCoroutine(FadeWay());
    }

    private IEnumerator FadeWay()
    {
        yield return new WaitForSeconds(fadeSpeed);
        if (!Rendering)
        {
            lr.positionCount = 0;
        }
        
    }
}
