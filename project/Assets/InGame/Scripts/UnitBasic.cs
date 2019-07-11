using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UnitBasic
{
    void SetTeamCode(int code);
    int GetTeamCode();
    void TakeDamage(float dmg, GameObject from);
}
