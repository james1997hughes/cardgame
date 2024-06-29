using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards{

    public class Mon_Spider : Card
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

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[3];
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

            CardName = "Spider";
            CardDescription = "Use spell slot to attack twice";
            PlayEffectDescription = "Summons monster to field";
            HP = 2f;
            MonAtk = 3f;
            Def = 2f;
            PlayerAtk = 2f;
            Cost = 2f;
        }



        public override void SelectEffect(){
            Debug.Log("SpooderSelected");
        }
        public override void PlayEffect(){
            //attack twice if use spell slot
            Debug.Log("SpooderPlayed");
        }
        public override void SpellEffect(){
            //maybe throw
            return;
        }

    }
}
