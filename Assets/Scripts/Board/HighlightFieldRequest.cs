using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightFieldRequest : Request
{
    public Field field;
    public HighlightFieldRequest(Field f)
    {
        field = f;
    }
}
