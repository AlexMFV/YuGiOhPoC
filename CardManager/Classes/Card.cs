using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardManager
{
    public class Card
    {
        //ingame id (should be unique, this might be attributed when a card comes out of the deck)
        public Guid _id;
        //faceup/facedown
        public bool _faceup;
        //attack/defense mode
        public bool _attackMode;

        [JsonProperty("card_id")]
        public int _cardId; //id associates with the defacto card (image, etc)
        [JsonProperty("name")]
        public string _name { get; set; }
        [JsonProperty("description")]
        public string _description;
        [JsonProperty("attack")]
        public int _attack { get; set; }
        [JsonProperty("defense")]
        public int _defense { get; set; }
        [JsonProperty("level")]
        public int _starRating { get; set; }
        [JsonProperty("card_type")]
        public string _cardType;
        //public CardType _cardType;

        [JsonProperty("archetype")] //Not used
        public int _cardArchetype;  //Not used
        [JsonProperty("special_summon")]
        public bool _specialSummon;
        [JsonProperty("attribute")]
        public string _attribute; //Convert to attribute enumerator
        [JsonProperty("types")]
        public string[] _types; //Convert to array of enumerator Types
        [JsonProperty("image")]
        public string _imageName;

        public Card(int cardId)
        {
            _id = Guid.NewGuid();
            _cardId = cardId; //getCardFromDeck();
            _name = _name; //CardManager.getName(_cardId);
            _description = _description; //CardManager.getDescription(_cardId);
            _attack = _attack; //CardManager.getAttack(_cardId);
            _defense = _defense; //CardManager.getDefense(_cardId);
            _starRating = _starRating; //CardManager.getStarRating(_cardId);
            _cardType = _cardType; //CardManager.getType(_cardId);
            _cardArchetype = _cardArchetype; //CardManager.getArchetype(_cardId);
            _specialSummon = _specialSummon; //CardManager.getSpecialSummon(_cardId);
        }

        public Card() { this._id = Guid.NewGuid(); }

        public Card(Card card)
        {
            _id = Guid.NewGuid();
            _faceup = false;
            _attackMode = card._attackMode;
            _cardId = card._cardId; //getCardFromDeck();
            _name = card._name; //CardManager.getName(_cardId);
            _description = card._description; //CardManager.getDescription(_cardId);
            _attack = card._attack; //CardManager.getAttack(_cardId);
            _defense = card._defense; //CardManager.getDefense(_cardId);
            _starRating = card._starRating; //CardManager.getStarRating(_cardId);
            _cardType = card._cardType; //CardManager.getType(_cardId);
            _cardArchetype = card._cardArchetype; //CardManager.getArchetype(_cardId);
            _specialSummon = card._specialSummon; //CardManager.getSpecialSummon(_cardId);
            _attribute = card._attribute;
            _types = card._types;
            _imageName = card._imageName;
        }

        public Card Clone()
        {
            return new Card(this);
        }

        //Corresponds to the value of the card depending on its position, if the card is in attack mode, the attack value is returned, otherwise the defense value is returned
        public int GetPrimaryValue()
        {
            if (_attackMode)
                return _attack;
            else
                return _defense;
        }

        public int GetCardValue(bool isAttackMode)
        {
            if (isAttackMode)
                return _attack;
            else
                return _defense;
        }

        public static Card GetCardByID(IEnumerable<Card> cards, Guid id)
        {
            return cards.Where(x => x._id == id).FirstOrDefault();
        }
    }
}
