using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType{
    SUMMON,
    ATTACK_MONSTER,
    ATTACK_PLAYER,
    PASS
}

public enum SourceLane{
    MON_LANE_1,
    MON_LANE_2,
    TRAP_LANE
}

public class Move
{
    // A move will always have a MoveType, a target, and the card being played
    public GamePlayer player;
    public MoveType moveType;
    public Card cardTarget;
    public GamePlayer playerTarget;
    public Lane laneTarget;
    public SourceLane sourceLane;
    public Card card; //The card being played


    // Constructor for attack move
    public Move(MoveType moveType, Card card, Card cardTarget, SourceLane sourceLane, GamePlayer player)
    {
        this.moveType = moveType;
        this.card = card;
        this.cardTarget = cardTarget;
        this.sourceLane = sourceLane;
        this.player = player;
    }

    // Constructor for player attack move
    public Move(MoveType moveType, Card card, GamePlayer playerTarget, SourceLane sourceLane, GamePlayer player)
    {
        this.moveType = moveType;
        this.card = card;
        this.playerTarget = playerTarget;
        this.sourceLane = sourceLane;
        this.player = player;
    }

    // Constructor for lane summon move
    public Move(MoveType moveType, Card card, Lane laneTarget, SourceLane sourceLane, GamePlayer player)
    {
        this.moveType = moveType;
        this.card = card;
        this.laneTarget = laneTarget;
        this.sourceLane = sourceLane;
        this.player = player;
    }

    public Move(MoveType moveType, GamePlayer player)
    {
        this.moveType = moveType;
        this.player = player;
    }



}
