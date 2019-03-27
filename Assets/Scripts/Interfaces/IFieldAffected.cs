using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFieldAffected
{
    void OnFieldEnter<T> (T field);
    void OnFieldExit<T> (T field);
}