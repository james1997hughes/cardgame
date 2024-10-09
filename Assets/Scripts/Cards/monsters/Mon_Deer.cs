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
            CardDescription = "+1 <sprite name=\"icons2_3\"> ON ATK";
            HP = 4f;
            MonAtk = 3f;
            Def = 3f;
            PlayerAtk = 1f;
            Cost = 1f;
        }
        public override void SelectEffect() {
            OnSelectAudio = null; //Setting null with intent to put a clip here. If set null, default clip will not play.

            base.SelectEffect(); //Do default select behaviour
        }
        public override void PlayEffect()
        {
            OnPlayAudio = Resources.Load<AudioClip>("Sound/Deer"); //Easier to set this here than deal with inheritance start/awake order

            base.PlayEffect();
        }
        public override void PreAttackEffect()
        {
            StatModifiers["Def"] = 1; //Temporary effect
            HP += 1; //Permanent effect

            base.PreAttackEffect();
        }
        public override void PostAttackEffect()
        {
            resetStats(); //or you can do StatModifiers["Def"] = 0;
            base.PostAttackEffect();
        }
        public override void DiscardEffect()
        {
            base.DiscardEffect();
            Debug.Log("Deer discardEffect");
        }

    }
}