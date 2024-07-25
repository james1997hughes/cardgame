using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards
{

    public class Mon_Bat : Card
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

            this.subjectSprite = Resources.LoadAll<Sprite>("monsters")[7];
            isMonster = true;
            isSpell = false;
            canBeTrap = false;

            CardName = "Bat";
            CardDescription = "+1 Player HPon Attack";
            HP = 1f;
            MonAtk = 2f;
            Def = 2f;
            PlayerAtk = 2f;
            Cost = 1f;
        }



        public override void SelectEffect()
        {
            OnSelectAudio = null;
            base.SelectEffect();
        }
        public override void PlayEffect()
        {
            OnPlayAudio = Resources.Load<AudioClip>("Sound/Bat"); //Easier to set this here than deal with inheritance start/awake order

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
            OnDiscardAudio = null;
            base.DiscardEffect();
        }
        public override void SpellEffect()
        {
            base.SpellEffect();
        }
    }
}
