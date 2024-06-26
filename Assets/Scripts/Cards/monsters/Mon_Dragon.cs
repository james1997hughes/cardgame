using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards{

    public class Mon_Dragon : Card
    {
        
        public override string CardName {get;set;}
        public override string CardDescription {get;set;}
        public override float HP {get;set;}
        public override float MonAtk {get;set;}
        public override float PlayerAtk {get;set;}
        public override float Def {get;set;}
        public override float Cost {get;set;}
        void Awake(){

            this.subjectSprite = Resources.Load<Sprite>("MOTH");
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

            CardName = "Moth";
            CardDescription = "+1 Card HP on Attack";
            HP = 2f;
            MonAtk = 3f;
            Def = 4f;
            PlayerAtk = 1f;
            Cost = 3f;
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
