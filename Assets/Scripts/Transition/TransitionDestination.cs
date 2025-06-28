using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        ENTER,
        TREASURE,
        MONSTER,
        SECRET_AREA,
        PEAK_GRAY, PEAK_RED,
        PEAK_DRAGON_IN, PEAK_DRAGON_OUT,
        PEAK_BOSS_IN, PEAK_BOSS_OUT,
        DANGER,
        NEXT_LEVEL
    }

    public DestinationTag destinationTag; // 传送目的地标签
}
