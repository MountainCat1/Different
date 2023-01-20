using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem {
    public class SpriteSwitchAction : ActionObject
    {
        [SerializeField] public Sprite sprite;
        [SerializeField] private Sprite baseSprite;

        [HideInInspector] [SerializeField] private bool switched = false;

        private SpriteRenderer spriteRenderer;

        public bool Switched { get => switched; set => Swtich(value); }


        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseSprite = spriteRenderer.sprite;
        }

        public override void Action()
        {
            switched = !switched;
            Swtich(switched);
        }


        private void Swtich(bool value)
        {
            if (value == switched)
                return;

            switched = value;

            if (switched)
                spriteRenderer.sprite = sprite;
            else
                spriteRenderer.sprite = baseSprite;
        }

    }

}