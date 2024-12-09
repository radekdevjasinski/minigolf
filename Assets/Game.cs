using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    public TMP_Text holeText;
    public TMP_Text parText;
    public TMP_Text strokeText;
    public GameObject windPanel;
    public GameObject windLine;
    public TMP_Text windText;

    [Header("Game")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float powerSpeed;
    private float rotation;
    private float power;
    public Vector2 windForce;
    public float minWindVel;

    private int hole = 1;
    private int stroke = 1;

    [Header("Controls")]
    public Controls controls;
    private InputAction action;
    private InputAction effects;
    private InputAction next;
    private InputAction reset;
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
    public static Game Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        controls = new Controls();
    }
    public void Reset()
    {
        strokes = new();
        state = GameState.DIRECTION;
        rotation = 0;
        power = 0;
        stroke = 1;
        LevelController.Instance.activeLevel.FitCamera();
        GolfBall.Instance.transform.position = LevelController.Instance.activeLevel.ballStartingPos;
        GolfBall.Instance.ballCollider.enabled = true;
        Goal.Instance.transform.position = LevelController.Instance.activeLevel.goalPos;
        if(LevelController.Instance.activeLevel.hasWind)
        {
            windForce = GenerateRandomWind();
        }
        else
        {
            windForce = Vector2.zero;

        }
        UpdateUI();
    }
    private void OnEnable()
    {
        action = controls.Game.Action;
        action.Enable();
        action.performed += Action;

        effects = controls.Game.Effects;
        effects.Enable();
        effects.performed += Effects;

        next = controls.Game.Next;
        next.Enable();
        next.performed += NextKey;

        reset = controls.Game.Reset;
        reset.Enable();
        reset.performed += ResetKey;
    }
    private void OnDisable()
    {
        action.Disable();

        effects.Disable();

        next.Disable();

        reset.Disable();
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
                if (GolfBall.Instance.velocity.magnitude == 0 && GolfBall.Instance.ballCollider.enabled)
                {
                    stroke++;
                    state = GameState.DIRECTION;
                    //rotation = 0;
                    //power = 0;
                    if(stroke > LevelController.Instance.activeLevel.par)
                    {
                        UIAnimator.Instance.PlayLose();
                        //MoveToNextLevel(false);
                    }
                    UpdateUI();
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
    private void NextKey(InputAction.CallbackContext context)
    {
        //MoveToNextLevel(true);
        UIAnimator.Instance.PlayLevelChange();
    }
    private void ResetKey(InputAction.CallbackContext context)
    {
        LevelController.Instance.FirstLevel();
        hole = 1;
        Reset();
    }
    void UpdateUI()
    {
            holeText.text = "hole " + hole;
            parText.text = "par " + LevelController.Instance.activeLevel.par;
            strokeText.text = "stroke " + stroke;

            if(LevelController.Instance.activeLevel.hasWind)
            {
                windPanel.SetActive(true);
                windText.text = (windForce.magnitude * 100).ToString("F0") + " mph";
                float angle = Mathf.Atan2(windForce.y, windForce.x) * Mathf.Rad2Deg;
                windLine.transform.rotation = Quaternion.Euler(0, 0, angle);

            }
            else
            {
               windPanel.SetActive(false);
            }
    }
    public void MoveToNextLevel(bool win)
    {
        if(win)
        {
            LevelController.Instance.NextLevel();
            hole++;
        }
        else
        {
            LevelController.Instance.LoseLevel();
            //hole = 1;
        }
        Reset();
        
    }
    private Vector2 GenerateRandomWind()
    {
        Vector2[] directions = new Vector2[]
        {
            new Vector2(0, 1),   // N
            new Vector2(1, 1),   // NE
            new Vector2(1, 0),   // E
            new Vector2(1, -1),  // SE
            new Vector2(0, -1),  // S
            new Vector2(-1, -1), // SW
            new Vector2(-1, 0),  // W
            new Vector2(-1, 1)   // NW
        };

        int randomIndex = Random.Range(0, directions.Length);
        Vector2 randomDirection = directions[randomIndex];

        float randomStrength = Random.Range(0.01f, 0.1f);

        return randomDirection.normalized * randomStrength;
    }

}
