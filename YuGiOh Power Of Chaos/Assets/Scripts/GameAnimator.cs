﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameAnimator : MonoBehaviour
    {
        static GameObject handCard;
        static GameObject playedCard;

        //Animators
        [SerializeField]
        public static Animator animator;
        //public Animator animator_nextPlayerP1;

        void Start()
        {
            handCard = Resources.Load("Prefabs/handCard") as GameObject;
            playedCard = Resources.Load("Prefabs/playedCard") as GameObject;

            //Find GameObject named GamePhaseAnimator
            GameObject animatorObj = GameObject.Find("GamePhaseAnimator");

            if(animatorObj != null)
                animator = animatorObj.GetComponent<Animator>();

            if(animator == null)
                Debug.Log("Animator not found");
        }

        internal static void InstantiateCard(Player player, Card player_card)
        {
            GameObject obj = Instantiate(handCard, player.HandPosition);
            obj.name = $"{player.ID}:{player_card._id}"; //hand Card (maybe add -h- in the middle to differenciate between played and hand cards)

            if(player_card._faceup)
                obj.GetComponent<SpriteRenderer>().sprite = Globals.Sprites[player_card._imageName];
            else
                obj.GetComponent<SpriteRenderer>().sprite = Globals.Sprites["card_ura"];

            if (obj != null)
                player_card.Object = obj;
        }
        
        internal static void InstantiatePlayedCard(Player player, Card player_card, Transform position)
        {
            GameObject obj = Instantiate(playedCard, position.transform);
            obj.name = $"{player.ID}:{player_card._id}"; //played Card (maybe add -p- in the middle to differenciate)

            player_card._attackMode = true;

            if (player_card._cardType == "trap")
            {
                player_card._attackMode = false;
                player_card._faceup = false;
            }

            if (player_card.isSet)
            {
                if (player_card._cardType == "monster" || player_card._cardType == "fusion")
                    obj.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                
                player_card._faceup = false;
                player_card._attackMode = false;
            }
            
            
            if (player_card._faceup)
                obj.GetComponent<SpriteRenderer>().sprite = Globals.Sprites[player_card._imageName];
            else
                obj.GetComponent<SpriteRenderer>().sprite = Globals.Sprites["card_ura"];

            if(player_card._cardType == "monster" || player_card._cardType == "fusion")
                CreateAttackDefenseLabel(player_card, position);

            if (obj != null)
                player_card.Object = obj;
        }
        
        internal static void CreateAttackDefenseLabel(Card card, Transform trans)
        {
            TextMeshPro meshPro = new TextMeshPro();
            TextMesh mesh = new TextMesh();
            
            Vector3 newPos = trans.position;
            newPos.y -= 0.84f;
            newPos.z = 0f;

            GameObject txtobj = new GameObject($"label-{card._id}");

            //if (trans.childCount > 0)
            //    txtobj.transform.SetParent(trans.GetChild(0));
            //else
                txtobj.transform.SetParent(trans);

            txtobj.transform.position = newPos;
            TextMeshPro label = txtobj.AddComponent<TextMeshPro>();
            label.text = $"{card._attack}/{card._defense}";
            label.fontSize = 5;
            label.transform.localScale = new Vector3(.5f, .5f, .5f);
            label.fontStyle = FontStyles.Bold;
            label.horizontalAlignment = HorizontalAlignmentOptions.Center;
            label.verticalAlignment = VerticalAlignmentOptions.Middle;

            if (trans.childCount > 0)
                txtobj.transform.SetParent(trans.GetChild(0));
        }

        internal static void AnimateNextPlayer(Player currentPlayer)
        {
            if(currentPlayer.ID == 1) //isPlayer then play the blue to red animation
                animator.Play("NextPlayerCPU", -1, 0.0f);
            else
                animator.Play("NextPlayerP1", -1, 0.0f);
        }
    }
}