using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFieldAffected
{
    void OnFieldEnter(Field field);
    void OnFieldExit(Field field);
}

public interface IGravityFieldAffected
{
    void OnFieldEnter(GravityField field);
    void OnFieldExit(GravityField field);
}


