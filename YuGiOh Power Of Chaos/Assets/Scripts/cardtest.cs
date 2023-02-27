using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardtest : MonoBehaviour
{
    [Range(0, 7)]
    public int maxCards = 0;

    int numCards = 0;
    
    [SerializeField]
    public GameObject playedPrefab;
    [SerializeField]
    public GameObject handPrefab;
    [SerializeField]
    bool arrange = false;
    [SerializeField]
    public bool player1 = true;
    
    Transform handPos;

    List<GameObject> handTracker = new List<GameObject>();

    int prevCards = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //DrawCards();
        //ArrangeCards();
    }

    void DrawCards()
    {
        if (player1)
            handPos = GameObject.Find("player1_hand").transform;
        else
            handPos = GameObject.Find("player2_hand").transform;

        if (numCards < maxCards)
        {
            GameObject card = Instantiate(handPrefab, handPos.position, Quaternion.identity);
            card.transform.SetParent(handPos);
            handTracker.Add(card);
            numCards++;
        }

        //If maxCards less than numCards remove cards
        if (numCards > maxCards)
        {
            Destroy(handTracker[handTracker.Count - 1]);
            handTracker.RemoveAt(handTracker.Count - 1);
            numCards--;
        }        
    }

    void ArrangeCards()
    {
        if (prevCards != numCards)
        {
            //HandManager.ArrangeHand(handTracker, handPos);
            arrange = false;
            prevCards = numCards;
        }
    }
}
