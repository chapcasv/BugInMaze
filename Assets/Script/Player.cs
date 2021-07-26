using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float minDistance = 0.005f;
    private static List<Cell> pathAuto;
    private float moveSpeed = 1.5f;
    private Rigidbody2D rb;

    public static bool inAutoMode = false;
    public int pathIndex = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {   
        if (inAutoMode)
        {
            PlayerMove();
        }
    }

    public static void AutoMoveBy(List<Cell> path)
    {
        pathAuto = path;
        inAutoMode = true;
    }

    private void PlayerMove()
    {   
        if(pathAuto != null && pathAuto.Count > pathIndex)
        {
            Cell target = pathAuto[pathIndex];
            Vector3 direction = (target.WorldPos - transform.position);
            LookAt(target);
            if (direction.magnitude <= minDistance)
            {
                transform.position = target.WorldPos;
                pathIndex++;
            }
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
        else inAutoMode = false;
    }

    /// Some magic number for rotation :))
    private void LookAt(Cell target)
    {
        Vector3 direction = transform.position - target.WorldPos;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if(angle < 1 && angle > -2) angle = 180;
        else if(angle > 170 || angle < - 170) angle = 0;
        rb.rotation = angle;
    }
}
