using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TextManager
{
    public static GameObject TakeDamage(Player player, int damage)
    {
        GameObject parent;

        if (player.ID == 1)
            parent = GameObject.Find("p_health_label");
        else
            parent = GameObject.Find("cpu_health_label");

        //Create a new GameObject
        GameObject text = new GameObject("DamageText");
        text.transform.position = new Vector3(0f, 0f, 0f);
        text.transform.SetParent(parent.transform);
        text.transform.localScale = new Vector3(4f, 4f, 1f);
        
        //Add the TextMesh component
        Text textMesh = text.AddComponent<Text>();
        RectTransform recttrans = text.GetComponent<RectTransform>();

        recttrans.sizeDelta = new Vector2(50f, 10f);
        textMesh.transform.position = new Vector3(0f, 0f, 0f);

        //Set the text
        textMesh.text = damage.ToString();
        textMesh.alignment = TextAnchor.UpperCenter;
        //Set the font to DamageFont
        textMesh.font = Resources.Load<Font>("Font/DamageFont");

        return text;
    }
}
