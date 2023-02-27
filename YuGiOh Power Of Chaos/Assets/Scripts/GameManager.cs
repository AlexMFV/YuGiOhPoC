using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GamePhase phase;
    public static bool shouldRun = true;
    static GameTimer timer;
    static BoardManager board;

    static int curr_player = -1;

    static bool firstRun = true;
    static bool debug = false;

    // Start is called before the first frame update
    void Start()
    {        
        board = new BoardManager();
        timer = new GameTimer();

        //This should be done once the game is started and is loading
        CacheParser.ParseCards(); //Loads all the cards in the game

        //Create players
        Globals.p1 = new Player(1);
        Globals.cpu = new Player(2, true);

        //Play Jakenpo (Rock paper scissors) to see who goes first
        curr_player = Globals.p1.ID; //Player 1 goes first
        //^^^^^^^^ TEMPORARY^^^^^^^^

        //Preload the player Decks
        //This can be done from a fileinside the player computer
        //or if an accoutn system is implemented, use the default deck instead
        Globals.p1.LoadDeck();
        Globals.cpu.LoadDeck();

        SpriteManager.PreloadSprites(Globals.p1, Globals.cpu);

        //Preload decks
        Globals.p1.RandomizeDeck();
        Globals.cpu.RandomizeDeck();

        Globals.p1_cards = Globals.p1.GetAllPlayerCards(); //Loads all the cards for ease of access
        Globals.cpu_cards = Globals.cpu.GetAllPlayerCards();

        phase = GamePhase.GameStart;
    }

    // Update is called once per frame
    void Update()
    {
        if (debug && firstRun)
        {
            foreach (Transform t in board.allpositions)
                GameAnimator.InstatiatePlayedCard(Globals.p1, Globals.p1.Deck[0], t);

            timer.Wait(400);
            firstRun = false;
        }

        if (shouldRun)
        {
            switch (phase)
            {
                case GamePhase.GameStart: GameStart(); break;
                case GamePhase.StandbyPhase: StandbyPhase(); break;
                case GamePhase.MainPhase1: break;
                case GamePhase.BattlePhase: break;
                case GamePhase.BP_BattleStep: break;
                case GamePhase.BP_DamageStep: break;
                case GamePhase.BP_EndStep: break;
                case GamePhase.MainPhase2: break;
                case GamePhase.EndPhase: EndPhase(); break;
            }
        }
        else
        {
            timer.Tick();
        }
    }

    static void ChangePlayer()
    {
        if (curr_player == Globals.p1.ID)
            curr_player = Globals.cpu.ID;
        else
            curr_player = Globals.p1.ID;
    }

    static void GameStart()
    {
        if (curr_player == Globals.p1?.ID)
        {
            if (Globals.p1.Hand.GetCardCount() < 5)
            {
                GameAnimator.InstatiateCard(Globals.p1, Globals.p1.DrawCard());
                HandManager.ArrangeHand(Globals.p1);
                timer.Wait(400);
            }
            else
                ChangePlayer();
        }
        else
        {
            if (curr_player == Globals.cpu?.ID && Globals.cpu.Hand.GetCardCount() < 5)
            {
                GameAnimator.InstatiateCard(Globals.cpu, Globals.cpu.DrawCard());
                HandManager.ArrangeHand(Globals.cpu);
                timer.Wait(400);
            }
        }

        if (Globals.p1?.Hand.GetCardCount() == 5 && Globals.cpu?.Hand.GetCardCount() == 5)
        {
            HandManager.ArrangeHand(Globals.p1);
            HandManager.ArrangeHand(Globals.cpu);
            ChangePlayer();
            phase = GamePhase.StandbyPhase;
        }
        //Draw 5 cards player 1
        //Draw 5 cards player 2
    }

    static void StandbyPhase()
    {
        //Process the cards the current player can play (maybe process both players since there are cards that can be activated during DamageStep)
        if(curr_player == Globals.p1.ID)
        {
            //Add to new method and call it (repeating code)
            foreach(Card card in Globals.p1.Hand.GetCards())
            {
                //Mark each card as playable and which play type or not
                switch (card._cardType)
                {
                    case "monster": card._playType = PlayType.Summon; break;
                    case "spell": //Spell
                    case "normal": card._playType = PlayType.Activate; break; //Spell
                    case "fusion": card._playType = PlayType.Fusion; break;
                }
            }
        }
        else
        {
            foreach (Card card in Globals.cpu.Hand.GetCards())
            {
                //Mark each card as playable and which play type or not
                //Mark each card as playable and which play type or not
                switch (card._cardType)
                {
                    case "monster": card._playType = PlayType.Summon; break;
                    case "spell": //Spell
                    case "normal": card._playType = PlayType.Activate; break; //Spell
                    case "fusion": card._playType = PlayType.Fusion; break;
                }
            }
        }
    }
    
    static void EndPhase()
    {
        ChangePlayer();
        phase = GamePhase.DrawPhase;
    }
}