using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards{

    public class Mon_Bat : Card
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

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[7];
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

        CardName = "Bat";
        CardDescription = "+1 Player HPon Attack";
        PlayEffectDescription = "Summons monster to field";
        HP = 1f;
        MonAtk = 2f;
        Def = 2f;
        PlayerAtk = 2f;
        Cost = 1f;
    }



    public override void SelectEffect(){
        Debug.Log("MothSelected");
    }
    public override void PlayEffect(){
        //+1 player hp
        Debug.Log("MothPlayed");
    }
    public override void SpellEffect(){
        //maybe throw
        return;
    }

    }
}
