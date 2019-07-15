using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState_AxeAttack02 : CharacterState_MeleeAttackBase
{


    public CharacterState_AxeAttack02(CharacterStateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {
        Init(typeof(CharacterState_AxeAttack03), 0.6f, 0.95f, InputList.FIRE1);
    }
}
