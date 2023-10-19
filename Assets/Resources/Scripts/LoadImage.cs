using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadImage : MonoBehaviour
{
    // Start is called before the first frame update
    public void loadImage()
    {
        Sprite sprite = Resources.Load<Sprite>("Sprites/image");
        if (sprite == null) { Debug.Log("couldn't find sprite"); }
        else if (GameObject.Find("image")!= null)
        {
            Debug.Log("image already exists");
        }
        else
        {
            GameObject Image = new GameObject("image");
            Image.AddComponent<SpriteRenderer>();
            Image.GetComponent<SpriteRenderer>().sprite = sprite;
            Image.transform.localScale = new Vector3(200f, 200f, 1f);
            Color currentColor = Image.GetComponent<SpriteRenderer>().color;
            currentColor.a = 1/3f;
            Image.GetComponent<SpriteRenderer>().color = currentColor;
        }
        
    }
}
