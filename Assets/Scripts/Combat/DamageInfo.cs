using UnityEngine;

public readonly struct DamageInfo
{
    public int Amount { get; }
    public Vector2 SourcePosition { get; }
    public bool HasSourcePosition { get; }

    public DamageInfo(int amount)
    {
        Amount = amount;
        SourcePosition = Vector2.zero;
        HasSourcePosition = false;
    }

    public DamageInfo(int amount, Vector2 sourcePosition)
    {
        Amount = amount;
        SourcePosition = sourcePosition;
        HasSourcePosition = true;
    }
}