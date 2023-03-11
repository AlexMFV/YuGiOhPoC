using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
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
        public string _name;
        [JsonProperty("description")]
        public string _description;
        [JsonProperty("attack")]
        public int _attack;
        [JsonProperty("defense")]
        public int _defense;
        [JsonProperty("level")]
        public int _starRating;
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

        public GameObject _prefab;
        public PlayType _playType;
        public bool isSet;
        public bool canAttack;

        public Card(int cardId, GameObject prefab)
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
            _prefab = prefab;
        }

        public Card() { this._id = Guid.NewGuid(); }

        public Card(Card card)
        {
            _id = Guid.NewGuid();
            _faceup = false;
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

        public GameObject Object { get { return _prefab; } set { _prefab = value; } }
        public PlayType PlayType { get { return _playType; } set { _playType = value; } }

        public void Flip()
        {
            _faceup = !_faceup;
            if (_faceup)
                this.Object.GetComponent<SpriteRenderer>().sprite = Globals.Sprites[this._imageName];
            else
                this.Object.GetComponent<SpriteRenderer>().sprite = Globals.Sprites["card_ura"];
        }

        public Card Clone()
        {
            return new Card(this);
        }
    }
}
