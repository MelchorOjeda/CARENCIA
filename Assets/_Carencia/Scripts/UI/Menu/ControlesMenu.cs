using UnityEngine;
using UnityEngine.InputSystem;

// Teclas de los menús. Cada acción es un botón: se lee una vez por pulsación con WasPressedThisFrame.
// Al abrir una pantalla se llama a Bloquear, así la misma pulsación que la abrió no la usa también.
public static class ControlesMenu
{
    public static readonly InputAction Arriba = Boton("<Keyboard>/upArrow", "<Keyboard>/w", "<Gamepad>/dpad/up");
    public static readonly InputAction Abajo = Boton("<Keyboard>/downArrow", "<Keyboard>/s", "<Gamepad>/dpad/down");
    public static readonly InputAction Izquierda = Boton("<Keyboard>/leftArrow", "<Keyboard>/a", "<Gamepad>/dpad/left");
    public static readonly InputAction Derecha = Boton("<Keyboard>/rightArrow", "<Keyboard>/d", "<Gamepad>/dpad/right");
    public static readonly InputAction Confirmar = Boton("<Keyboard>/enter", "<Keyboard>/numpadEnter", "<Gamepad>/buttonSouth");
    public static readonly InputAction Volver = Boton("<Keyboard>/escape", "<Gamepad>/buttonEast");
    public static readonly InputAction PestanaAnterior = Boton("<Keyboard>/q", "<Gamepad>/leftShoulder");
    public static readonly InputAction PestanaSiguiente = Boton("<Keyboard>/e", "<Gamepad>/rightShoulder");
    public static readonly InputAction Guardar = Boton("<Keyboard>/g", "<Gamepad>/buttonWest");
    public static readonly InputAction Restablecer = Boton("<Keyboard>/r", "<Gamepad>/buttonNorth");
    public static readonly InputAction CualquierTecla = Boton("<Keyboard>/anyKey", "<Mouse>/leftButton", "<Gamepad>/start", "<Gamepad>/buttonSouth");

    private static int bloqueadoHasta = -1;

    // Falso durante el fotograma en que se abrió o cerró una pantalla
    public static bool Listo => Time.frameCount > bloqueadoHasta;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        bloqueadoHasta = -1;
    }

    public static void Bloquear()
    {
        bloqueadoHasta = Time.frameCount;
    }

    // Arriba/abajo como -1, 0 o +1 (arriba es -1 porque la lista se lee de arriba hacia abajo)
    public static int Vertical() => Pulsada(Abajo) - Pulsada(Arriba);

    public static int Horizontal() => Pulsada(Derecha) - Pulsada(Izquierda);

    public static int Pestana() => Pulsada(PestanaSiguiente) - Pulsada(PestanaAnterior);

    // El proyecto no recarga el dominio al entrar a Play: las acciones sobreviven entre partidas, pero el
    // Input System las apaga al reiniciarse. Se encienden al leerlas, sin depender del orden de arranque.
    public static bool Pulso(InputAction accion)
    {
        if (!accion.enabled)
            accion.Enable();

        return Listo && accion.WasPressedThisFrame();
    }

    private static int Pulsada(InputAction accion) => Pulso(accion) ? 1 : 0;

    private static InputAction Boton(params string[] rutas)
    {
        var accion = new InputAction(type: InputActionType.Button);
        foreach (var ruta in rutas)
            accion.AddBinding(ruta);

        return accion;
    }
}
