using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards
{
    public class Mon_Pig : Card
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

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[8];
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

            CardName = "Pig";
            CardDescription = "+1 Card HP on Attack";
            HP = 1f;
            MonAtk = 4f;
            Def = 4f;
            PlayerAtk = 3f;
            Cost = 3f;
        }



        public override void SelectEffect()
        {
            OnSelectAudio = null;
            base.SelectEffect();
        }
        public override void PlayEffect()
        {
            OnPlayAudio = Resources.Load<AudioClip>("Sound/Boar"); //Easier to set this here than deal with inheritance start/awake order

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

    }
}