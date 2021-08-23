using UnityEngine;
using Managers;

public class CameraController : MonoBehaviour, ISubscriber
{
    [SerializeField] float panSpeed;
    private Vector2 panLimit;
    [SerializeField] private float scrollSpeed;
    private Vector2 _cameraDirection;
    private Vector3 _cameraPos;

    [HideInInspector] public float maxYScroll;
    public float minYScroll;
    [SerializeField] private LayerMask fogOfWarLayer;
    
    public static Camera rtsCamera;
    // Start is called before the first frame update
    void Start()
    {
        SetCameraPanLimit();
        rtsCamera = GetComponent<Camera>();
        maxYScroll = transform.position.y + 10;
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

    public bool GetMousePosition(out RaycastHit hit)
    {
        //Sends a ray to wherever the mouse position is.
        var ray = rtsCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray,out hit,Mathf.Infinity,fogOfWarLayer);
    }

    private void MoveCamera(Vector2 direction)
    {
        _cameraPos = rtsCamera.transform.position; 
        
        
        _cameraPos.z += direction.y * panSpeed * Time.deltaTime;
        _cameraPos.x += direction.x * panSpeed * Time.deltaTime;
                

        _cameraPos.x = Mathf.Clamp(_cameraPos.x, -panLimit.x, panLimit.x);
        _cameraPos.z = Mathf.Clamp(_cameraPos.z, -panLimit.y, panLimit.y);
        transform.position = _cameraPos;
    }

    private void OnCameraMove(Vector2 direction)
    {
        _cameraDirection = direction;
    }

    private void ScrollCamera(float scrollAmount)
    {
        _cameraPos = rtsCamera.transform.position;
        _cameraPos.y -= scrollAmount * scrollSpeed * 100f * Time.deltaTime;
        _cameraPos.y = Mathf.Clamp(_cameraPos.y, minYScroll, maxYScroll);
        transform.position = _cameraPos;
    }

    public void Subscribe(CharacterInput publisher)
    {
        publisher.cameraMovement += OnCameraMove;
        publisher.mouseScrollWheel += ScrollCamera;
    }

    public void UnSubscribe(CharacterInput publisher)
    {
        publisher.cameraMovement -= OnCameraMove;
        publisher.mouseScrollWheel -= ScrollCamera;
    }
}
