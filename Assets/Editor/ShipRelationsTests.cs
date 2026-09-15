#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ModuleSpaceShip.Defs;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class ShipRelationsTests
{
    [TestCase(FactionId.Player, FactionId.Player, RelationType.Friendly)]
    [TestCase(FactionId.Enemy, FactionId.Enemy, RelationType.Friendly)]
    [TestCase(FactionId.Independent, FactionId.Independent, RelationType.Friendly)]
    [TestCase(FactionId.Player, FactionId.Enemy, RelationType.Hostile)]
    [TestCase(FactionId.Enemy, FactionId.Player, RelationType.Hostile)]
    [TestCase(FactionId.Independent, FactionId.Player, RelationType.Neutral)]
    [TestCase(FactionId.Player, FactionId.Independent, RelationType.Neutral)]
    [TestCase(FactionId.Independent, FactionId.Enemy, RelationType.Neutral)]
    [TestCase(FactionId.Enemy, FactionId.Independent, RelationType.Neutral)]
    public void DefaultTableDefinesAllFactionPairs(FactionId first, FactionId second, RelationType expected)
    {
        var table = AssetDatabase.LoadAssetAtPath<FactionRelationTable>("Assets/Settings/FactionRelations.asset");
        Assert.That(table, Is.Not.Null);
        Assert.That(table.GetRelation(first, second), Is.EqualTo(expected));
    }

    [UnityTest]
    public IEnumerator RelationsAndTargetsFollowShipLifecycle()
    {
        yield return new EnterPlayMode();
        Assert.That(ShipManager.Instance, Is.Null, "빈 테스트 씬에서 실행해야 합니다.");
        DefDatabase.Register(new ShipDef { defName = "RelationTestShip" });
        var objects = new List<GameObject>();
        GameObject managerObject = new("Relation test manager");
        managerObject.SetActive(false);
        var manager = managerObject.AddComponent<ShipManager>();
        SetField(typeof(ShipManager), manager, "factionRelations",
            AssetDatabase.LoadAssetAtPath<FactionRelationTable>("Assets/Settings/FactionRelations.asset"));
        managerObject.SetActive(true);
        objects.Add(managerObject);

        try
        {
            ComputerShip ai = CreateShip(FactionId.Enemy, objects, true);
            var controller = ai.GetComponent<ComputerShipController>();
            ai.SetControlIntent(new ShipControlIntent(Vector2.up, 1f, true));
            yield return null;
            yield return null;
            Assert.That(controller.HasTarget, Is.False);
            Assert.That(ai.GetRelation(null), Is.EqualTo(RelationType.Neutral));
            Assert.That(GetField<Vector2>(ai, "currentMoveIntent"), Is.EqualTo(Vector2.zero));
            Assert.That(GetField<float>(ai, "currentTurnIntent"), Is.Zero);

            ComputerShip friendly = CreateShip(FactionId.Enemy, objects);
            ComputerShip neutral = CreateShip(FactionId.Independent, objects);
            ComputerShip enemy = CreateShip(FactionId.Player, objects);
            yield return null;
            yield return null;
            Assert.That(ai.Relations.Count, Is.EqualTo(3));
            Assert.That(ai.Relations.ContainsKey(ai), Is.False);
            Assert.That(ai.GetRelation(friendly), Is.EqualTo(RelationType.Friendly));
            Assert.That(ai.GetRelation(neutral), Is.EqualTo(RelationType.Neutral));
            Assert.That(ai.GetRelation(enemy), Is.EqualTo(RelationType.Hostile));
            Assert.That(enemy.GetRelation(ai), Is.EqualTo(RelationType.Hostile));
            Assert.That(controller.CurrentTarget, Is.SameAs(enemy));

            ai.SetRelation(ai, RelationType.Hostile);
            Assert.That(ai.Relations.ContainsKey(ai), Is.False);
            ai.SetRelation(enemy, RelationType.Friendly);
            yield return null;
            yield return null;
            Assert.That(controller.CurrentTarget, Is.Null);
            Assert.That(enemy.GetRelation(ai), Is.EqualTo(RelationType.Hostile), "개별 관계는 방향을 가진다.");

            ComputerShip secondEnemy = CreateShip(FactionId.Player, objects);
            yield return null;
            yield return null;
            Assert.That(ai.GetRelation(enemy), Is.EqualTo(RelationType.Friendly), "새 등록이 기존 관계를 덮어쓰면 안 된다.");
            Assert.That(controller.CurrentTarget, Is.SameAs(secondEnemy));
            ai.SetRelation(enemy, RelationType.Hostile);
            yield return null;
            yield return null;
            Assert.That(controller.CurrentTarget, Is.SameAs(secondEnemy), "유효한 기존 표적은 유지한다.");

            secondEnemy.DestroyShip();
            Assert.That(secondEnemy.IsDestroyed, Is.True);
            Assert.That(controller.HasTarget, Is.False);
            Assert.That(ai.Relations.ContainsKey(secondEnemy), Is.False);
            yield return null;
            yield return null;
            Assert.That(controller.CurrentTarget, Is.SameAs(enemy));

            bool stoppedFiring = false;
            ai.OnTryFireStop += () => stoppedFiring = true;
            ai.SetControlIntent(new ShipControlIntent(Vector2.one, -1f, true));
            Object.Destroy(enemy.gameObject);
            yield return null;
            yield return null;
            Assert.That(controller.CurrentTarget, Is.Null);
            Assert.That(controller.HasTarget, Is.False);
            Assert.That(ai.Relations.Count, Is.EqualTo(2));
            Assert.That(stoppedFiring, Is.True);
            Assert.That(GetField<Vector2>(ai, "currentMoveIntent"), Is.EqualTo(Vector2.zero));
            Assert.That(GetField<float>(ai, "currentTurnIntent"), Is.Zero);

            controller.enabled = false;
            ComputerShip lateEnemy = CreateShip(FactionId.Player, objects);
            controller.enabled = true;
            yield return null;
            yield return null;
            Assert.That(controller.CurrentTarget, Is.SameAs(lateEnemy));

            ComputerShip lateAI = CreateShip(FactionId.Enemy, objects, true);
            yield return null;
            yield return null;
            Assert.That(lateAI.GetComponent<ComputerShipController>().CurrentTarget, Is.SameAs(lateEnemy),
                "AI보다 먼저 등록된 함선도 표적 후보가 되어야 한다.");
            Assert.That(ai.GetRelation(lateAI), Is.EqualTo(RelationType.Friendly));

            Object.Destroy(managerObject);
            yield return null;
            yield return null;
            Assert.That(controller.CurrentTarget, Is.Null);
            Assert.That(ai.Relations.Count, Is.Zero);
        }
        finally
        {
            for (int i = objects.Count - 1; i >= 0; i--)
                if (objects[i] != null) Object.DestroyImmediate(objects[i]);
        }
        yield return new ExitPlayMode();
    }

    private static ComputerShip CreateShip(FactionId faction, List<GameObject> objects, bool controller = false)
    {
        GameObject go = new("Relation test " + faction);
        objects.Add(go);
        go.SetActive(false);
        go.AddComponent<Rigidbody2D>().gravityScale = 0f;
        var ship = go.AddComponent<ComputerShip>();
        SetField(typeof(Ship), ship, "def", "RelationTestShip");
        SetField(typeof(Ship), ship, "faction", faction);
        if (controller) go.AddComponent<ComputerShipController>();
        go.SetActive(true);
        return ship;
    }

    private static void SetField(System.Type type, object target, string name, object value)
        => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    private static T GetField<T>(Ship ship, string name)
        => (T)typeof(Ship).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ship);
}
#endif
