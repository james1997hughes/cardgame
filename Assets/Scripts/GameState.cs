using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // Basically keep the state of the game here
    // Event driven
    // List of Move (s) to allow replay of game state
    // and utilize for AI - implement gamestate Copy & Calculate win probability functions.
    // for each AI move, it will copy the gamestate & iterate over possible moves, choosing the best move each time for now
    // Once current best move has been selected, implement search logic, allowing it to iterate over the next N moves, where N is difficulty
    // difficulty could also be adjusted by keeping track of all possible moves in order of how good the move is
    // that way, an ai could choose the 2nd or 3rd best move instead
    // it will need to simulate player turns whilst not knowing exactly what cards the player has
    // stockfish-esque

    //What's the input for how good a move is?
    //Strength on field - monAttack total, playerAttack total
    //This would be cards played, buffs to current cards
    //Player HP reduced - if a player attack can be played, highest value reduction
    GamePlayer friendlyPlayer {get; set;}
    GamePlayer enemyPlayer {get; set;}

    List<Move> playedMoves {get; set;}


    GameState copyGameState(){
        GameState copy = new GameState();
        copy.friendlyPlayer = friendlyPlayer;
        copy.enemyPlayer = enemyPlayer;
        copy.playedMoves = playedMoves;
        //etc
        return copy;
    }

    float calculateWinProbability(GamePlayer player, float depth){
        // Calculate win chance at current move
        // This is the hard one i think
        // it will need to recurse up to depth
        // starting with the move at the top of playedMoves
        // Therefore, AI will add it's possible move to the top of played moves
        // then simulate all possible following moves up to depth moves
        // then somehow calculate a strength value for the end state, representing the win chance
        return -1f;
    }

    List<Move> getPossibleMoves(GamePlayer player){
        //Should this be here?
        return new List<Move>();
    }

    Move getBestMove(GamePlayer player, float depth){
        // Generate all possible current moves
        // for each possible current move, run a simulation of all possible best followup moves up to depth turns
        // if the current move creates a chain leading to a greater win chance, set this is the candidate
        // once all possible current moves have been analysed, return best move
        List<Move> possibleMoves = getPossibleMoves(player);
        Move bestMove = null;
        float bestMoveStrength = -1;

        foreach(Move move in possibleMoves){
            float moveStrength = calculateWinProbability(player, depth);
            if (moveStrength > bestMoveStrength){
                bestMoveStrength = moveStrength;
                bestMove = move;
            }
        }

        return bestMove;
    }


}
