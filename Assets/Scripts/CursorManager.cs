using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D normalCursor;
    public Texture2D moveCursor;
    public Texture2D enemyCursor;
    public Texture2D doorCursor;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Move"))
            {
                Cursor.SetCursor(moveCursor, Vector2.zero, CursorMode.Auto);
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                Cursor.SetCursor(enemyCursor, Vector2.zero, CursorMode.Auto);
            }
            else if (hit.collider.CompareTag("FrontDoorway") || hit.collider.CompareTag("BackDoorway"))
            {
                Cursor.SetCursor(doorCursor, Vector2.zero, CursorMode.Auto);
            }
        }
        else
        {
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}

