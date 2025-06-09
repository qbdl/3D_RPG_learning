using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag { ENTER, TREASURE, PEAK, DANGER, NEXT_LEVEL }

    public DestinationTag destinationTag; // 传送目的地标签
}
