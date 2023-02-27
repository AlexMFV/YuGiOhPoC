using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;

namespace Assets.Scripts
{
    public class GameTimer
    {
        public float timeLeft = 0.0f;
        
        public void Wait(int miliseconds)
        {
            GameManager.shouldRun = false;
            timeLeft = miliseconds / 1000.0f;
        }
        
        public void Tick()
        {   
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f)
            {
                timeLeft = 0.0f;
                GameManager.shouldRun = true;
            }
        }
    }
}
