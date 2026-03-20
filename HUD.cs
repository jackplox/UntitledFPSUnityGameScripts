using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public float xHairWidth = 10f; //This is actually the distance from the center/position
    public float xHairHeight = 10f; //Same with this. Add new xHariWidth/Height for the drawn length
    private Canvas canvas;
    private Image lineTop, lineBottom, lineLeft, lineRight;
    private Image dot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GetComponent<Canvas>();

        lineTop = CreateLine(new Vector2(2, 10), new Vector2(0, xHairHeight));
        lineBottom = CreateLine(new Vector2(2, 10), new Vector2(0, -xHairHeight));
        lineLeft = CreateLine(new Vector2(10, 2), new Vector2(-xHairWidth, 0));
        lineRight = CreateLine(new Vector2(10, 2), new Vector2( xHairWidth, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Image CreateLine(Vector2 size, Vector2 position)
    {
        GameObject obj = new GameObject("CrosshairElement");
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;

        Image img = obj.AddComponent<Image>();
        img.color = Color.white;
        return img;
    }
}
