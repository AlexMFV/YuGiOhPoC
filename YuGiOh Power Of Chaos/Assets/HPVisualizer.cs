using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPVisualizer : MonoBehaviour
{
    public GameObject visualizer;

    float maxHeight = -5.0f; //Full HP
    //float minHeight = -55.0f; //No HP

    // Start is called before the first frame update
    void Start()
    {

    }

    //TODO: Save the previous HP, if its lower, then change the type of animation
    //To do this, we need to save the previous HP, and then compare it to the current one, and also save the current animation state and for how long it has been running
    //so that we can update the animation speed as needed.

    // Update is called once per frame
    void Update()
    {
        UpdateHPLevel(); //Moves up and down depending on HP (TODO: needs to have into account player ID)
        ContinuousMove(); //Moves continuously to the left (might break on other resolutions)
    }

    void UpdateHPLevel()
    {
        int health = Globals.cpu.Health > 8000 ? 8000 : Globals.cpu.Health;
        float fraction = (int)(45 * (health / 8000.0f));
        float height = maxHeight - (45 - fraction);

        visualizer.transform.localPosition = new Vector2(visualizer.transform.localPosition.x, height);
    }

    void ContinuousMove()
    {
        visualizer.transform.Translate(Vector3.left * (Time.deltaTime * 0.3f));
        if (visualizer.transform.localPosition.x < 95.0f)
            visualizer.transform.localPosition = new Vector3(210.0f, visualizer.transform.localPosition.y, visualizer.transform.localPosition.z);
    }
}
