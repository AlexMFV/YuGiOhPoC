using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    internal class HandManager
    {
        public void DrawCards(Player player, int numberOfCards)
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                //player.Hand.AddCard(player.Deck[0]);
                //player.Deck.RemoveAt(0);
                
                
                //Should spawn a card close to the deck,
                //then move the card to the hand while accounting
                //for other cards in the hand and space them accordingly
            }
        }

        public static void ArrangeHand(Player player)
        {
            if (player.Hand.GetCards() == null || player.Hand.GetCardCount() <= 0)
                return;

            //Test only
            var p1 = player.Hand[0].Object.transform.TransformPoint(0, 0, 0);
            var p2 = player.Hand[0].Object.transform.TransformPoint(1, 1, 0);
            double width = p2.x - p1.x;

            double spacing = 0.2f; //Spacing not working
            double unit = width;

            float prevY = player.Hand[0].Object.transform.position.y;
            player.Hand[0].Object.transform.position = new Vector3(
                (float)(player.HandPosition.transform.position.x - unit * (player.Hand.GetCardCount()-1)), prevY);

            float startPos = player.Hand[0].Object.transform.position.x;
            for(int i = 1; i < player.Hand.GetCardCount(); i++)
            {
                player.Hand[i].Object.transform.position = new Vector3((float)(startPos + (width*2*i)), prevY);
            }
        }
    }
}
