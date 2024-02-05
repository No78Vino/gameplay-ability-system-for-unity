using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GroundDetector : MonoBehaviour
{
    [SerializeField] private Player _element;
    private BoxCollider2D _box;
    private static int GroundLayer;
    
    
    private void Awake()
    {
        GroundLayer = LayerMask.GetMask("Terrain");
        _box = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        bool grounded = IsGrounded();
        _element.SetIsGrounded(grounded);
    }

    bool IsGrounded()
    {
        var bounds = _box.bounds;
        var hitGround = Physics2D.OverlapBox(bounds.center, bounds.size, 0, GroundLayer);
        return hitGround != null;
    }
}
