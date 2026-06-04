using UnityEngine;

public static class BossRuntimeSprites
{
    private static Sprite squareSprite;
    private static Sprite circleSprite;

    public static Sprite Square
    {
        get
        {
            if (squareSprite == null)
            {
                squareSprite = CreateSolidSprite("BossRuntimeSquare", false);
            }

            return squareSprite;
        }
    }

    public static Sprite Circle
    {
        get
        {
            if (circleSprite == null)
            {
                circleSprite = CreateSolidSprite("BossRuntimeCircle", true);
            }

            return circleSprite;
        }
    }

    private static Sprite CreateSolidSprite(string spriteName, bool circle)
    {
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = spriteName
        };

        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.48f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool filled = !circle || Vector2.Distance(new Vector2(x, y), center) <= radius;
                pixels[y * size + x] = filled ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
