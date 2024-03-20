using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnhighlightFieldRequest : Request
{
    public Field field;
    public UnhighlightFieldRequest(Field f)
    {
        field = f;
    }
}
