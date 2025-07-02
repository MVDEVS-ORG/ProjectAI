using Assets.Services;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class GunsView : MonoBehaviour, IInteractable
{
    [HideInInspector]public GunsModel GunsModel;
    [HideInInspector]public GunsController GunsController;
    [HideInInspector] public bool GunActive;
    public GunsSO GunsDataModel;
    [HideInInspector] public Transform PlayerTransform;
    [HideInInspector] public ObjectPoolManager PoolManager;
    public Transform GunBulletSpawnTransform;
    [HideInInspector] public Transform PlayerCursor;
    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Vector3 Scale;
    [HideInInspector] public Vector3 ReverseScale;
    [HideInInspector] public BoxCollider2D Collider;

    [HideInInspector] public bool _firing = false;
    [HideInInspector] public IGunUI GunUI;

    private void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Scale = transform.localScale;
        ReverseScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        Collider = GetComponent<BoxCollider2D>();
    }
    public GunsModel InitializeGun(GunsController controller, ObjectPoolManager objectPoolManager, Transform playerTrasform, Transform playerCursor)
    {
        Debug.Log("Gun initialized");
        GunsController = controller;
        if (GunsModel == null)
        {
            GunsModel = new GunsModel(GunsDataModel);
        }
        GunActive = true;
        PlayerTransform = playerTrasform;
        PoolManager = objectPoolManager;
        PlayerCursor = playerCursor;
        Collider.enabled = false;
        return GunsModel;
    }

    public void SetGunUI(IGunUI gunUI)
    {
        GunUI = gunUI;
    }

    public virtual void DeactivateGun(Vector3 position)
    {
        GunActive = false;
        Collider.enabled = true;
        transform.rotation = Quaternion.identity;
        transform.localScale = Scale;
        transform.position = position;
        PlayerTransform = null;
    }

    public virtual void Fire(bool firing)
    {
        _firing = firing;
        Debug.Log("Firing Trigger is Pressed");
    }



    public void Update()
    {
        if(GunActive)
        {
            /*transform.position = _playerTransform.position;*/
            OrbitalMotion();
        }
    }

    public void OrbitalMotion()
    {
        float angle = MathF.Atan2(PlayerCursor.position.y - PlayerTransform.position.y, PlayerCursor.position.x - PlayerTransform.position.x);
        if (angle > 0)
        {
            SpriteRenderer.sortingOrder = 4;
        }
        else
        {
            SpriteRenderer.sortingOrder = 10;
        }
        transform.position = PlayerTransform.position + new Vector3(GunsModel.ElipseHorizontalRadius * MathF.Sin(Mathf.PI * (0.5f) - angle), GunsModel.ElipseVerticalRadius * MathF.Cos(Mathf.PI * (0.5f) - angle), transform.position.z);
        transform.right = (PlayerCursor.position - PlayerTransform.position).normalized;
        if (MathF.Abs(angle) > Mathf.PI / 2)
        {
            transform.localScale = ReverseScale; 
        }
        else
        {
            transform.localScale = Scale;
        }
    }

    void IInteractable.Interact()
    {
        Debug.Log("GunPickUp Available");
    }
}
