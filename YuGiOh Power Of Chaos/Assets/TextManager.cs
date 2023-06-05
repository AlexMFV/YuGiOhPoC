using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;

public static class TextManager
{
    public static GameObject TakeDamage(Player player, int damage)
    {
        GameObject parent;
        GameObject newChild = null;

        if (player.ID == 1)
            parent = GameObject.Find("p_health_label");
        else
            parent = GameObject.Find("cpu_health_label");

        //Check if a GameObject with the name DamageText already exists inside the parent, if it does use that one instead
        foreach (Transform child in parent.transform)
            if (child.name == "DamageText")
                newChild = child.gameObject;

        //Otherwise create a new one
        if (newChild == null)
            newChild = new GameObject("DamageText");

        //Create a new GameObject
        GameObject final = SetText(newChild, parent, damage.ToString(), "Font/DamageFont", true, customScale: new Vector3(4f, 4f, 1f), sizeDelta: new Vector2(50f, 10f));

        return final;
    }

    private static GameObject SetText(GameObject textObj, GameObject parent, string text, string fontName, bool useParent = false, Vector3? customPos = null, Vector3? customScale = null, Vector2? sizeDelta = null)
    {
        if (useParent)
            textObj.transform.parent = parent.transform;

        if (customPos != null)
            textObj.transform.localPosition = (Vector3)customPos;
        else
            textObj.transform.localPosition = new Vector3(0f, 0f, 0f);

        if (customScale != null)
            textObj.transform.localScale = (Vector3)customScale;
        else
            textObj.transform.localScale = new Vector3(0f, 0f, 0f);

        //Set the text
        Text textMesh;
        if (textObj.GetComponent<Text>() == null)
            textMesh = textObj.AddComponent<Text>();
        else
            textMesh = textObj.GetComponent<Text>();

        textMesh.text = text.ToString();
        textMesh.alignment = TextAnchor.UpperCenter;
        //Set the font to DamageFont
        textMesh.font = Resources.Load<Font>(fontName);

        if (sizeDelta != null)
        {
            //Add the TextMesh component
            RectTransform recttrans = textObj.GetComponent<RectTransform>();
            recttrans.sizeDelta = (Vector2)sizeDelta;
        }

        return textObj;
    }
}
