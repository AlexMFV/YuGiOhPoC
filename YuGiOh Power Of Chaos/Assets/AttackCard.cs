using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class AttackCard : MonoBehaviour
{
    public bool isSelected;
    public GameObject parent;
    public GameObject target;
    public bool isAttacking;
    public bool hitTarget;
    public bool processedTarget;
    float speed = 1f;
    float increase = 1.04f;
    public Sprite final_sprite;

    // Start is called before the first frame update
    void Start()
    {
        final_sprite = Resources.Load<Sprite>("UI/attacked");
        isSelected = false;
        isAttacking = false;
        hitTarget = false;
        processedTarget = false;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitTarget)
        {
            if (isSelected)
            {
                if (target == null)
                {
                    //Rotate 2d sprite to rotate and follow cursor
                    LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
                else
                {
                    if(target == parent)
                        target = null;

                    if (!processedTarget)
                    {
                        LookAt(target.transform.position);
                        processedTarget = true;
                    }
                    
                    if (processedTarget)
                    {
                        if (isAttacking)
                        {
                            //Move forward with exponential speed until it reaches the target position
                            float step = speed * Time.deltaTime;
                            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                            if (transform.position == target.transform.position)
                            {
                                hitTarget = true;
                                isAttacking = false;
                                isSelected = false;
                                this.GetComponent<SpriteRenderer>().sprite = final_sprite;
                            }
                            speed *= increase;
                        }
                    }
                }
            }
            else
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void LookAt(Vector3 lookAt)
    {
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(lookAt.x - transform.position.x, lookAt.y - transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
