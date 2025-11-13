using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class PrototypeSaveSystem
{
    public static void PrototypeSavePlayer(TyrantController tyrant)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/prototypePlayer.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PrototypeSaveData data = new PrototypeSaveData(tyrant);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PrototypeSaveData PrototypeLoadPlayer()
    {
        string path = Application.persistentDataPath + "/prototypePlayer.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PrototypeSaveData data = formatter.Deserialize(stream) as PrototypeSaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("No save file found in " + path);
            return null;
        }
    }
}

[Serializable]
public class PrototypeSaveData
{
    public double biomass;
    public float liftSpeed;
    public float liftSpeedMultiplier;
    public int liftSpeedUpgradesOwned;
    public int liftSpeedUpgradeNextPrice;

    public bool larynxIsActive;

    public float biomassPerHuman;
    public int biomassPerHumanUpgradesOwned;
    public int biomassPerHumanUpgradeNextPrice;

    public PrototypeSaveData(TyrantController tyrant)
    {
        biomass = tyrant.gameManager.biomass;
        liftSpeed = tyrant.liftSpeed;
        liftSpeedMultiplier = tyrant.liftSpeedMultiplier;
        liftSpeedUpgradesOwned = tyrant.liftSpeedUpgradesOwned;
        liftSpeedUpgradeNextPrice = tyrant.liftSpeedUpgradeNextPrice;

        larynxIsActive = tyrant.larynxIsActive;

        biomassPerHuman = tyrant.biomassPerHuman;
        biomassPerHumanUpgradesOwned = tyrant.biomassPerHumanUpgradesOwned;
        biomassPerHumanUpgradeNextPrice = tyrant.biomassPerHumanUpgradeNextPrice;
    }
}
