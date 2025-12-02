using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    
    private Vector2 mousePosition;
    
    private void Update()
    {
        
    }

    public void onPoint(InputAction.CallbackContext context)
    {
        
    }
    
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            return;
        }
        
        
        Debug.Log("click at " + mousePosition);
        
    }
}
