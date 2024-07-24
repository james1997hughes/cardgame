using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards
{

    public class Mon_Spider : Card
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

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[3];
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

            CardName = "Spider";
            CardDescription = "Use spell slot to attack twice";
            HP = 2f;
            MonAtk = 3f;
            Def = 2f;
            PlayerAtk = 2f;
            Cost = 2f;
        }



        public override void SelectEffect()
        {
            OnSelectAudio = null;
            base.SelectEffect();
        }
        public override void PlayEffect()
        {
            OnPlayAudio = Resources.Load<AudioClip>("Sound/Eagle"); //Easier to set this here than deal with inheritance start/awake order

            base.PlayEffect();
        }
        public override void PreAttackEffect()
        {
            base.PreAttackEffect();
        }
        public override void PostAttackEffect()
        {
            base.PostAttackEffect();
        }
        public override void DiscardEffect()
        {
            base.DiscardEffect();
        }
        public override void SpellEffect()
        {
            base.SpellEffect();
        }

    }
}
