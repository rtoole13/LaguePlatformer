﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void FireAction(Vector2 dir);

public interface ICanFire
{
    event FireAction OnFire;
}