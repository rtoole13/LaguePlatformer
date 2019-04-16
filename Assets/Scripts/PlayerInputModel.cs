using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputModel : MonoBehaviour, ICanFire
{
    public Vector2 movement { get; private set; }
    public bool sliding { get; private set; }
    public bool jump { get; private set; }
    
    #region EVENTS
    //delegates
    public delegate void JumpAction();

    //events
    public event JumpAction OnJump;
    public event FireAction OnFire;
    public event AltFireAction OnAltFire;
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

        if (Input.GetMouseButtonDown(0))
        {
            InvokeFire();
        }
        if (Input.GetMouseButtonDown(1))
        {
            InvokeAltFire();
        }
    }
    void InvokeFire()
    {
        Vector3 direction = Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position); //WARNING Vector3 stuff. If we do anything with zepth,  we may run into some weirdness here.
        OnFire(direction);
    }
    void InvokeAltFire()
    {
        Vector3 direction = Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position); //WARNING Vector3 stuff. If we do anything with zepth,  we may run into some weirdness here.
        OnAltFire(direction);
    }
}
