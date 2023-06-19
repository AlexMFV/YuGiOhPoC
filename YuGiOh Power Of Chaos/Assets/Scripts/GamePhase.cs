using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal enum GamePhase
    {
        GameStart,
        DrawPhase,
        StandbyPhase,
        MainPhase1,
        BattlePhase,
        BP_StartStep,
        BP_BattleStep,
        BP_DamageStep,
        BP_EndStep,
        MainPhase2,
        EndPhase,
        EndGame
    }
}