using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards
{

    public class Mon_Eagle : Card
    {

        public override string CardName { get; set; }
        public override string CardDescription { get; set; }
        public override float HP { get; set; }
        public override float MonAtk { get; set; }
        public override float PlayerAtk { get; set; }
        public override float Def { get; set; }
        public override float Cost { get; set; }
        void Awake()
        {

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[6];
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

            CardName = "Eagle";
            CardDescription = "+1 Spell slot next turn";
            HP = 2f;
            MonAtk = 5f;
            Def = 4f;
            PlayerAtk = 3f;
            Cost = 4f;
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