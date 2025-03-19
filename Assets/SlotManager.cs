using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    private Dictionary<int, GameObject> slotAssignments = new Dictionary<int, GameObject>();
    private HashSet<int> occupiedSlots = new HashSet<int>();

    public bool IsSlotOccupied(int slot)
    {
        return occupiedSlots.Contains(slot);
    }

    public bool AssignDingoToSlot(int slot, GameObject dingo)
    {
        if (IsSlotOccupied(slot))
        {
            Debug.LogWarning($"[SlotManager] Slot {slot} is already occupied.");
            return false;
        }

        slotAssignments[slot] = dingo;
        occupiedSlots.Add(slot);
        Debug.Log($"[SlotManager] Assigned Dingo to Slot {slot}");
        return true;
    }

    public void ClearSlot(int slot)
    {
        if (slotAssignments.ContainsKey(slot))
        {
            Destroy(slotAssignments[slot]);
            slotAssignments.Remove(slot);
            occupiedSlots.Remove(slot);
            Debug.Log($"[SlotManager] Cleared Slot {slot}");
        }
    }

    public GameObject GetDingoInSlot(int slot)
    {
        if (slotAssignments.ContainsKey(slot))
        {
            return slotAssignments[slot];
        }
        return null;
    }
}