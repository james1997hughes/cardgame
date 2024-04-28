using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Mon_Eagle : Card
{
    
    public override string CardName {get;set;}
    public override string CardDescription {get;set;}
    public override string PlayEffectDescription {get;set;}
    public override float HP {get;set;}
    public override float MonAtk {get;set;}
    public override float PlayerAtk {get;set;}
    public override float Def {get;set;}
    public override float Cost {get;set;}
    void Awake(){

        this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[6];
        isMonster = true;
        isSpell = false;
        canBeTrap = false;

        CardName = "Eagle";
        CardDescription = "+1 Card HP on Attack";
        PlayEffectDescription = "Summons monster to field";
        HP = 5f;
        MonAtk = 5f;
        Def = 5f;
        PlayerAtk = 2f;
        Cost = 1f;
    }



    public override void SelectEffect(){
        Debug.Log("MothSelected");
    }
    public override void PlayEffect(){
        Debug.Log("MothPlayed");
    }
    public override void SpellEffect(){
        //maybe throw
        return;
    }

}
