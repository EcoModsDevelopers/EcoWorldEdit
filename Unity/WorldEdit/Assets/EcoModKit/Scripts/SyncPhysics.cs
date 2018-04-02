// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;
using AxisSyncMode = UnityEngine.Networking.NetworkTransform.AxisSyncMode;

// Interpolates game object physics over time from network updates
public partial class SyncPhysics : MonoBehaviour
{
    public bool ManuallyUpdated = false;

    public bool SyncPos;
    public bool SyncRot;
    public bool SyncVelocity = true;

    public AxisSyncMode RotSyncMode = AxisSyncMode.AxisXYZ;
    public bool useLocalPosition;
    public bool useLocalRotation;
    public float lockDistance = 10f;
    public float snapDistance = 5f;
    public float interpolateMovement = 1f;
    public float interpolateRotation = 1f;
    public bool syncInitialTransform = true;

    public bool debugKeyframes = false;
}
