using System;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong ClientId;
    public int ColorId;

    public bool Equals(PlayerData other)
    {
        return ClientId == other.ClientId && ColorId == other.ColorId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref ColorId);
    }
}
