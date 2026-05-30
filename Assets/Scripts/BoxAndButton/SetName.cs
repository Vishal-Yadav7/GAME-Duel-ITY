using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetName : MonoBehaviour
{
    public string objectName;
    public SpriteRenderer sp;
    public Color boxColor;
    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        sp.color = boxColor;
    }
}
