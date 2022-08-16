using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WFCTile
{
    enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    Sprite sprite;

    public List<int> possibleTiles;
    bool collapsed;

    public WFCTile()
    {
        possibleTiles = new List<int>();
        collapsed = false;
    }

    public void AddPossibility(int tileNum)
    {
        possibleTiles.Add(tileNum);
    }

    public void RemovePossibility(int tileNum)
    {
        possibleTiles.Remove(tileNum);
    }

    public int Possibilities()
    {
        return possibleTiles.Count;
    }

    public void CollapseTile(Vector2 position, Sprite sp, int selectedTile, Transform parent)
    {
        if (collapsed)
            throw new InvalidOperationException();

        Instantiate(position, sp, parent);
        possibleTiles = new List<int>();
        AddPossibility(selectedTile);
        collapsed = true;
    }

    public bool IsCollapsed()
    {
        return collapsed;
    }

    private void Instantiate(Vector2 position, Sprite sp, Transform parent)
    {
        GameObject go = new GameObject();
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>() as SpriteRenderer;
        sr.sprite = sp;
        Transform tf = go.GetComponent<Transform>();
        tf.position = new Vector3(position.x, position.y, 0);
        tf.SetParent(parent);
    }
}
