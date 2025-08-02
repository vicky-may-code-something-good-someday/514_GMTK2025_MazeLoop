using UnityEngine;

public class StateInTime
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    public StateInTime(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}
