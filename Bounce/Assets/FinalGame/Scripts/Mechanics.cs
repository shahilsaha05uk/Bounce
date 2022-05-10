using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Mechanics : MonoBehaviour
{
    public virtual void Activate() { }

    protected abstract void Init();
    protected abstract IEnumerator MechanicUpdate();
}