using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource source;
    public AudioSource stageMusic;
    [SerializeField] AudioClip moveCard;
    [SerializeField] AudioClip playCard;
    [SerializeField] AudioClip takeDamage;
    [SerializeField] AudioClip attackCard;
    [SerializeField] AudioClip startGame;
    [SerializeField] AudioClip losingHP;
    [SerializeField] AudioClip completeHP;
    [SerializeField] AudioClip stage1;
    [SerializeField] AudioClip stage2;
    [SerializeField] AudioClip nextTurn;
    [SerializeField] AudioClip battlePhase;

    // Start is called before the first frame update
    void Start()
    {
        //source = GetComponent<AudioSource>();
        //stageMusic = GetComponent<AudioSource>();
        //Change stageMusic to another audio source
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCard()
    {
        //source.clip = moveCard;
        source.PlayOneShot(moveCard);
    }

    public void PlayCard()
    {
        //source.clip = playCard;
        //source.Play();
        source.PlayOneShot(playCard);
    }

    public void TakeDamage()
    {
        source.PlayOneShot(takeDamage);
    }

    public void AttackCard()
    {
        source.PlayOneShot(attackCard);
    }

    public void StartGame()
    {
        source.volume = 0.3f;
        source.PlayOneShot(startGame);
    }

    public void LosingHP()
    {
        source.clip = losingHP;
        source.loop = true;
        source.volume = 1.0f;
        source.Play();
    }

    public void BattlePhase()
    {
        source.PlayOneShot(battlePhase);
    }

    public void NextTurn()
    {
        source.PlayOneShot(nextTurn);
    }

    public void CompleteHP()
    {
        source.Stop();
        source.loop = false;
        source.volume = 0.3f;
        source.PlayOneShot(completeHP);
    }

    public void Stage1Music()
    {
        stageMusic.clip = stage1;
        stageMusic.volume = 0.3f;
        stageMusic.loop = true;
        stageMusic.Play();
    }

    public void Stage2Music()
    {
        stageMusic.clip = stage2;
        stageMusic.volume = 0.3f;
        stageMusic.loop = true;
        stageMusic.Play();
    }
}
