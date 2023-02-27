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
    static RaycastHit2D hit;

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
        bool isHeldDown = Input.GetMouseButton(0);

        if(isHeldDown && !isClicked)
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
        }
    }

    void MouseHover()
    {
        CardHover();
    }

    public void CardHover()
    {
        if (hit.collider?.gameObject.tag == "Card")
        {
            //Instead of card sprite we should be getting the corresponding Card object and load the title and description also
            string tag = hit.collider.gameObject.name;
            Card card = GetCard(tag);

            //Only show description for faceup cards
            if (card != null && (card._faceup || Globals.p1_cards.TryGetValue(card._id, out Card x)))
            {
                card_image.GetComponent<SpriteRenderer>().sprite = Globals.Sprites[card._imageName];
                card_title.text = card._name;
                string attribs = string.Join('/', card._types.Select(x => char.ToUpperInvariant(x[0]) + x.Substring(1)).ToArray()) ?? "[No attribute]";

                if (card._cardType != "monster" && card._cardType != "fusion")   //Later use enumerator instead
                    ((RectTransform)card_attributes.GetComponent<RectTransform>()).gameObject.SetActive(false);
                else
                    ((RectTransform)card_attributes.GetComponent<RectTransform>()).gameObject.SetActive(true);

                card_attributes.text = $"[{attribs}]";
                card_description.text = card._description;
            }
            else
            {
                card_image.GetComponent<SpriteRenderer>().sprite = Globals.Sprites["card_ura"];
                card_title.text = "";
                card_attributes.text = "";
                card_description.text = "";
            }
        }
    }

    #region Misc

    void MouseRaycast()
    {
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
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

    #endregion
}
