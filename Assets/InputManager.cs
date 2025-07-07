public class PlayerInputManager
{
    public InputSystem_Actions controls { get; private set; }

    public PlayerInputManager()
    {
        controls = new InputSystem_Actions();
        controls.Enable();
    }
    public void OnEnable()
    {
        controls.Enable();
    }

    public void OnDisable()
    {
        controls.Disable();
    }

    public void OnDestroy()
    {
        controls.Dispose();
    }
}