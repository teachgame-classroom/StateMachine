using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState_AxeAttack01 : CharacterState_MeleeAttackBase
{


    public CharacterState_AxeAttack01(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        Init(typeof(CharacterState_AxeAttack02), 0.6f, 0.95f, InputList.FIRE1);
    }
}
