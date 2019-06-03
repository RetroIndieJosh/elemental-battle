using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArenaPlayer
{
    public string Name {  get { return player == null ? "Unset" : player.name; } }
    public Actor player = null;
    public int firstLevel = 0;
}
