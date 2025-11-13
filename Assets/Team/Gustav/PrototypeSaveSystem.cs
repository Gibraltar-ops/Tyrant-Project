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
            Debug.LogError("No save file found in " + path);
            return null;
        }
    }
}

[Serializable]
public class PrototypeSaveData
{
    public float liftSpeed;
    public float liftSpeedMultiplier;
    public float liftSpeedBase;
    public float liftSpeedPercentageIncrement;
    public int liftSpeedUpgradesOwned;
    public int liftSpeedUpgradeBasePrice;
    public int liftSpeedUpgradeNextPrice;
    public float liftSpeedUpgradePriceGrowthRate;

    public bool larynxIsActive;

    public float biomassPerHuman;
    public float biomassPerHumanBase;
    public float biomassPerHumanIncrement;
    public int biomassPerHumanUpgradesOwned;
    public int biomassPerHumanUpgradeBasePrice;
    public int biomassPerHumanUpgradeNextPrice;
    public float biomassPerHumanUpgradePriceGrowthRate;

    public PrototypeSaveData(TyrantController tyrant)
    {
        liftSpeed = tyrant.liftSpeed;
        liftSpeedMultiplier = tyrant.liftSpeedMultiplier;
        liftSpeedBase = tyrant.liftSpeedBase;
        liftSpeedPercentageIncrement = tyrant.liftSpeedPercentageIncrement;
        liftSpeedUpgradesOwned = tyrant.liftSpeedUpgradesOwned;
        liftSpeedUpgradeBasePrice = tyrant.liftSpeedUpgradeBasePrice;
        liftSpeedUpgradeNextPrice = tyrant.liftSpeedUpgradeNextPrice;
        liftSpeedUpgradePriceGrowthRate = tyrant.liftSpeedUpgradePriceGrowthRate;

        larynxIsActive = tyrant.larynxIsActive;

        biomassPerHuman = tyrant.biomassPerHuman;
        biomassPerHumanBase = tyrant.biomassPerHumanBase;
        biomassPerHumanIncrement = tyrant.biomassPerHumanIncrement;
        biomassPerHumanUpgradesOwned = tyrant.biomassPerHumanUpgradesOwned;
        biomassPerHumanUpgradeBasePrice = tyrant.biomassPerHumanUpgradeBasePrice;
        biomassPerHumanUpgradeNextPrice = tyrant.biomassPerHumanUpgradeNextPrice;
        biomassPerHumanUpgradePriceGrowthRate = tyrant.biomassPerHumanUpgradePriceGrowthRate;
    }
}
