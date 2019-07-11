using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Lesson
{
    public interface IHealth
    {
        int hp { get; set; }
        bool isAlive { get; }
    }

}
