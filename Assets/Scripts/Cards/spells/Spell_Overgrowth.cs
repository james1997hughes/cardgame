using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class Spell_Overgrowth : Card
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

            this.subjectSprite = Resources.LoadAll<Sprite>("spells")[19];
            isMonster = false;
            isSpell = true;
            canBeTrap = false;
            isReactive = true;

            CardName = "Overgrowth";
            CardDescription = "+1 MonHP";
            HP = 0f;
            MonAtk = 0f;
            Def = 0f;
            PlayerAtk = 0f;
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
        public override void SpellEffect(Card card)
        {

        }
    }
}