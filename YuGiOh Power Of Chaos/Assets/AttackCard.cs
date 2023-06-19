using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class AttackCard : MonoBehaviour
{
    public bool isSelected;
    public GameObject parent;
    public Card card_ref;
    public GameObject target;
    public bool isAttacking;
    public bool hitTarget;
    public bool processedTarget;
    float speed = 1f;
    float increase = 1.025f;
    public Sprite final_sprite;
    public bool playedSound = false;

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

                            //TODO: This is fucked, we calculate a small distance between both vectors to allow the sound to play before
                            //the vectors meet. This is because if we play the sound ONLY when they meet the sound will be delayed.
                            if (!playedSound && Vector3.Distance(transform.position, target.transform.position) < 15f)
                            {
                                playedSound = true;
                                GameManager.sound.AttackCard();
                            }

                            if (transform.position == target.transform.position)
                            {
                                playedSound = false;
                                hitTarget = true;
                                isAttacking = false;
                                isSelected = false;
                                this.transform.SetParent(target.transform);
                                this.GetComponent<SpriteRenderer>().sprite = final_sprite;
                                Globals.currentPhase = GamePhase.BP_DamageStep;
                                GameManager.timer.Wait(1500);
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
