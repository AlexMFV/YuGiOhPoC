using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D c_default;
    [SerializeField] Texture2D c_clicked;
    [SerializeField] GameObject c_clickAnimation;

    bool isClicked = false;
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        //NOTE: Offset is different when the surrender cursor is selected
        offset = new Vector3(6, 11, 0);
        
        //Set default cursor
        Cursor.SetCursor(c_default, offset, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
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
}
