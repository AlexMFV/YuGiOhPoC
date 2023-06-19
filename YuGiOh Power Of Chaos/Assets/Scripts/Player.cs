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
        private Card[] _monsterZone;
        private Card[] _specialZone;
        private int _health;
        Transform handPos;
        private bool _isBot;

        public int ID { get { return _playerId; } }
        public Hand Hand { get { return _hand; } set { _hand = value; } }
        public List<Card> Deck { get { return _mainDeck; } set { _mainDeck = value; } }
        public Transform HandPosition { get { return handPos; } }
        public int Health { get { return _health; } set { _health = value; } }

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
            allCards.AddRange(_monsterZone.Where(x => x != null));
            allCards.AddRange(_specialZone.Where(x => x != null));
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
            _monsterZone = new Card[5]; //Can only have a maximum of 5 monsters on the field
            _specialZone = new Card[5]; //Can only have a maximum of 5 special cards on the field
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
            return _monsterZone.Where(x => x != null).ToList();
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
                    _monsterZone[FirstPlayable(_monsterZone)] = card;
                else
                    return null;
            }
            else
                _specialZone[FirstPlayable(_specialZone)] = card;

            this.Hand.Discard(id); //Remove from hand
                                   //Check if card is removed
            return card;
        }

        internal int FirstPlayable(Card[] zone)
        {
            for (int i = 0; i < zone.Count(); i++)
                if (zone[i] == null)
                    return i;
            return -1;
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
                if (_monsterZone.Count(x => x != null) <= 5)
                    return GameObject.Find($"{player}_normal" + GetCardIndex(_monsterZone, card)).transform;
            }
            else
            {
                if (_specialZone.Count(x => x != null) <= 5)
                    return GameObject.Find($"{player}_special" + GetCardIndex(_specialZone, card)).transform;
            }

            return null;
        }

        private string GetCardIndex(Card[] zone, Card card)
        {
            for (int i = 1; i <= zone.Count(); i++)
                if (zone != null && zone[i-1]._id == card._id)
                    return i.ToString();
            return "";
        }

        internal bool CanPlayCard(Card card)
        {
            if (card == null)
                return false;

            if (card._cardType == "monster" || card._cardType == "fusion")
            {
                //if (Globals.canPlayCard) //(Globals.currentPhase == GamePhase.MainPhase1 || Globals.currentPhase == GamePhase.MainPhase2) && 
                //{
                if (_monsterZone.Count(x => x != null) < 5)
                    return true;
                //}
            }
            else
            {
                if (_specialZone.Count(x => x != null) < 5)
                    return true;
            }

            return false;
        }

        internal int MonstersCount()
        {
            return _monsterZone.Count(x => x != null);
        }

        internal int SpecialsCount()
        {
            return _specialZone.Count(x => x != null);
        }

        internal bool CardBelongsToPlayer(Guid cardID)
        {
            if (_monsterZone.Any(y => y != null && y._id == cardID)) return true;
            if (_specialZone.Any(y => y != null && y._id == cardID)) return true;
            if (_graveyard.Any(y => y._id == cardID)) return true;
            if (_fusionStack.Any(y => y._id == cardID)) return true;
            if (_sideDeck.Any(y => y._id == cardID)) return true;
            if (Hand.GetCards().Any(y => y._id == cardID)) return true;
            if (_mainDeck.Any(y => y._id == cardID)) return true;

            return false;
        }

        internal void TakeDamage(int damage)
        {
            if(damage > 0)
            {
                //Use blue font
                //Plus sign in front
                this.Health += damage; //Should be added to health
                //Play damage earned sound
            }
            else
            {
                TextManager.TakeDamage(this, damage);
                //this.Health += damage; //Should be added to health
            }
        }

        internal void RemoveMonster(Guid id)
        {
            for(int i = 0; i < _monsterZone.Count(); i++)
            {
                if (_monsterZone[i] != null && _monsterZone[i]._id == id)
                    _monsterZone[i] = null;
            }
        }
    }
}
