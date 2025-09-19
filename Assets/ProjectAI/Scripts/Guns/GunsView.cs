using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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
    [HideInInspector] public Vector3 LateralInversion;
    [HideInInspector] public BoxCollider2D Collider;

    [HideInInspector] public bool _firing = false;
    [HideInInspector] public IGunUI GunUI;

    [HideInInspector] public Dictionary<ElementEnum, int> ElementalBuffs = new();

    [HideInInspector] public bool AlternateRotation = false;
    [HideInInspector] public bool WeaponKnockback = false;

    public Action<IGunProjectileBehavior> OnShotFired;
    public Sprite GunSprite;
    private float _angle;
    private void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Scale = transform.localScale;
        ReverseScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        Collider = GetComponent<BoxCollider2D>();
    }

    protected void ShotFired(IGunProjectileBehavior projectileBehavior)
    {
        OnShotFired?.Invoke(projectileBehavior);
    }

    public GunsModel InitializeGun(GunsController controller, ObjectPoolManager objectPoolManager, Transform playerTrasform, Transform playerCursor)
    {
        Debug.Log("Gun initialized");
        GunsController = controller;
        if (!GunsModel.Empty)
        {
            GunsModel = new GunsModel(GunsDataModel);
            GunsModel.Empty = true;
        }
        GunActive = true;
        PlayerTransform = playerTrasform;
        PoolManager = objectPoolManager;
        PlayerCursor = playerCursor;
        Collider.enabled = false;
        gameObject.name = gameObject.name + (GunsModel.GetHashCode()%1000000);
        ActivateGun();
        return GunsModel;
    }

    public virtual void ActivateGun()
    {
        Debug.Log("Not written for gun " + gameObject.name);
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
            if (!AlternateRotation)
            {
                OrbitalMotion();
            }
            else
            {
                RotationalMotion();
            }
        }
    }

    public void OrbitalMotion()
    {
        _angle = MathF.Atan2(PlayerCursor.position.y - PlayerTransform.position.y, PlayerCursor.position.x - PlayerTransform.position.x);
        if (_angle > 0)
        {
            SpriteRenderer.sortingOrder = 4;
        }
        else
        {
            SpriteRenderer.sortingOrder = 10;
        }
        transform.position = PlayerTransform.position + new Vector3(GunsModel.ElipseHorizontalRadius * MathF.Cos(_angle), GunsModel.ElipseVerticalRadius * MathF.Sin(_angle), transform.position.z);
        transform.right = (PlayerCursor.transform.position - PlayerTransform.position).normalized;
        RotateGun();
    }

    public void RandomRotateGun()
    {
        _angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
        transform.position = PlayerTransform.position + new Vector3(GunsModel.ElipseHorizontalRadius * MathF.Cos(_angle), GunsModel.ElipseHorizontalRadius * MathF.Sin(_angle), transform.position.z);
        transform.right = (transform.position - PlayerTransform.position).normalized;
        RotateGun();
    }

    public void SetStartingRotation(int order,int max)
    {
        _angle = 0 + (Mathf.PI * 2 / max) * order;
        transform.position = PlayerTransform.position + new Vector3(GunsModel.ElipseHorizontalRadius * MathF.Cos(_angle), GunsModel.ElipseHorizontalRadius * MathF.Sin(_angle), transform.position.z);
        transform.right = (transform.position - PlayerTransform.position).normalized;
        RotateGun();   
    }

    public void RotationalMotion()
    {
        //_angle += MathF.PI * GunsModel.FireRate * Time.deltaTime / 20;
        _angle += MathF.PI * 3 * Time.deltaTime / 20;
        transform.position = PlayerTransform.position + new Vector3(GunsModel.ElipseHorizontalRadius * MathF.Cos(_angle), GunsModel.ElipseHorizontalRadius * MathF.Sin(_angle), transform.position.z);
        transform.right = (transform.position - PlayerTransform.position).normalized;
        RotateGun();
    }

    void IInteractable.Interact(Transform transform)
    {
        Debug.Log("GunPickUp Available");
    }

    private void RotateGun()
    {
        float rotationalAngle = Mathf.Atan2(transform.position.y - PlayerTransform.position.y, transform.position.x - PlayerTransform.position.x);
        if (transform.rotation.y == 1) //This if statement is to prevent a bug where the transform.rotation.y becomes 180 because of gimble lock
        {
            transform.localScale = Scale;
        }
        else if (MathF.Abs(rotationalAngle) > Mathf.PI / 2)
        {
            transform.localScale = ReverseScale;
        }
        else
        {
            transform.localScale = Scale;
        }
    }
}
