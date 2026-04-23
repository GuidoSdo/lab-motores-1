using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ScreenController : MonoBehaviour
{
    [Header("Control De Brazo")]
    public Transform screenArm;

    public float armSpeed = 20f;
    private bool readyToScreen = true;
    private bool onScreen = true;
    public float screenCooldown = 0.25f;
    private float rotX;
    public Key buttonCamera;

    [Header("Luz De Pantalla")]
    public Light backLight;

    //====De Prueba borrar despues=======
    [Header("Prueba, borrar luego")]
    public Key cameraState;
    public Key offState;
    public Key staticState;
    public Key rechargeBattery;
    //===================================

    [Header("Renderer y Texturas")]
    public Renderer screenRenderer;

    public Texture cameraTexture;
    public Texture offTexture;
    public VideoPlayer staticVideo;

    

    public enum ScreenState
    {
        On,
        Off,
        Static
    }
    private ScreenState state = ScreenState.On;

    //Batería
    [Header("Batería")]
    private BatteryController battery;
    [SerializeField] private Image batteryFill;
    [SerializeField] private float drainMultiplier = 1f;


    //Que pueda pulsar un botón que llame a una funcion del enemigo (Afectar)


    private void Awake()
    {
        battery = GetComponentInParent<BatteryController>();
    }

    private void Start()
    {
        //Valores Iniciales
        rotX = screenArm.rotation.eulerAngles.x;
        screenArm.Rotate(35, 0, 0);
    }
    void Update()
    {
        ScreenInput();
        ArmControl();
        BatteryUsage();
        UpdateUI();

    }


    private void ScreenInput()
    {
        if (Keyboard.current[buttonCamera].wasPressedThisFrame && readyToScreen)
        {
            readyToScreen = false;
            onScreen = !onScreen;

            if (onScreen && battery.HasBattery)
                ChangeState(ScreenState.On);
            else
                ChangeState(ScreenState.Off);

            Invoke(nameof(ResetScreen), screenCooldown);
        }

    }

    //Arreglar, poner bandera para que se pueda poner otro estado (Estática) desde otro lado
    private void ArmControl()
    {
        //Si onScreen es verdadero target=35f, si es falso target=0f
        float target = onScreen ? 0f : 35f;
        //"Animacion" de la camara levantandose y escondiendose
        rotX = Mathf.MoveTowards(rotX, target, armSpeed * Time.deltaTime); //Se mueve hacia  rotando
        screenArm.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    }

    //Si tiene batería, puede encender y apagar la camara
    //Si no tiene bateria, queda apagada
    //Si recarga batería puede volver a encender y apagar
    public void BatteryUsage()
    {

        if (onScreen && state == ScreenState.On && battery.HasBattery)
        {
            battery.Drain(drainMultiplier);
        }

        if (!battery.HasBattery)
        {
            ChangeState(ScreenState.Off);
        }

        if (battery.HasBattery && state == ScreenState.Off)
        {
            ChangeState(ScreenState.On);
        }

    }
    private void UpdateUI()
    {
        float percent = battery.BatteryPercent;

        batteryFill.fillAmount = percent;
        batteryFill.color = Color.Lerp(Color.red, Color.green, percent);
    }

    //Funcion Publica para que sea modificable externamente
    //Estados= "on", "off" y "static"
    public void ChangeState(ScreenState newState)
    {
        if (state == newState) return;

        state = newState;

        switch (state)
        {
            case ScreenState.On:
                setOn();
                break;
            case ScreenState.Off:
                setOff();
                break;
            case ScreenState.Static:
                setStatic();
                break;
        }
    }




    private void ResetScreen()
    {
        readyToScreen = true;
    }



    //Seteos de los Estados (Encendido, Apagado o Estatica) (Para reutilizar codigo)
    private void setOn()
    {
        backLight.enabled = true;
        staticVideo.Stop();
        screenRenderer.material.SetTexture("_BaseMap", cameraTexture);
    }
    private void setOff()
    {
        backLight.enabled = false;
        staticVideo.Stop();
        screenRenderer.material.SetTexture("_BaseMap", offTexture);
    }
    private void setStatic()
    {
        backLight.enabled = true;
        screenRenderer.material.SetTexture("_BaseMap", offTexture);
        staticVideo.Play();
    }

}


