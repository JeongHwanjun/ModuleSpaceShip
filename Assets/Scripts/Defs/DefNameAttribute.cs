using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public sealed class DefNameAttribute : PropertyAttribute
{
    // (선택) 나중에 Def 타입별 필터링하고 싶을 때 사용
    public readonly string baseDefClassName;

    public DefNameAttribute(string baseDefClassName = null)
    {
        this.baseDefClassName = baseDefClassName;
    }
}
