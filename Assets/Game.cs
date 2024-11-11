using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public enum GameState
    {
        DIRECTION,
        POWER,
        WAIT
    }
    public GameState state;

    [Header("UI")]
    public GameObject line;
    public TMP_Text powerBar;

    [Header("Game")]
    public float rotation;
    public float rotationSpeed;
    public float power;
    public float powerSpeed;

    [Header("Controls")]
    public Controls controls;
    private InputAction action;
    private InputAction effects;
    public class Stroke
    {
        public float rotation;
        public float power;
        public Stroke(float rotation)
        {
            this.rotation = rotation;
        }
        public void SetRotation(float _rotation)
        {
            rotation = _rotation;
        }
        public void SetPower(float _power)
        {
            power = _power;
        }
        public override string ToString()
        {
            return "( " + rotation.ToString("F2") + ", " + power.ToString("F2") + " )";
        }
        public Vector2 ToVector2()
        {
            return new Vector2(rotation, power);
        }
    }
    public List<Stroke> strokes;

    private void Awake()
    {
        controls = new Controls();
    }
    void Start()
    {
        strokes = new();
        state = GameState.DIRECTION;
        rotation = 0;
        power = 0;
    }
    private void OnEnable()
    {
        action = controls.Game.Action;
        action.Enable();
        action.performed += Action;

        effects = controls.Game.Effects;
        effects.Enable();
        effects.performed += Effects;
    }
    private void OnDisable()
    {
        action.Disable();

        effects.Disable();
    }
    void Update()
    {
        switch (state)
        {
            case GameState.DIRECTION:

                rotation -= Time.deltaTime * rotationSpeed;
                if (rotation <= -360)
                {
                    rotation = 0;
                }
                RotateDirection();
                break;




            case GameState.POWER:
                power += Time.deltaTime * powerSpeed;
                if (power >= 50)
                {
                    power = 0;
                }
                int powerInt = Mathf.CeilToInt((power * 4) / 10);
                SetPowerBarText(powerInt);

                break;



            case GameState.WAIT:
                if (GolfBall.Instance.velocity.magnitude == 0)
                {
                    state = GameState.DIRECTION;
                    rotation = 0;
                    power = 0;
                }
                break;
            default:
                break;
        }
    }
    void RotateDirection()
    {
        line.gameObject.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
    void SetPowerBarText(int power)
    {
        string powerBarString = "";
        for (int i = 0; i < power; i++)
        {
            powerBarString += "|";
        }
        powerBar.text = powerBarString;
    }
    private void Action(InputAction.CallbackContext context)
    {
        switch (state)
        {
            case GameState.DIRECTION:
                Stroke stroke = new Stroke(rotation);
                strokes.Add(stroke);
                state = GameState.POWER;
                break;


            case GameState.POWER:
                if (strokes.Count > 0)
                {
                    strokes[^1].SetPower(power);
                }
                GolfBall.Instance.SetVelocity(strokes[^1].ToVector2());
                state = GameState.WAIT;
                break;


            case GameState.WAIT:
                
                break;
            default:
                break;
        }
    }
    private void Effects(InputAction.CallbackContext context)
    {

    }
}
