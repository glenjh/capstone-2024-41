using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] private NarrationEvent narration;
    
    public Narration[] GetNarration()
    {
        narration.narrations = DatabaseManager.instance.GetNarration((int)narration.line.x, (int)narration.line.y);
        return narration.narrations;
    }
}