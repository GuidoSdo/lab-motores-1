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

    private string state = "On";

    //Batería
    [Header("Batería")]
    public Image batteryFill;

    public float maxBattery = 100f;
    public float currentBattery;

    public float drainSpeed = 5f;

    private bool hasBattery = true;

    //Que pueda pulsar un botón que llame a una funcion del enemigo (Afectar)

    private void Start()
    {
        //Valores Iniciales
        rotX = screenArm.rotation.eulerAngles.x;
        screenArm.Rotate(35, 0, 0);
        currentBattery = maxBattery;
    }
    void Update()
    {
        SwapStates(); //Temporal
        setState(state); //Depende que state haya, cambia la pantalla
        ScreenInput();
        ArmControl();
        BatteryControl();

    }

    public void BatteryRecharge(float charge)
    {
        if (!hasBattery && state == "Off" && onScreen)
        {
            hasBattery = true;
            state = "On";
        }
        if (currentBattery + charge > maxBattery)
        {
            currentBattery = maxBattery;
        }
        else
        {
            currentBattery = currentBattery + charge;
        }
    }

    //Si tiene batería, puede encender y apagar la camara
    //Si no tiene bateria, queda apagada
    //Si recarga batería puede volver a encender y apagar
    public void BatteryControl()
    {


        if (onScreen && hasBattery && state == "On")
        {
            currentBattery -= drainSpeed * Time.deltaTime;
        }

        else if (!hasBattery)
        {
            state = "Off";
        }

        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);
        batteryFill.color = Color.Lerp(Color.red, Color.green, currentBattery / maxBattery);
        batteryFill.fillAmount = currentBattery / maxBattery;

        if (currentBattery <= 0f)
        {
            hasBattery = false;
        }
        else
        {
            hasBattery = true;
        }

    }

    //Funcion Publica para que sea modificable externamente
    //Estados= "on", "off" y "static"
    public void ChangeState(string stateInput)
    {
        switch (stateInput)
        {
            case "on" or "On":
                setOn();
                break;
            case "off" or "Off":
                setOff();
                break;
            case "static" or "Static":
                setStatic();
                break;

            default:
                setOn();
                break;
        }
    }


    //Si se llama desde el player Input
    private void ScreenActivation()
    {
        if (readyToScreen)
        {
            readyToScreen = false; //Bandera para el cooldown
            onScreen = !onScreen;
            if (onScreen)
            {
                setOff();
            }
            else if (!onScreen)
            {
                setOn();
            }
            Invoke(nameof(ResetScreen), screenCooldown); //Cooldown para no spamear el botón de cámara

        }
    }


    private void ScreenInput()
    {
        if (Keyboard.current[buttonCamera].wasPressedThisFrame && readyToScreen)
        {
            readyToScreen = false; //Bandera para el cooldown
            onScreen = !onScreen;
            if (onScreen)
            {
                state = "On";
            }
            else
            {
                state = "Off";
            }
            Invoke(nameof(ResetScreen), screenCooldown); //Cooldown para no spamear el botón de cámara

        }


        if (Keyboard.current[rechargeBattery].wasPressedThisFrame)
        {
            BatteryRecharge(25f);
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

    private void ResetScreen()
    {
        readyToScreen = true;
    }

    //Cambia de Estado entre Encendido, Apagado y Estática Crear funciones publicas para que sean llamadas desde afuera
    private void SwapStates()
    {
        if (Keyboard.current[staticState].wasPressedThisFrame)
        {
            state = "Static";
        }
        if (Keyboard.current[offState].wasPressedThisFrame)
        {
            state = "Off";
        }
        if (Keyboard.current[cameraState].wasPressedThisFrame)
        {
            state = "On";
        }
    }

    private void setState(string stateInput)
    {
        switch (stateInput)
        {
            case "on" or "On":
                setOn();
                break;
            case "off" or "Off":
                setOff();
                break;
            case "static" or "Static":
                setStatic();
                break;

            default:
                setOn();
                break;
        }
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


