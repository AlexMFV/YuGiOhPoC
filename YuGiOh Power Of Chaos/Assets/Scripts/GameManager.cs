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
    //static int CARD_ID_TRACKER = 0;
    static Player p1; //User
    static Player cpu; //Yugi
    static GamePhase phase;
    public static bool shouldRun = true;
    static GameTimer timer;
    static BoardManager board;

    //TMP Texts
    public TextMeshProUGUI tmp_title;
    public TextMeshProUGUI tmp_attributes;
    public TextMeshProUGUI tmp_description;

    //Usable texts
    static TextMeshProUGUI card_title;
    static TextMeshProUGUI card_attributes;
    static TextMeshProUGUI card_description;

    private bool prevShowCards = false;
    public bool showCards = false;

    static int curr_player = -1;

    static bool firstRun = true;
    static bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        LoadUI();
        
        board = new BoardManager();
        timer = new GameTimer();

        //This should be done once the game is started and is loading
        CacheParser.ParseCards(); //Loads all the cards in the game

        //Create players
        p1 = new Player(1);
        cpu = new Player(2, true);

        //Play Jakenpo (Rock paper scissors) to see who goes first
        curr_player = p1.ID; //Player 1 goes first
        //^^^^^^^^ TEMPORARY^^^^^^^^

        //Preload the player Decks
        //This can be done from a fileinside the player computer
        //or if an accoutn system is implemented, use the default deck instead
        p1.LoadDeck();
        cpu.LoadDeck();

        SpriteManager.PreloadSprites(p1, cpu);

        //Preload decks
        p1.RandomizeDeck();
        cpu.RandomizeDeck();

        phase = GamePhase.GameStart;
    }

    // Update is called once per frame
    void Update()
    {
        if (debug && firstRun)
        {
            foreach (Transform t in board.allpositions)
                GameAnimator.InstatiatePlayedCard(p1, new Card(), t);

            timer.Wait(400);
            firstRun = false;
        }

        CursorChecks();

        if (shouldRun)
        {
            switch (phase)
            {
                case GamePhase.GameStart: GameStart(); break;
                case GamePhase.StandbyPhase: break;
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


        //-----Everything below this needs to be changed-----
        //If we are loading for the deck construction scene load all the UNLOCKED CARDS but the mini version (less RAM usage)
        if (showCards)
        {
            foreach (Card card in p1?.Hand.GetCards())
            {
                card.Object.GetComponent<SpriteRenderer>().sprite = Globals.Sprites[card._imageName];
            }
        }
        else
        {
            foreach (Card card in p1?.Hand.GetCards())
                card.Object.GetComponent<SpriteRenderer>().sprite = Globals.Sprites["card_ura"];
        }
    }

    public static void CursorChecks()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Card")
            {
                GameObject obj = GameObject.Find("card_image");
                //Instead of card sprite we should be getting the corresponding Card object and load the title and description also
                string tag = hit.collider.gameObject.name;
                Card card = GetCard(tag);
                obj.GetComponent<SpriteRenderer>().sprite = Globals.Sprites[card._imageName];
                card_title.text = card._name;
                string attribs = string.Join('/', card._types.Select(x => char.ToUpperInvariant(x[0]) + x.Substring(1)).ToArray()) ?? "[No attribute]";

                if (card._cardType != "monster" && card._cardType != "fusion")   //Later use enumerator instead
                    ((RectTransform)card_attributes.GetComponent<RectTransform>()).gameObject.SetActive(false);
                else
                    ((RectTransform)card_attributes.GetComponent<RectTransform>()).gameObject.SetActive(true);
                
                card_attributes.text = $"[{attribs}]";
                card_description.text = card._description;
            }
        }
    }

    static Card GetCard(string prefabTag)
    {
        string[] splits = prefabTag.Split(':');

        if (splits.Count() > 1)
        {
            int player = int.Parse(splits[0]);
            Guid guid = Guid.Parse(splits[1]);

            if (player == p1.ID)
                return p1.FindCard(guid);
            else
                return cpu.FindCard(guid);
        }
        return null;
    }

    //public static int getNewCardID()
    //{
    //    CARD_ID_TRACKER += 1;
    //    return CARD_ID_TRACKER;
    //}

    static void ChangePlayer()
    {
        if (curr_player == p1.ID)
            curr_player = cpu.ID;
        else
            curr_player = p1.ID;
    }

    static void GameStart()
    {
        if (curr_player == p1?.ID)
        {
            if (p1.Hand.GetCardCount() < 5)
            {
                GameAnimator.InstatiateCard(p1, p1.DrawCard());
                HandManager.ArrangeHand(p1);
                timer.Wait(400);
            }
            else
                ChangePlayer();
        }
        else
        {
            if (curr_player == cpu?.ID && cpu.Hand.GetCardCount() < 5)
            {
                GameAnimator.InstatiateCard(cpu, cpu.DrawCard());
                HandManager.ArrangeHand(cpu);
                timer.Wait(400);
            }
        }

        if (p1?.Hand.GetCardCount() == 5 && cpu?.Hand.GetCardCount() == 5)
        {
            HandManager.ArrangeHand(p1);
            HandManager.ArrangeHand(cpu);
            ChangePlayer();
            phase = GamePhase.DrawPhase;
        }
        //Draw 5 cards player 1
        //Draw 5 cards player 2
    }

    static void EndPhase()
    {
        ChangePlayer();
        phase = GamePhase.DrawPhase;
    }

    void LoadUI()
    {
        card_title = tmp_title;
        card_attributes = tmp_attributes;
        card_description = tmp_description;
    }
}