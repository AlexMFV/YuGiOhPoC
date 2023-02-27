using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class CacheParser
    {
        static string assets_path = Application.dataPath;

        //Load deck from file main_deck.json
        public static List<Card> ParseDeck(int playerId)
        {
            List<Card> deck = new List<Card>();
            string finalPath = Path.Combine(assets_path, "Data", $"deck_{playerId}.json");

            if (File.Exists(finalPath))
            {
                string json = File.ReadAllText(finalPath);
                int[] card_ids = JsonConvert.DeserializeObject<int[]>(json);
                //Load parsed cards into deck

                foreach (int id in card_ids)
                    deck.Add((Card)Globals.AllCards.Where(x => x._cardId == id).FirstOrDefault());
            }

            return deck;
        }

        public static void ParseCards()
        {
            List<Card> cards = new List<Card>();
            string finalPath = Path.Combine(assets_path, "Data", "all_cards.json");

            if (File.Exists(finalPath))
            {
                string json = File.ReadAllText(finalPath);
                cards = JsonConvert.DeserializeObject<List<Card>>(json);
            }

            Globals.AllCards = cards;
        }
    }
}
