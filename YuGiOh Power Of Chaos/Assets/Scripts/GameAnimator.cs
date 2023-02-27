using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameAnimator : MonoBehaviour
    {
        static GameObject handCard;
        static GameObject playedCard;

        void Start()
        {
            handCard = Resources.Load("Prefabs/handCard") as GameObject;
            playedCard = Resources.Load("Prefabs/playedCard") as GameObject;
        }

        internal static void InstatiateCard(Player player, Card player_card)
        {
            GameObject obj = Instantiate(handCard, player.HandPosition);
            obj.name = $"{player.ID}:{player_card._id}"; //hand Card (maybe add -h- in the middle to differenciate between played and hand cards)

            if (obj != null)
                player_card.Object = obj;
        }
        
        internal static void InstatiatePlayedCard(Player player, Card player_card, Transform position)
        {
            GameObject obj = Instantiate(playedCard, position.transform);
            obj.name = $"{player.ID}:{player_card._id}"; //played Card (maybe add -p- in the middle to differenciate)

            if (obj != null)
                player_card.Object = obj;
        }
    }
}