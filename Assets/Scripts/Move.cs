using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType{
    SUMMON,
    ATTACK_MONSTER,
    ATTACK_PLAYER,
    SPELL,
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
    public Lanes laneTarget;
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
    public Move(MoveType moveType, Card card, Lanes laneTarget, GamePlayer player)
    {
        this.moveType = moveType;
        this.card = card;
        this.laneTarget = laneTarget;
        this.player = player;
    }

    // Constructor for spell move
    public Move(MoveType moveType, Card card, Card cardTarget, GamePlayer player)
    {
        this.moveType = moveType;
        this.card = card;
        this.cardTarget = cardTarget;
        this.player = player;
    }



    // Constructor for pass move
    public Move(MoveType moveType, GamePlayer player)
    {
        this.moveType = moveType;
        this.player = player;
    }


    public override String ToString(){
        String str = $"Player: {player.name}, Type: {moveType}\n";
        String target = "";
        if (moveType == MoveType.SUMMON){
            target = $"{player.name} summons {card.CardName} to {laneTarget}!";
        }
        if (moveType == MoveType.ATTACK_MONSTER){
            target = $"{player.name} attacks {cardTarget.CardName} with {card.CardName}!";
        }
        if (moveType == MoveType.ATTACK_PLAYER){
            target = $"{player.name} attacks {playerTarget.name} with {card.CardName}!";
        }
        if (moveType == MoveType.SPELL){
            target = $"{player.name} uses {card.CardName} on {cardTarget.CardName}!";
        }
        return str+target;
    }


}
