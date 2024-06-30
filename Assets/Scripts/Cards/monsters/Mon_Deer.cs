using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards
{

    public class Mon_Deer : Card
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

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[10];
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

            CardName = "Deer";
            CardDescription = "+1 Def while Attacking & +1 HP Permanently";
            HP = 2f;
            MonAtk = 1f;
            Def = 2f;
            PlayerAtk = 1f;
            Cost = 1f;
        }

        public override void SelectEffect()
        {

        }
        public override void PreAttackEffect()
        {
            StatModifiers["Def"] = 1; //Temporary
            HP += 1; //Permanent
        }
        public override void PostAttackEffect()
        {
            resetStats(); //or you can do StatModifiers["Def"] = 0;
        }

    }
}