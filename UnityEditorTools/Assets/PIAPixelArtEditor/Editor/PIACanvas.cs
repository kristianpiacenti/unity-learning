﻿using UnityEngine;
using System.Collections.Generic;
public class PIACanvas {
    private List<PIACanvasLayer> layers;
    private int currentLayer = 0;


    public Rect grid{ get; set; }

    public PIACanvas() {
        layers = new List<PIACanvasLayer>();
        PIACanvasLayer.index = 0;
        layers.Add(new PIACanvasLayer());
    }

    public PIACanvasLayer GetCurrentLayer() {
        return layers[currentLayer];
    }

    public Texture2D GetFinalImage() {
       

        //Texture2D tex = new Texture2D(GetCurrentLayer().Texture.width, GetCurrentLayer().Texture.height);
        //foreach (var item in layers)
        //{
        //    for (int x = 0; x < tex.width; x++)
        //    {
        //        for (int y = 0; y < tex.height; y++)
        //        {
        //            Color currentPixelColor = item.Texture.GetPixel(x, y);
        //            if (!Color.Equals(currentPixelColor, Color.clear))
        //                tex.SetPixel(x, y, currentPixelColor);
        //        }
        //    }
        //}
        
        //return tex;
        return GetCurrentLayer().Texture;
    }
    public static Vector2 WorldPositionToGridPixel(Vector2 worldPosition, Rect grid, Texture tex)
    {
        int cellWidth = (int)(grid.width / tex.width);
        int cellHeight = (int)(grid.height / tex.height);
       
        float relX = (worldPosition.x - grid.x) / cellWidth;
        float relY = (worldPosition.y - grid.y) / cellHeight;
        Vector2 gridPosition = new Vector2((int)relX, (int)relY);

        return gridPosition;
    }
    public static Vector2Int WorldPositionToGridPixel(Vector2 worldPosition, Rect grid, Vector2 gridParent, Texture tex)
    {
        float cellWidth = (grid.width / tex.width);
        float cellHeight = (grid.height / tex.height);
        Vector2 localPosition = ParentToLocalPosition(worldPosition, gridParent);
        float relX = (localPosition.x - grid.x) / cellWidth;
        float relY = (localPosition.y - grid.y) / cellHeight;
        Vector2Int gridPixelPosition = new Vector2Int((int)relX, (int)relY);
        return gridPixelPosition;
    }
    public static Vector2Int GridPixelToWorldPosition(Vector2Int gridPixel, Rect grid, Texture tex)
    {
        float cellWidth = (grid.width / tex.width);
        float cellHeight = (grid.height / tex.height);
        float relX = (gridPixel.x * cellWidth) + grid.x;
        float relY = (gridPixel.y * cellWidth) + grid.y;
        Vector2Int worldPosition = new Vector2Int((int)relX, (int)relY);
        return worldPosition;
    }
    public static Vector2Int GridPixelToWorldPosition(Vector2Int gridPixel, Rect grid, Vector2 gridParent, Texture tex) {
        float cellWidth = (grid.width / tex.width);
        float cellHeight = (grid.height / tex.height);
        float relX = (gridPixel.x * cellWidth) + grid.x;
        float relY = (gridPixel.y * cellWidth) + grid.y;
        Vector2Int worldPosition = new Vector2Int((int)relX, (int)relY);
        Vector2 parentPosition = LocalToParentPosition(worldPosition, gridParent);

        return new Vector2Int((int)parentPosition.x,(int)parentPosition.y);
    }
    public static Vector2 ParentToLocalPosition(Vector2 parentPosition, Vector2 childPosition) {
        return parentPosition - childPosition;
    }
    public static Vector2 LocalToParentPosition(Vector2 parentPosition, Vector2 childPosition)
    {
        return parentPosition + childPosition;
    }
}
