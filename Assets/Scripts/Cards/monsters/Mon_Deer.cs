using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards{

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

// original values for stat mods
    public float originalHP {get;set;}
    public float originalMonAtk {get;set;}
    public float originalPlayerAtk {get;set;}
    public float originalDef {get;set;}
    public float originalCost {get;set;}
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

            originalHP = 2f;
            originalMonAtk = 1f;
            originalDef = 2f;
            originalPlayerAtk = 1f;
            originalCost = 1f;
    }

        public override void SelectEffect(){
            Debug.Log("DeerSelected");
        }
        public override void PlayEffect(){
            isPermanent = false;
            Def = Def+1f;
            Debug.Log("DeerPlayed");
        }

        public override void SpellEffect(){
            //maybe throw
            return;
        }

}
}