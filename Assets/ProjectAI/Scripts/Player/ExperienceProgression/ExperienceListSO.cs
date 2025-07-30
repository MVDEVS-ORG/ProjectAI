using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperienceListSO", menuName = "Scriptable Objects/ExperienceListSO")]
public class ExperienceListSO : ScriptableObject
{
    public List<int> ExperiencePerLevel;
}
