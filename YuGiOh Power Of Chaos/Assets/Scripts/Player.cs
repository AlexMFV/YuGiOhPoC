using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Player
    {
        //0: bottom (self), 1: top (other/AI)
        private int _playerId;
        private Hand _hand;
        private List<Card> _mainDeck;
        private List<Card> _graveyard;
        private List<Card> _fusionStack;
        private List<Card> _sideDeck;
        private List<Card> _monsterZone;
        private List<Card> _specialZone;
        private int _health;
        Transform handPos;
        private bool _isBot;

        public int ID { get { return _playerId; } }
        public Hand Hand { get { return _hand; } set { _hand = value; } }
        public List<Card> Deck { get { return _mainDeck; } set { _mainDeck = value; } }
        public Transform HandPosition { get { return handPos; } }

        //public Card FindCard(string cardTag)
        //{
        //    string[] splits = cardTag.Split('-');
        //
        //    if (splits.Count() > 1)
        //    {
        //        int player = int.Parse(splits[0]);
        //        Guid guid = Guid.Parse(splits[1]);
        //
        //        List<Card> allCards = new List<Card>();
        //        allCards.AddRange(_mainDeck);
        //        allCards.AddRange(_graveyard);
        //        allCards.AddRange(_fusionStack);
        //        allCards.AddRange(_sideDeck);
        //        allCards.AddRange(_monsterZone);
        //        allCards.AddRange(_specialZone);
        //
        //        return allCards.FirstOrDefault(x => x._id == guid);
        //    }
        //    return null;
        //}

        public Dictionary<Guid, Card> GetAllPlayerCards()
        {
            List<Card> allCards = new List<Card>();
            allCards.AddRange(_mainDeck);
            allCards.AddRange(_graveyard);
            allCards.AddRange(_fusionStack);
            allCards.AddRange(_sideDeck);
            allCards.AddRange(_monsterZone);
            allCards.AddRange(_specialZone);
            allCards.AddRange(this._hand.GetCards());

            Dictionary<Guid, Card> allCardsDict = new Dictionary<Guid, Card>();
            foreach (Card card in allCards)
                allCardsDict.Add(card._id, card);

            return allCardsDict;
            //return allCards.FirstOrDefault(x => x._id == guid);
        }

        public Player(int playerId, bool isBot = false)
        {
            _playerId = playerId;
            _hand = new Hand();
            _mainDeck = new List<Card>();
            _graveyard = new List<Card>();
            _fusionStack = new List<Card>();
            _sideDeck = new List<Card>();
            _monsterZone = new List<Card>();
            _specialZone = new List<Card>();
            _health = 8000;
            _isBot = isBot;

            if(playerId == 1) //Player1
                handPos = GameObject.Find("player1_hand").transform;
            else
                handPos = GameObject.Find("player2_hand").transform;
        }

        internal void RandomizeDeck()
        {
            _mainDeck = _mainDeck.OrderBy(x => Guid.NewGuid()).ToList();
        }

        internal void LoadDeck()
        {
            _mainDeck = CacheParser.ParseDeck(_playerId);
        }

        internal Card DrawCard()
        {
            if (_mainDeck.Count > 0)
            {
                Card card = _mainDeck[0];
                card._faceup = !this._isBot; //If bot then facedown, if player faceup (maybe use a different variable for played faceup/facedown)
                _mainDeck.RemoveAt(0);
                _hand.AddCard(card);
                return card;
            }
            else
            {
                //TODO: Should return an error or end the game since the player has no more cards left
            }
            return null;
        }
    }
}
