using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards{
    public class Mon_Slime : Card
    {
        
        public override string CardName {get;set;}
        public override string CardDescription {get;set;}
        public override float HP {get;set;}
        public override float MonAtk {get;set;}
        public override float PlayerAtk {get;set;}
        public override float Def {get;set;}
        public override float Cost {get;set;}
        void Awake(){

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[0];
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

            CardName = "Slime";
            CardDescription = "+1 Card HP on Attack";
            HP = 2f;
            MonAtk = 2f;
            Def = 1f;
            PlayerAtk = 4f;
            Cost = 2f;
        }



        public override void SelectEffect()
        {

        }
        public override void PreAttackEffect()
        {
            
        }
        public override void PostAttackEffect()
        {
            
        }
        public override void SpellEffect()
        {

        }

    }
}