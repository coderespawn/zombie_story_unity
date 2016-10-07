//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;

public class GrassTransformRule : TransformationRule {

    public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale) {
        base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);

        var angle = random.Range(0.0f, Mathf.PI * 2);
        var rotation = Quaternion.Euler(0, angle, 0);
        outRotation = rotation;

        float cellSize = 20;
        float halfCellSize = cellSize / 2.0f;
        outPosition.x = random.Range(-halfCellSize, halfCellSize);
        outPosition.z = random.Range(-halfCellSize, halfCellSize);
    }
}
