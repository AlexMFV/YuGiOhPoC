using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal class CardType
    {
        public ParentType _type;
        public List<SubType> _subTypes;
    }

    internal enum ParentType
    {
        Monster,    //M
        Spell,      //S
        Trap        //T
    }

    internal enum SubType
    {
        Fusion,     //F
        Effect,     //E
        Once,       //O
        Continuous  //C
    }
}
