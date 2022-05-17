using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnAlter : MonoBehaviour
{
    Color emptyAlterSpriteColor = Color.black;

    SpriteRenderer _itemSprite;
    private Color _curColor;

    [SerializeField] private Checkpoint _checkpoint = null;

    void Start()
    {
        emptyAlterSpriteColor.a = 0.4f;
        _itemSprite = gameObject.FindComponentInChildWithTag<SpriteRenderer>("AlterItemSprite");
        _curColor = _itemSprite.color;
        _itemSprite.color = emptyAlterSpriteColor;
    }

    public void Place()
    {
        _itemSprite.color = _curColor;
    }
}