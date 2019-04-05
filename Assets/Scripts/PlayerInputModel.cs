using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputModel : MonoBehaviour
{
    public Vector2 movement { get; private set; }
    public bool sliding { get; private set; }
    public bool jump { get; private set; }
    
    #region EVENTS
    //delegates
    public delegate void JumpAction();

    //events
    public static event JumpAction OnJump;
    #endregion //EVENTS
    void Start()
    {
        movement = Vector2.zero;
        sliding = false;
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            sliding = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            sliding = false;
        }
        jump = Input.GetKeyDown(KeyCode.Space);
    }
}
