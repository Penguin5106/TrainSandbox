using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            return;
        }

        Vector2 mousePos = playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Debug.Log("click at " + mousePos);
        
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit)
        {
            Debug.Log("hit object" + hit.collider.name);
            
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            
            clickable?.Click();
            
            GameEngine.GetInstance().SelectedObject = clickable;
        }
        
    }
}
