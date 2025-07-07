using UnityEngine;

public class MeleeWeaponView : MonoBehaviour
{
    [SerializeField] private MeleeWeaponSO _meleeData;

    public MeleeWeaponModel SetupAndActivate(Transform playerTransform, Transform cursorTransform)
    {
        MeleeWeaponModel model = new MeleeWeaponModel(_meleeData);
        return model;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
