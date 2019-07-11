using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState_AxeAttack00 : CharacterState_AttackBase
{
    public CharacterState_AxeAttack00(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        Init(typeof(CharacterState_AxeAttack01), 0.6f, 0.95f);
    }
}
