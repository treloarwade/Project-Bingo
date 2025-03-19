public class BattleSlot
{
    public NetworkDingo Dingo { get; set; }
    public int SlotIndex { get; set; } // 0-3 for indexing
    public bool IsPlayer { get; set; } // True for player Dingos, false for opponents

    public BattleSlot(NetworkDingo dingo, int slotIndex, bool isPlayer)
    {
        Dingo = dingo;
        SlotIndex = slotIndex;
        IsPlayer = isPlayer;
    }
}
