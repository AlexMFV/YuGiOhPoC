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
    static GameTimer timer;
    static BoardManager board;

    static Player curr_player = null;

    static bool firstRun = true;
    static bool debug = true;

    static bool isAttackSelected = false;
    static GameObject attackSelected;
    static List<GameObject> attackObjs;

    static SoundManager sound;
    static GameObject phaseObj;
    static GameObject deckCountObj;

    public GameObject attackVectorObj;
    public GameObject damageIndicator;

    //DEBUG ONLY Variables
    static float damage = 500;

    // Start is called before the first frame update
    void Start()
    {
        phaseObj = GameObject.Find("Phase");
        deckCountObj = GameObject.Find("Deck_Count");
        
        sound = Camera.main.GetComponent<SoundManager>();
        board = new BoardManager();
        timer = new GameTimer();
        attackObjs = new List<GameObject>();

        //DEBUG ONLY Initialize
        //damageIndicator = Instantiate(damageIndicator);
        //damageIndicator.transform.position = new Vector2(0.0f, 0.0f);

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
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePhase();
        UpdateDeckCount();

        //DEBUG ONLY
        UpdateDamage();

        if (debug && firstRun)
        {
            //timer.Wait(1000);
            //Show all card spots
            //foreach (Transform t in board.allpositions)
            //    GameAnimator.InstatiatePlayedCard(Globals.p1, Globals.p1.Deck[0], t);

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
                case GamePhase.MainPhase2: MainPhase2();  break;
                case GamePhase.EndPhase: EndPhase(); break;
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
        TextManager.TakeDamage(Globals.p1, (int)damage);
        TextManager.TakeDamage(Globals.cpu, (int)damage);
        int damageTaken = 999;
        damage += 1 * Time.deltaTime * damageTaken; //Depending on the value of damageTaken, the damage will always take 1 second to reach 0
        if(damage > 0)
            damage = -damageTaken;
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
            GameAnimator.InstatiateCard(curr_player, curr_player.DrawCard());
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
        GameAnimator.InstatiateCard(curr_player, curr_player.DrawCard());
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
                GameAnimator.InstatiatePlayedCard(Globals.cpu, cpuCard, Globals.cpu.GetCardPosition(cpuCard));
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
            Globals.currentPhase = GamePhase.BP_DamageStep;

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
        Card hit = Globals.hitCard;

        DamageResolver(source, hit);
    }

    void DamageResolver(Card source, Card hit)
    {
        int sourceHP = source.GetPrimaryValue();
        int hitHP = hit.GetPrimaryValue();
        int damageControl;

        if (sourceHP - hitHP == 0)
        {
            damageControl = 0;
            //Should play shield animation on attacked card
            timer.Wait(1000);
            Globals.currentPhase = GamePhase.BP_BattleStep;
            return;
        }

        if(sourceHP > hitHP)
        {
            damageControl = sourceHP - hitHP;
            //Kill the attacked card
            //Deduct from the CPU's HP
            timer.Wait(1000);
            Globals.currentPhase = GamePhase.BP_BattleStep;
            return;
        }

        if (sourceHP < hitHP)
        {
            damageControl = hitHP - sourceHP;
            //Kill the attacking card
            //Deduct from the player's HP
            timer.Wait(1000);
            Globals.currentPhase = GamePhase.BP_BattleStep;
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
}