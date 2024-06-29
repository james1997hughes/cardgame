using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Mon_Deer : Card
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

        this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[10];
        isMonster = true;
        isSpell = false;
        canBeTrap = false;

        CardName = "Deer";
        CardDescription = "+1 Def on Attack";
        PlayEffectDescription = "Summons monster to field";
        HP = 2f;
        MonAtk = 1f;
        Def = 2f;
        PlayerAtk = 1f;
        Cost = 1f;
    }



    public override void SelectEffect(){
        Debug.Log("DeerSelected");
    }
    public override void PlayEffect(){
        //+1 def on atk
        Debug.Log("DeerPlayed");
    }
    public override void SpellEffect(){
        //maybe throw
        return;
    }

}
