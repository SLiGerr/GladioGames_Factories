using UnityEngine;

namespace Source.Code
{
    public class MapControl : MonoBehaviour {
        public float maxZoom;
        public float minZoom;
        public float panSpeed = -1;
   
        Vector3 _bottomLeft;
        Vector3 _topRight;
   
        float _cameraMaxY;
        float _cameraMinY;
        float _cameraMaxX;
        float _cameraMinX;

        private Camera _camera;
        private Transform _transform;
   
        void Start()
        {
            _camera = GetComponent<Camera>();
            _transform = transform;

            var position = _transform.position;
            _topRight = _camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight, -position.z));
            _bottomLeft = _camera.ScreenToWorldPoint(new Vector3(0,0,-position.z));
            
            _cameraMaxX = _topRight.x;
            _cameraMaxY = _topRight.y;
            _cameraMinX = _bottomLeft.x;
            _cameraMinY = _bottomLeft.y;
        }
   
        void Update ()
        {
            if(Input.GetMouseButton(0))
            {
                var x = Input.GetAxis("Mouse X") * panSpeed;
                var y = Input.GetAxis("Mouse Y") * panSpeed;
                transform.Translate(x, y, 0);
            }
       
            if((Input.GetAxis("Mouse ScrollWheel") > 0) && _camera.orthographicSize > minZoom || _camera.orthographicSize > maxZoom)
            {
                _camera.orthographicSize -= 0.5f;
            }
           
            if ((Input.GetAxis("Mouse ScrollWheel") < 0) && _camera.orthographicSize < maxZoom)           
            {
                _camera.orthographicSize += 0.5f;
            }

            //check if camera is out-of-bounds, if so, move back in-bounds
            _topRight = _camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight, -_transform.position.z));
            _bottomLeft = _camera.ScreenToWorldPoint(new Vector3(0,0,-_transform.position.z));
       
            if(_topRight.x > _cameraMaxX)
            {
                _transform.position = new Vector3(transform.position.x - (_topRight.x - _cameraMaxX), _transform.position.y, _transform.position.z);
            }
       
            if(_topRight.y > _cameraMaxY)
            {
                _transform.position = new Vector3(transform.position.x, _transform.position.y - (_topRight.y - _cameraMaxY), _transform.position.z);
            }
       
            if(_bottomLeft.x < _cameraMinX)
            {
                _transform.position = new Vector3(transform.position.x + (_cameraMinX - _bottomLeft.x), _transform.position.y, _transform.position.z);
            }
       
            if(_bottomLeft.y < _cameraMinY)
            {
                _transform.position = new Vector3(transform.position.x, _transform.position.y + (_cameraMinY - _bottomLeft.y), _transform.position.z);
            }
        }
    }
}
 