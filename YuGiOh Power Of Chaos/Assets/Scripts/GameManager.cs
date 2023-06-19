using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool shouldRun = true;
    public static GameTimer timer;
    static BoardManager board;
    public static bool hpFirstRun = true;

    static Player curr_player = null;

    static bool firstRun = true;
    static bool debug = true;

    static bool isAttackSelected = false;
    static GameObject attackSelected;
    static List<GameObject> attackObjs;

    public static SoundManager sound;
    static GameObject phaseObj;
    static GameObject deckCountObj;

    public GameObject attackVectorObj;
    public GameObject damageIndicator;

    // Start is called before the first frame update
    void Start()
    {
        phaseObj = GameObject.Find("Phase");
        deckCountObj = GameObject.Find("Deck_Count");
        
        sound = Camera.main.GetComponent<SoundManager>();
        board = new BoardManager();
        timer = new GameTimer();
        attackObjs = new List<GameObject>();

        //This should be done once the game is started and is loading
        CacheParser.ParseCards(); //Loads all the cards in the game

        //Create players
        Globals.p1 = new Player(1);
        Globals.cpu = new Player(2, true);

        //Play Jakenpo (Rock paper scissors) to see who goes first
        curr_player = Globals.p1; //Player 1 goes first
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

        Globals.currentPhase = GamePhase.GameStart;

        TextManager.SetHP(Globals.p1, Globals.p1.Health);
        TextManager.SetHP(Globals.cpu, Globals.cpu.Health);

        timer.Wait(1500);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePhase();
        UpdateDeckCount();

        if (firstRun)
        {
            sound.StartGame();
            System.Random r = new System.Random();
            int i = r.Next(0, 2);
            if(i == 0)
                sound.Stage1Music();
            else
                sound.Stage2Music();
            //timer.Wait(1000);
            //Show all card spots
            //foreach (Transform t in board.allpositions)
            //    GameAnimator.InstantiatePlayedCard(Globals.p1, Globals.p1.Deck[0], t);

            //Testing with damage text
            //int attack = 3000;
            //int def = 450;
            //GameObject obj = TextManager.TakeDamage(Globals.p1, (attack-def)*-1);
            //GameObject obj2 = TextManager.TakeDamage(Globals.cpu, (attack-def)*-1);

            //timer.Wait(400);
            firstRun = false;
        }

        if (shouldRun)
        {
            UpdateDamage();

            switch (Globals.currentPhase)
            {
                case GamePhase.GameStart: GameStart(); break;
                case GamePhase.DrawPhase: DrawPhase(); break;
                case GamePhase.StandbyPhase: StandbyPhase(); break;
                case GamePhase.MainPhase1: MainPhase1(); break;
                case GamePhase.BattlePhase: BattlePhase(); break;
                case GamePhase.BP_StartStep: StartStep(); break;
                case GamePhase.BP_BattleStep: BattleStep(); break;
                case GamePhase.BP_DamageStep: DamageStep(); break;
                case GamePhase.BP_EndStep: EndStep(); break;
                case GamePhase.MainPhase2: MainPhase2(); break;
                case GamePhase.EndPhase: EndPhase(); break;
                case GamePhase.EndGame: EndGame(); break;
            }
        }
        else
        {
            timer.Tick();
        }
    }

    //DEBUG ONLY
    //TRY TO RUN DAMAGE INDICATOR ON ANOTHER THREAD
    static void UpdateDamage()
    {
        if (TextManager.ActiveIndicator != null)
        {
            if (hpFirstRun)
            {
                sound.LosingHP();
                hpFirstRun = false;
            }
            float test = .05f * Time.fixedDeltaTime * TextManager.ActiveIndicator.InitialDamage;

            if(test > -1)
                test = -1;

            TextManager.ActiveIndicator.Player.Health += (int)test;

            //Winning/Losing condition
            if(TextManager.ActiveIndicator.Player.Health <= 0)
            {
                TextManager.ActiveIndicator.Player.Health = 0;
                Globals.currentPhase = GamePhase.EndGame;
            }

            TextManager.SetHP(TextManager.ActiveIndicator.Player, TextManager.ActiveIndicator.Player.Health);
            TextManager.ActiveIndicator.Update((int)test, sound); //Depending on the value of damageTaken, the damage will always take 1 second to reach 0
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
        
        if (curr_player.ID == Globals.p1.ID)
            curr_player = Globals.cpu;
        else
            curr_player = Globals.p1;
    }

    static void GameStart()
    {
        if (curr_player.Hand.GetCardCount() < 5)
        {
            GameAnimator.InstantiateCard(curr_player, curr_player.DrawCard());
            sound.DrawCard();
            HandManager.ArrangeHand(curr_player);
            timer.Wait(350);
        }
        else
            ChangePlayer();

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
        GameAnimator.InstantiateCard(curr_player, curr_player.DrawCard());
        sound.DrawCard();
        HandManager.ArrangeHand(curr_player);

        Globals.currentPhase = GamePhase.StandbyPhase;
        timer.Wait(500);
    }

    static void StandbyPhase()
    {
        Globals.canPlayCard = true;

        //Add to new method and call it (repeating code)
        foreach (Card card in curr_player.Hand.GetCards())
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
                card.PlayType = Helpers.checkStarRating(curr_player, card);
            }

            if(card._cardType == "spell")
            {
                card.PlayType = Helpers.checkSpellType(curr_player, card);
            }

            if(card._cardType == "trap")
            {
                card.PlayType = Helpers.checkTrapType(curr_player, card);
            }
        }

        Globals.currentPhase = GamePhase.MainPhase1;
        //Globals.canPlayCard = true;
    }

    static void MainPhase1()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BattlePhase;

        if (curr_player.ID == Globals.p1.ID)
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
                            GameAnimator.InstantiatePlayedCard(Globals.p1, card, Globals.p1.GetCardPosition(card));
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
            //Bot round
            //Really simple random plays for testing purposes of the battle phase

            //Get Random monster card from Globals.cpu.Hand
            //Check if it can be played
            //If yes, play it
            Card cpuCard = Globals.cpu.Hand.GetCards().Where(x => Globals.cpu.CanPlayCard(x) && (x._playType == PlayType.Set || x._playType == PlayType.Summon)).FirstOrDefault();

            if (cpuCard != null && Globals.canPlayCard)
            {
                Globals.cpu.PlayCard(cpuCard._id);

                if (cpuCard._cardType != "spell" && cpuCard._cardType != "trap")
                {
                    cpuCard._faceup = true;
                    Globals.canPlayCard = false;
                }

                Destroy(cpuCard.Object);
                GameAnimator.InstantiatePlayedCard(Globals.cpu, cpuCard, Globals.cpu.GetCardPosition(cpuCard));
                sound.PlayCard();
                HandManager.ArrangeHand(Globals.cpu);

                timer.Wait(1000); //Between each card pause the game either to give time to the player to process and/or to play animations/sounds
            }
            else
                Globals.currentPhase = GamePhase.MainPhase2;
        }
    }

    //Battle Phase preparations
    void BattlePhase()
    {
        if(Globals.isFirstRound)
            Globals.currentPhase = GamePhase.MainPhase2;

        //Check all the cards that can attack
        foreach(Card c in curr_player.GetMonsterZone())
        {
            if (c._cardType == "monster" && c._faceup) //&& c._attackMode)
                c.canAttack = true;
            else
                c.canAttack = false;
        }
        
            //Monsters
            //Not in defense mode
            //Not flipped down
        //Show attack indicator on top of each card

        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BP_StartStep;
    }

    void StartStep()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BP_BattleStep;

        foreach(Card c in curr_player.GetMonsterZone())
        {
            //if this card has flag canAttack, spawn the attack_vector prefab on top of it
            if (c.canAttack)
            {
                //Spawn attack vector
                GameObject attacker = Instantiate(attackVectorObj, c.Object.transform.position, Quaternion.identity);
                attacker.transform.SetParent(c.Object.transform);
                attacker.transform.localPosition = new Vector3(0, 0, 0);
                attacker.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
                attacker.transform.localRotation = Quaternion.Euler(0, 0, 0);
                attacker.GetComponent<AttackCard>().parent = c.Object; //Append the parent to a new variable
                attacker.GetComponent<AttackCard>().card_ref = c; //Append the parent to a new variable
                attackObjs.Add(attacker);
            }
        }

        Globals.currentPhase = GamePhase.BP_BattleStep;
    }

    void BattleStep()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BP_EndStep;

        if (attackSelected != null && isAttackSelected && !attackSelected.GetComponent<AttackCard>().isSelected)
        {
            isAttackSelected = false;
            attackSelected = null;
        }

        //If this object is pressed enabled the isSelected flag, if no other object is pressed
        if (!isAttackSelected && Input.GetMouseButtonDown(0))
        {
            //If no other object is selected, select this object
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.RaycastAll(mousePos, Vector2.zero).Where(x => x.transform.tag == "Attacker").FirstOrDefault();
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Attacker" )
                {
                    isAttackSelected = true;
                    attackSelected = hit.collider.gameObject;
                    attackSelected.GetComponent<AttackCard>().isSelected = true;
                }
            }
        }

        if (isAttackSelected && Input.GetMouseButtonDown(0))
        {
            if(Globals.hitCard != null)
            {
                if (Globals.hitCard.Object != attackSelected.GetComponent<AttackCard>().parent && !curr_player.CardBelongsToPlayer(Globals.hitCard._id))
                {
                    attackSelected.GetComponent<AttackCard>().isAttacking = true;
                    attackSelected.GetComponent<AttackCard>().target = Globals.hitCard.Object;
                    Globals.permanentHitCard = Globals.hitCard;
                    isAttackSelected = false; //TODO: Probably working. Check if we can attack again after finishing an attack with another card
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            isAttackSelected = false;
            if (attackSelected != null)
            {
                attackSelected.GetComponent<AttackCard>().isSelected = false;
                attackSelected = null;
            }
        }
    }

    void DamageStep()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.BP_EndStep;

        Card source = attackSelected.GetComponent<AttackCard>().card_ref;
        Card hit = Globals.permanentHitCard;
        //sound.AttackCard();

        //We attack and play the attack sound, after that we wait 1/2 seconds and then the damage label is shown
        DamageResolver(source, hit);
    }

    void DamageResolver(Card source, Card hit)
    {
        if(source == null || hit == null)
            throw new Exception("Source or hit card is null");

        int sourceHP = source.GetPrimaryValue();
        int hitHP = hit.GetPrimaryValue();
        int damageControl;

        if (sourceHP - hitHP == 0)
        {
            damageControl = 0;
            //Should play shield animation on attacked card
            Globals.permanentHitCard = null; //Clear the card used for damage calculation
            //The attack vector prefab needs to be cleaned
            Globals.currentPhase = GamePhase.BP_BattleStep;
            timer.Wait(1500);
            return;
        }

        if(sourceHP > hitHP)
        {
            damageControl = (sourceHP - hitHP) * -1;
            Globals.cpu.TakeDamage(damageControl);

            hit.Kill(Globals.cpu);//Kill the attacked card
            Globals.permanentHitCard = null; //Clear the card used for damage calculation
            Globals.currentPhase = GamePhase.BP_BattleStep; //Not needed as we already are in the battle step
            sound.TakeDamage();
            timer.Wait(1500);
            return;
        }

        if (sourceHP < hitHP)
        {
            damageControl = hitHP - sourceHP;
            Globals.p1.TakeDamage(damageControl);

            //The attacking card is not killed, the player simply loses HP
            Globals.permanentHitCard = null; //Clear the card used for damage calculation
            Globals.currentPhase = GamePhase.BP_BattleStep;
            sound.TakeDamage();
            timer.Wait(1500);
            return;
        }
    }

    void EndStep()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.MainPhase2;

        isAttackSelected = false;

        foreach (GameObject obj in attackObjs)
            Destroy(obj);
    }

    void MainPhase2()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Globals.currentPhase = GamePhase.EndPhase;
    }
    
    static void EndPhase()
    {
        if (Globals.isFirstRound)
            Globals.isFirstRound = false;

        ChangePlayer();
        Globals.currentPhase = GamePhase.DrawPhase;
    }

    static void EndGame()
    {
        if(Globals.p1.Health <= 0)
        {
            //Player 2 wins
        }
        else
        {
            //Player 1 wins
        }
    }
}