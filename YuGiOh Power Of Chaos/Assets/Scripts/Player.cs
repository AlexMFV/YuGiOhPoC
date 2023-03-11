using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player
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

            if (playerId == 1) //Player1
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

        internal List<Card> GetMonsterZone()
        {
            return _monsterZone;
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

        internal Card PlayCard(Guid id)
        {
            Card card = this.Hand.GetCard(id); //Get the card to remove from the hand

            //If card is monster/fusion, move it to the monster zone
            //If card is special move it to the special zone
            if (card._cardType == "monster" || card._cardType == "fusion")
            {
                if (Globals.canPlayCard)
                    _monsterZone.Add(card);
                else
                    return null;
            }
            else
                _specialZone.Add(card);

            this.Hand.Discard(id); //Remove from hand
                                   //Check if card is removed
            return card;
        }

        internal Transform GetCardPosition(Card card)
        {
            //CHANGE THIS LATER
            string player;
            if(this._playerId == 1)
                player = "p";
            else
                player = "cpu";

            if (card._cardType == "monster" || card._cardType == "fusion")
            {
                if (_monsterZone.Count() <= 5)
                    return GameObject.Find($"{player}_normal" + (_monsterZone.Count())).transform;
            }
            else
            {
                if (_specialZone.Count() <= 5)
                    return GameObject.Find($"{player}_special" + (_specialZone.Count())).transform;
            }

            return null;
        }

        internal bool CanPlayCard(Card card)
        {
            if (card == null)
                return false;

            if (card._cardType == "monster" || card._cardType == "fusion")
            {
                //if (Globals.canPlayCard) //(Globals.currentPhase == GamePhase.MainPhase1 || Globals.currentPhase == GamePhase.MainPhase2) && 
                //{
                if (_monsterZone.Count() < 5)
                    return true;
                //}
            }
            else
            {
                if (_specialZone.Count() < 5)
                    return true;
            }

            return false;
        }

        internal int MonstersCount()
        {
            return _monsterZone.Count();
        }

        internal int SpecialsCount()
        {
            return _specialZone.Count();
        }

        internal bool CardBelongsToPlayer(Guid cardID)
        {
            if (_monsterZone.Any(y => y._id == cardID)) return true;
            if (_specialZone.Any(y => y._id == cardID)) return true;
            if (_graveyard.Any(y => y._id == cardID)) return true;
            if (_fusionStack.Any(y => y._id == cardID)) return true;
            if (_sideDeck.Any(y => y._id == cardID)) return true;
            if (Hand.GetCards().Any(y => y._id == cardID)) return true;
            if (_mainDeck.Any(y => y._id == cardID)) return true;

            return false;
        }
    }
}
