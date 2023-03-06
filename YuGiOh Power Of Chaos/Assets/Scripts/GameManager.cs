using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool shouldRun = true;
    static GameTimer timer;
    static BoardManager board;

    static int curr_player = -1;

    static bool firstRun = true;
    static bool debug = false;

    static SoundManager sound;
    static GameObject phaseObj;
    static GameObject deckCountObj;

    // Start is called before the first frame update
    void Start()
    {
        phaseObj = GameObject.Find("Phase");
        deckCountObj = GameObject.Find("Deck_Count");
        
        sound = Camera.main.GetComponent<SoundManager>();
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
        //CHANGE THE CURR_PLAYER TO USE THE REFERENCE OF THE PLAYER INSTEAD OF THE ID (EASIER TO ACESS AN REDUCE REUSED CODE)

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

        Globals.currentPhase = GamePhase.GameStart;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePhase();
        UpdateDeckCount();

        if (debug && firstRun)
        {
            //Show all card spots
            //foreach (Transform t in board.allpositions)
            //    GameAnimator.InstatiatePlayedCard(Globals.p1, Globals.p1.Deck[0], t);

            //Testing with damage text
            //int attack = 3000;
            //int def = 450;
            //GameObject obj = TextManager.TakeDamage(Globals.p1, (attack-def)*-1);
            //GameObject obj2 = TextManager.TakeDamage(Globals.cpu, (attack-def)*-1);

            timer.Wait(400);
            firstRun = false;
        }

        if (shouldRun)
        {
            switch (Globals.currentPhase)
            {
                case GamePhase.GameStart: GameStart(); break;
                case GamePhase.DrawPhase: DrawPhase(); break;
                case GamePhase.StandbyPhase: StandbyPhase(); break;
                case GamePhase.MainPhase1: MainPhase1(); break;
                case GamePhase.BattlePhase: BattlePhase(); break;
                case GamePhase.BP_BattleStep: BattleStep(); break;
                case GamePhase.BP_DamageStep: DamageStep(); break;
                case GamePhase.BP_EndStep: EndStep(); break;
                case GamePhase.MainPhase2: MainPhase2();  break;
                case GamePhase.EndPhase: EndPhase(); break;
            }
        }
        else
        {
            timer.Tick();
        }
    }

    static void UpdatePhase()
    {
        TextMeshProUGUI text = phaseObj.GetComponent<TextMeshProUGUI>();
        text.text = Enum.GetName(typeof(GamePhase), Globals.currentPhase);
    }
    
    static void UpdateDeckCount()
    {
        TextMeshProUGUI text = deckCountObj.GetComponent<TextMeshProUGUI>();
        text.text = Globals.p1.Deck.Count().ToString();
    }

    static void ChangePlayer()
    {
        timer.Wait(100);
        
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
                sound.DrawCard();
                HandManager.ArrangeHand(Globals.p1);
                timer.Wait(350);
            }
            else
                ChangePlayer();
        }
        else
        {
            if (curr_player == Globals.cpu?.ID && Globals.cpu.Hand.GetCardCount() < 5)
            {
                GameAnimator.InstatiateCard(Globals.cpu, Globals.cpu.DrawCard());
                sound.DrawCard();
                HandManager.ArrangeHand(Globals.cpu);
                timer.Wait(350);
            }
            else
                ChangePlayer();
        }

        if (Globals.p1?.Hand.GetCardCount() == 5 && Globals.cpu?.Hand.GetCardCount() == 5)
        {
            HandManager.ArrangeHand(Globals.p1);
            HandManager.ArrangeHand(Globals.cpu);
            ChangePlayer();
            Globals.currentPhase = GamePhase.DrawPhase;
            timer.Wait(1000);
        }
    }

    static void DrawPhase()
    {
        Player player;
        if (curr_player == Globals.p1.ID)
            player = Globals.p1;
        else
            player = Globals.cpu;
        
        GameAnimator.InstatiateCard(player, player.DrawCard());
        sound.DrawCard();
        HandManager.ArrangeHand(player);

        Globals.currentPhase = GamePhase.StandbyPhase;
        timer.Wait(500);
    }

    static void StandbyPhase()
    {
        Player player;
        Globals.canPlayCard = true;

        //Process the cards the current player can play (maybe process both players since there are cards that can be activated during DamageStep)
        if (curr_player == Globals.p1.ID)
            player = Globals.p1;
        else
            player = Globals.cpu;

        //Add to new method and call it (repeating code)
        foreach (Card card in player.Hand.GetCards())
        {
            ////Mark each card as playable and which play type or not
            //switch (card._cardType)
            //{
            //    case "monster": card.PlayType = PlayType.Summon; break;
            //    case "spell": //Spell
            //    case "normal": card.PlayType = PlayType.Activate; break; //Spell
            //    case "fusion": card.PlayType = PlayType.Fusion; break;
            //}


            if (card._cardType == "monster")
            {
                card.PlayType = Helpers.checkStarRating(player, card);
            }

            if(card._cardType == "spell")
            {
                card.PlayType = Helpers.checkSpellType(player, card);
            }

            if(card._cardType == "trap")
            {
                card.PlayType = Helpers.checkTrapType(player, card);
            }
        }

        Globals.currentPhase = GamePhase.MainPhase1;
        //Globals.canPlayCard = true;
    }

    static void MainPhase1()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BattlePhase;

        if (curr_player == Globals.p1.ID)
        {
            //Player
            //If a card is hit and the user presses the button
            if(Input.GetMouseButtonDown(0) && Globals.isCardHit)
            {
                Card pressedCard = Globals.p1.Hand.GetCard(Globals.hitCard._id);

                //And if the card belongs to the player
                if (pressedCard != null)
                {
                    if (Globals.p1.CanPlayCard(pressedCard) && pressedCard.PlayType != PlayType.NotPlayable)
                    {
                        Card card = Globals.p1.PlayCard(pressedCard._id);

                        if (card != null)
                        {
                            if (card._cardType != "spell" && card._cardType != "trap")
                                Globals.canPlayCard = false;

                            Destroy(card.Object);
                            GameAnimator.InstatiatePlayedCard(Globals.p1, card, Globals.p1.GetCardPosition(card));
                            sound.PlayCard();
                            HandManager.ArrangeHand(Globals.p1);
                        }
                    }
                }
            }
            else
            {
                if(Input.GetMouseButtonDown(1) && Globals.isCardHit)
                {
                    Globals.hitCard.isSet = !Globals.hitCard.isSet;
                    Globals.isDefaultCursor = false;
                }
            }
        }
        else
        {
            //Bot
        }
    }

    void BattlePhase()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BP_BattleStep;
    }

    void BattleStep()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BP_DamageStep;
    }

    void DamageStep()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BP_EndStep;
    }

    void EndStep()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.MainPhase2;
    }

    void MainPhase2()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.EndPhase;
    }
    
    static void EndPhase()
    {
        ChangePlayer();
        Globals.currentPhase = GamePhase.DrawPhase;
    }
}