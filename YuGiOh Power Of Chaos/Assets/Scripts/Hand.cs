using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Hand
    {
        private List<Card> _cardsInHand;

        public Hand()
        {
            _cardsInHand = new List<Card>();
        }

        public void AddCard(int _cardId)
        {
            _cardsInHand.Add(new Card(_cardId, null));
        }
        
        public void AddCard(Card card)
        {
            _cardsInHand.Add(card);
        }

        public int GetCardCount()
        {
            return _cardsInHand.Count;
        }

        public List<Card> GetCards()
        {
            return _cardsInHand;
        }

        public Card GetHighestAttackCard()
        {
            return _cardsInHand.Where(x => x._cardType == "monster").OrderByDescending(x => x._attack).FirstOrDefault();
        }

        public Card GetHighestDefenseCard()
        {
            return _cardsInHand.Where(x => x._cardType == "monster").OrderByDescending(x => x._defense).FirstOrDefault();
        }

        public Card GetHighestValueSpellCard()
        {

        }

        public void Discard(Guid id)
        {
            _cardsInHand.RemoveAll(x => x._id == id);
        }

        public Card GetCard(Guid id)
        {
            return _cardsInHand.Where(x => x?._id == id).FirstOrDefault();
        }

        public List<GameObject> GetCardObjects()
        {
            List<GameObject> cardObjects = new List<GameObject>();
            foreach (Card c in _cardsInHand)
                cardObjects.Add(c.Object);
            return cardObjects;
        }

        public Card this[int index]
        {
            get { return (Card)_cardsInHand[index]; }
            set { _cardsInHand[index] = value; }
        }

        public void ClearSet()
        {
            _cardsInHand.ForEach(x => x.isSet = false);
        }
    }
}
