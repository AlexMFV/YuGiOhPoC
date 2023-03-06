using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class Helpers
    {
        public static PlayType checkStarRating(Player p, Card card)
        {
            //No Tribute Summon
            if (card._starRating < 5 && p.CanPlayCard(card))
                    return PlayType.Summon;
            
            //Require 1 Tribute
            if(card._starRating == 5 || card._starRating == 6)
            {
                if (p.MonstersCount() > 0) //Does not need CanPlay verification since it will replace the card
                    return PlayType.Summon;
            }

            //Require 2 Tributes
            if(card._starRating > 6)
            {
                if (p.MonstersCount() > 1)
                    return PlayType.Summon;
            }

            return PlayType.NotPlayable;
        }

        public static PlayType checkSpellType(Player p, Card card)
        {
            if(card._types[0] == "instant")
            {
                //Checks in the effect list if this card meets all the requirements to be played.
                //e.g SpellEffect(card.cardID) returns boolean
                return PlayType.Activate;
            }
            else
            {
                return PlayType.Set;
            }
        }

        public static PlayType checkTrapType(Player p, Card card)
        {
            if (card._types[0] == "single")
            {
                return PlayType.Set;
            }
            else
            {
                //Continuous
                return PlayType.Set;
            }
        }
    }
}
