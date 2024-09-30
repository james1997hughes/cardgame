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
}
