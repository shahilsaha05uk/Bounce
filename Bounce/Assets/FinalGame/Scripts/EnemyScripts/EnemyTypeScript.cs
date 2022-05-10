using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    ANT,
    FOX
}
public class EnemyTypeScript : MonoBehaviour
{
    public EnemyType type;

    public EnemyType returnType()
    {
        return type;
    }
}
