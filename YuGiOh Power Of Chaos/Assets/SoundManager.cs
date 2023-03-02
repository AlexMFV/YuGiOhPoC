using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static AudioSource source;
    [SerializeField] AudioClip moveCard;
    [SerializeField] AudioClip playCard;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCard()
    {
        source.clip = moveCard;
        source.Play();
    }

    public void PlayCard()
    {
        source.clip = playCard;
        source.Play();
    }
}
