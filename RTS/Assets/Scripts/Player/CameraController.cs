using UnityEngine;
using Managers;

public class CameraController : MonoBehaviour, ISubscriber
{
    [SerializeField] float panSpeed;
    [SerializeField] private Vector2 panLimit;
    private Vector2 _cameraDirection;
    
    private static Camera _rtsCamera;
    // Start is called before the first frame update
    void Start()
    {
        SetCameraPanLimit();
        _rtsCamera = GetComponent<Camera>();
    }

    private void SetCameraPanLimit()
    {
        panLimit.x = MapManager.Instance.ReturnSizeOfMap().x;
        panLimit.y = MapManager.Instance.ReturnSizeOfMap().y;
    }

    // Update is called once per frame
    void Update()
    {
        if (_cameraDirection == Vector2.zero)
        {
            return;
        }
        MoveCamera(_cameraDirection);
    }

    public static bool GetMousePosition(out RaycastHit hit)
    {
        var ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return true;
        }

        return false;
    }

    private void MoveCamera(Vector2 direction)
    {
        Vector3 pos = _rtsCamera.transform.position; 
        
        
        pos.z += direction.y * panSpeed * Time.deltaTime;
        pos.x += direction.x * panSpeed * Time.deltaTime;
                

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        transform.position = pos;
    }

    private void OnCameraMove(Vector2 direction)
    {
        _cameraDirection = direction;
    }

    public void Subscribe(CharacterInput publisher)
    {
        publisher.cameraMovement += OnCameraMove;
    }

    public void UnSubscribe(CharacterInput publisher)
    {
        publisher.cameraMovement -= OnCameraMove;
    }
}
