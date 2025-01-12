using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Controls controls;
    [SerializeField] private Image image;
    [SerializeField] private Color color;
    private InputAction action;
    void Awake()
    {
        controls = new Controls();
    }
    private void OnEnable()
    {
        action = controls.Game.Action;
        action.Enable();
        action.performed += Action;

    }
    private void OnDisable()
    {
        action.Disable();
    }
    private void Action(InputAction.CallbackContext context)
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("menu_change");
    }
    public void ChangeMenu()
    {
        image.color = color;
        SceneManager.LoadScene(1);
    }
}
