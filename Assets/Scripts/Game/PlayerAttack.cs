using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform _trailTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletOrigin;

    private Camera _camera;
    private PlayerControls _playerControls;
    private PhotonView _photonView;

    private bool _isAiming;

    private TargetMark _target;

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();

        _playerControls = new PlayerControls();

        if (_photonView.IsMine)
        {
            _playerControls.KeyboardControls.Aim.started += OnAimChanged;
            _playerControls.KeyboardControls.Aim.canceled += OnAimChanged;
            _playerControls.KeyboardControls.Fire.started += OnShoot;
        }
    }

    private void Start()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        Aim();
    }

    private void OnAimChanged(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValueAsButton();
        _trailTransform.gameObject.SetActive(_isAiming);

        if (_target != null && _isAiming == false)
        {
            _target.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        Shoot();
    }

    private void Aim()
    {
        if (_isAiming)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 mousePosition = hitInfo.point;
                Vector3 direction = mousePosition - transform.position;
                direction = new Vector3(direction.x, 0, direction.z);
                _trailTransform.forward = direction;
            }

            Debug.DrawRay(_trailTransform.position + _trailTransform.forward, _trailTransform.forward * 8, Color.red);

            if (Physics.Raycast(_trailTransform.position, _trailTransform.forward, out RaycastHit hit, 8.0f))
            {
                if (hit.collider.TryGetComponent(out TargetMark targetMark))
                {
                    if (_target != targetMark && _target != null)
                        _target.GetComponent<MeshRenderer>().enabled = false;

                    _target = targetMark;
                    _target.GetComponent<MeshRenderer>().enabled = true;
                }
                else if (_target != null)
                {
                    _target.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            else if (_target != null)
            {
                _target.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void Shoot()
    {
        if (_isAiming)
        {
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", _bulletOrigin.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetDirection(_trailTransform.forward, gameObject);
        }
    }
}
