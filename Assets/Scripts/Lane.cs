using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Lanes
{
    MONSTER_LANE_1,
    MONSTER_LANE_2,
    TRAP_LANE,
    DISCARD_LANE
}
public class Lane : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Card card;

    public Lanes lane;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        card = null;
    }

    public void setCard(Card card1)
    {
        card = card1;
    }





    //Debug boundaries
    void OnDrawGizmos()
    {
        if (spriteRenderer == null)
            return;

        // Get the bounds of the sprite
        Bounds bounds = spriteRenderer.bounds;

        // Get the corner points of the bounds
        Vector3 topLeft = new Vector3(bounds.min.x, bounds.max.y, 0);
        Vector3 topRight = new Vector3(bounds.max.x, bounds.max.y, 0);
        Vector3 bottomLeft = new Vector3(bounds.min.x, bounds.min.y, 0);
        Vector3 bottomRight = new Vector3(bounds.max.x, bounds.min.y, 0);

        // Draw lines between the corner points
        Gizmos.color = Color.red; // Change the color as desired
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
