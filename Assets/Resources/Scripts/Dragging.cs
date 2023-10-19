using UnityEngine;

public class Dragging : MonoBehaviour
{
    private bool ifDragging = false;
    private Vector3 offset;

    private void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ifDragging = true;
    }

    private void OnMouseUp()
    {
        ifDragging = false;
    }

    private void Update()
    {
        if (ifDragging)
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(cursorPosition.x + offset.x, cursorPosition.y + offset.y, transform.position.z);
        }
    }
}
