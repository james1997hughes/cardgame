using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{

    public class Spell_Sliceup : Card
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

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[14];
            isMonster = false;
            isSpell = true;
            canBeTrap = false;

            CardName = "Slice Up";
            CardDescription = "+1 MonAtk permanently";
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
            Debug.Log("Card Atk:" + card.MonAtk);
            card.MonAtk++;
            Debug.Log("Card Atk:" + card.MonAtk);
            base.SpellEffect(card);
        }
        public override void DiscardEffect()
        {
            base.DiscardEffect();
        }
    }
}
