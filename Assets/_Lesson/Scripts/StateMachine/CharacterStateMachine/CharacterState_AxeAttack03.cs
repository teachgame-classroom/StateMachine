using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState_AxeAttack03 : CharacterState_AttackBase
{
    public CharacterState_AxeAttack03(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        Init(null, 0.6f, 0.95f);
    }
}
