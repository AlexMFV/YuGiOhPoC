using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateCursorClick : MonoBehaviour
{
    public float speed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale += (Vector3.one * speed) * Time.deltaTime;
        Color tmp = this.GetComponent<SpriteRenderer>().color;
        tmp.a -= (this.transform.localScale.x * 2f) * Time.deltaTime;
        this.GetComponent<SpriteRenderer>().color = tmp;

        if(tmp.a <= 0)
            Destroy(this.gameObject);
    }
}
