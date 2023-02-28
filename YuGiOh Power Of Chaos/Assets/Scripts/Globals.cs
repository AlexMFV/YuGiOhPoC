using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Globals
    {
        public static List<Card> AllCards = new List<Card>();
        public static Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
        public static Player p1;
        public static Player cpu;
        public static Dictionary<Guid, Card> p1_cards;
        public static Dictionary<Guid, Card> cpu_cards;

        //Cursor
        public static RaycastHit2D hit;
        public static Card hitCard = null;
        public static bool isCardHit = false;
        public static bool isDefaultCursor = true;
        public static bool isPlayedCard = false;

        public static GamePhase currentPhase = GamePhase.GameStart;
        public static bool canPlayCard = true;
    }
}