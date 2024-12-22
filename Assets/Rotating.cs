using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    [Tooltip("旋转速度(度/秒)")]
    public float rotationSpeed = 20f; // 每秒旋转20度

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 绕Z轴旋转(因为是2D游戏)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
