using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class BoardManager
{
    List<Transform> p1_normals;
    List<Transform> p1_specials;
    List<Transform> p2_normals;
    List<Transform> p2_specials;
    Transform p1Deck;
    Transform p2Deck;
    Transform p1Graveyard;
    Transform p2Graveyard;
    Transform p1Fusion;
    Transform p2Fusion;
    Transform p1Field;
    Transform p2Field;

    public List<Transform> allpositions; //test only

    public BoardManager()
    {
        LoadBoardPoints();
    }

    private void LoadBoardPoints()
    {
        p1_normals = new List<Transform>();
        p2_normals = new List<Transform>();
        p1_specials = new List<Transform>();
        p2_specials = new List<Transform>();
        allpositions = new List<Transform>();

        //Acess the Board parent and read all the board positions
        GameObject board = GameObject.Find("Board");

        foreach (Transform obj in board.transform)
        {
            allpositions.Add(obj); //Test only
            string name = obj.name;

            if (name.StartsWith("p_"))
            {
                //Player 1
                if (name.Contains("normal"))
                    p1_normals.Add(obj);

                if (name.Contains("special"))
                    p1_specials.Add(obj);

                if (name == "p_deck")
                    p1Deck = obj;

                if (name == "p_graveyard")
                    p1Graveyard = obj;

                if (name == "p_fusion")
                    p1Fusion = obj;

                if (name == "p_field")
                    p1Field = obj;
            }
            else
            {
                //Player 2
                if (name.Contains("normal"))
                    p2_normals.Add(obj);

                if (name.Contains("special"))
                    p2_specials.Add(obj);

                if (name == "cpu_deck")
                    p2Deck = obj;

                if (name == "cpu_graveyard")
                    p2Graveyard = obj;

                if (name == "cpu_fusion")
                    p2Fusion = obj;

                if (name == "cpu_field")
                    p2Field = obj;
            }
        }
    }

    public Transform Player1Normal(int card)
    {
        if (card >= 4)
            return p1_normals[4];

        if (card <= 0)
            return p1_normals[0];

        return p1_normals[card];
    }

    public Transform Player2Normal(int card)
    {
        if (card >= 4)
            return p2_normals[4];

        if (card <= 0)
            return p2_normals[0];

        return p2_normals[card];
    }

    public Transform Player1Special(int card)
    {
        if (card >= 4)
            return p1_specials[4];

        if (card <= 0)
            return p1_specials[0];

        return p1_specials[card];
    }

    public Transform Player2Special(int card)
    {
        if (card >= 4)
            return p2_specials[4];

        if (card <= 0)
            return p2_specials[0];

        return p2_specials[card];
    }

    public Transform Player1Deck { get { return p1Deck; } }

    public Transform Player2Deck { get { return p2Deck; } }

    public Transform Player1Graveyard { get { return p1Graveyard; } }

    public Transform Player2Graveyard { get { return p2Graveyard; } }

    public Transform Player1Fusion { get { return p1Fusion; } }

    public Transform Player2Fusion { get { return p2Fusion; } }

    public Transform Player1Field { get { return p1Field; } }

    public Transform Player2Field { get { return p2Field; } }
}
