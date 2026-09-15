using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>함선 생성 시 복사할 세력 관계. 한 행이 양방향 관계를 나타낸다.</summary>
[CreateAssetMenu(menuName = "ModuleSpaceShip/Faction Relation Table")]
public sealed class FactionRelationTable : ScriptableObject
{
    [Serializable]
    private struct Entry
    {
        public FactionId first;
        public FactionId second;
        public RelationType relation;
    }

    [SerializeField] private List<Entry> entries = new();

    public RelationType GetRelation(FactionId first, FactionId second)
    {
        // 중복 행이 있으면 마지막 행이 우선한다.
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            Entry entry = entries[i];
            if ((entry.first == first && entry.second == second) ||
                (entry.first == second && entry.second == first))
                return entry.relation;
        }

        return first == second ? RelationType.Friendly : RelationType.Neutral;
    }
}
