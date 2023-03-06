using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D c_default;
    [SerializeField] Texture2D c_clicked;
    [SerializeField] GameObject c_clickAnimation;

    [SerializeField] Texture2D c_summon;
    [SerializeField] Texture2D c_set;
    [SerializeField] Texture2D c_activate;
    [SerializeField] Texture2D c_fusion;

    //TMP Texts
    [SerializeField] TextMeshProUGUI tmp_title;
    [SerializeField] TextMeshProUGUI tmp_attributes;
    [SerializeField] TextMeshProUGUI tmp_description;
    [SerializeField] GameObject image_card;

    //Usable texts
    static TextMeshProUGUI card_title;
    static TextMeshProUGUI card_attributes;
    static TextMeshProUGUI card_description;
    static GameObject card_image;

    bool isClicked = false;
    Vector3 offset;
    List<CursorType> cursors;

    // Start is called before the first frame update
    void Start()
    {
        //NOTE: Offset is different when the surrender cursor is selected
        offset = new Vector3(6, 11, 0);
        
        //Set default cursor
        Cursor.SetCursor(c_default, offset, CursorMode.ForceSoftware);
        LoadUI();
    }

    // Update is called once per frame
    void Update()
    {
        MouseRaycast();
        MouseHover();
        MouseClicked();
    }

    void MouseClicked()
    {
        bool isHeldDown = Input.GetMouseButton(0);      //Left Click

        if(isHeldDown && !isClicked && Globals.isDefaultCursor)
        {
            Cursor.SetCursor(c_clicked, offset, CursorMode.ForceSoftware);
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vec.z = 0;
            Instantiate(c_clickAnimation, vec, Quaternion.identity);
            isClicked = true;
        }

        if(!isHeldDown && isClicked)
        {
            Cursor.SetCursor(c_default, offset, CursorMode.ForceSoftware);
            isClicked = false;
            Globals.isDefaultCursor = true;
        }
    }

    void MouseHover()
    {
        if (Globals.isCardHit)
        {
            if (!Globals.isPlayedCard)
                CursorCardHover();
            else
                CursorPlayedCardHover();
                
            CardHover();
        }
        else
        {
            if (!Globals.isDefaultCursor)
            {
                Cursor.SetCursor(c_default, offset, CursorMode.ForceSoftware);
                Globals.isDefaultCursor = true;
            }
        }
    }

    public void CardHover()
    {
        //CHANGES: Maybe for card hover during Standby Phase and processing of the Hand create a List that goes through all the cards in the hand
        //and adds to the list the cursor it should show, then during hover it goes through the list and shows the cursor that is in the list.

        //Only show description for faceup cards
        if (Globals.hitCard._faceup || Globals.p1_cards.TryGetValue(Globals.hitCard._id, out Card x))
        {
            card_image.GetComponent<SpriteRenderer>().sprite = Globals.Sprites[Globals.hitCard._imageName];
            card_title.text = Globals.hitCard._name;
            string attribs = string.Join('/', Globals.hitCard._types.Select(x => char.ToUpperInvariant(x[0]) + x.Substring(1)).ToArray()) ?? "[No attribute]";

            if (Globals.hitCard._cardType != "monster" && Globals.hitCard._cardType != "fusion")   //Later use enumerator instead
                ((RectTransform)card_attributes.GetComponent<RectTransform>()).gameObject.SetActive(false);
            else
                ((RectTransform)card_attributes.GetComponent<RectTransform>()).gameObject.SetActive(true);

            card_attributes.text = $"[{attribs}]";
            card_description.text = Globals.hitCard._description;
        }
        else
        {
            card_image.GetComponent<SpriteRenderer>().sprite = Globals.Sprites["card_ura"];
            card_title.text = "";
            card_attributes.text = "";
            card_description.text = "";
        }
    }

    //Changes the cursor 
    public void CursorCardHover()
    {
        if (Globals.canPlayCard)
        {
            if (!Globals.hitCard.isSet)
            {
                switch (Globals.hitCard.PlayType)
                {
                    case PlayType.Activate: Cursor.SetCursor(c_activate, offset, CursorMode.ForceSoftware); Globals.isDefaultCursor = false; break;
                    case PlayType.Fusion: Cursor.SetCursor(c_fusion, offset, CursorMode.ForceSoftware); Globals.isDefaultCursor = false; break;
                    case PlayType.Summon: Cursor.SetCursor(c_summon, offset, CursorMode.ForceSoftware); Globals.isDefaultCursor = false; break;
                    case PlayType.Set: Cursor.SetCursor(c_set, offset, CursorMode.ForceSoftware); Globals.isDefaultCursor = false; break;
                    default: Cursor.SetCursor(c_default, offset, CursorMode.ForceSoftware); break;
                }
            }
            else
            {
                if (Globals.hitCard._cardType == "monster" && Helpers.checkStarRating(Globals.p1, Globals.hitCard) == PlayType.Summon)
                {
                    Cursor.SetCursor(c_set, offset, CursorMode.ForceSoftware);
                    Globals.isDefaultCursor = false;
                }
            }
        }
        else
        {
            if (Globals.hitCard._cardType == "spell" || Globals.hitCard._cardType == "trap")
            {
                if (!Globals.hitCard.isSet)
                {
                    switch (Globals.hitCard.PlayType)
                    {
                        case PlayType.Activate: Cursor.SetCursor(c_activate, offset, CursorMode.ForceSoftware); Globals.isDefaultCursor = false; break;
                        default: Cursor.SetCursor(c_default, offset, CursorMode.ForceSoftware); break;
                    }
                }
                else
                {
                    Cursor.SetCursor(c_set, offset, CursorMode.ForceSoftware);
                    Globals.isDefaultCursor = false;
                }
            }
            else
                Cursor.SetCursor(c_default, offset, CursorMode.ForceSoftware);
        }
    }
    
    public void CursorPlayedCardHover() { }

    #region Misc

    void MouseRaycast()
    {
        Globals.hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (Globals.hit.collider?.gameObject.tag == "Card" || Globals.hit.collider?.gameObject.tag == "PlayedCard")
        {
            Globals.isPlayedCard = Globals.hit.collider.gameObject.tag == "PlayedCard";
            
            //Instead of card sprite we should be getting the corresponding Card object and load the title and description also
            string tag = Globals.hit.collider.gameObject.name;
            Card prevHit = Globals.hitCard;
            Globals.hitCard = GetCard(tag);

            if (prevHit != Globals.hitCard)
                Globals.p1.Hand.ClearSet();

            if (Globals.hitCard != null)
                Globals.isCardHit = true;
        }
        else
        {
            Globals.hitCard = null;
            Globals.p1.Hand.ClearSet();
            Globals.isCardHit = false;
        }
    }

    Card GetCard(string prefabTag)
    {
        string[] splits = prefabTag.Split(':');

        if (splits.Count() > 1)
        {
            int player = int.Parse(splits[0]);
            Guid guid = Guid.Parse(splits[1]);

            Card card = null;
            
            if (player == Globals.p1.ID)
                Globals.p1_cards.TryGetValue(guid, out card);
            else
                Globals.cpu_cards.TryGetValue(guid, out card);

            return card;
        }
        return null;
    }

    void LoadUI()
    {
        card_title = tmp_title;
        card_attributes = tmp_attributes;
        card_description = tmp_description;
        card_image = image_card;
    }

    public enum CursorType
    {
        DEFAULT,
        SUMMON,
        SET,
        DRAW,
        FUSION,
        ACTIVATE,
        ATTACK,
        SURRENDER,
        FLIP_SUMMON,
        ERROR,
        REVERSE,
        ATTACK_POSITION,
        DEFENSE_POSITION
    }

    #endregion
}
