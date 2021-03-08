using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : Singleton<GameSettingsManager>
{
    public GameSettingsData Settings { get; private set; } = new GameSettingsData();
}
