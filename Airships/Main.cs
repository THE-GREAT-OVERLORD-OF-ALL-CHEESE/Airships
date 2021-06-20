using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public class Airships : VTOLMOD
{
    public static Airships instance;

    public bool addedAirships;

    public GameObject airshipPrefab;

    public override void ModLoaded()
    {
        VTOLAPI.SceneLoaded += SceneLoaded;
        base.ModLoaded();

        instance = this;
        TryAddAirships();

        StartCoroutine("LoadAssetBundle");
    }

    private IEnumerator LoadAssetBundle()
    {
        Debug.Log("Checking " + ModFolder + "/airships.vab");
        if (!File.Exists(ModFolder + "/airships.vab"))
        {
            Debug.Log("Asset bundle does not exist");
            yield break;
        }
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(ModFolder + "/airships.vab");
        yield return request;

        AssetBundleRequest airshipRequest = request.assetBundle.LoadAssetAsync("Cheesenburg", typeof(GameObject));
        yield return airshipRequest;
        if (airshipRequest.asset == null)
        {
            Debug.Log("Couldnt find airship prefab");
            yield break;
        }
        airshipPrefab = airshipRequest.asset as GameObject;

        Debug.Log("airship prefabs loaded!");
    }

    private void TryAddAirships()
    {
        if (addedAirships == false) {
            foreach (Mod mod in VTOLAPI.GetUsersMods())
            {
                if (mod.name == "Custom Scenario Assets")
                {
                    Debug.Log("Airships has detected CSA, adding airships!");
                    AddAirships();
                    return;
                }
            }
            Debug.Log("Airships has not detected CSA, please enable CSA.");
        }
    }

    private void AddAirships()
    {
        CustomScenarioAssets.instance.AddCustomUnit(new CustomUnit_CommercialAirship(Teams.Allied, "Airships", "cheese_airships_commercialairship1_allied", "Commercial Airship", "This is a commercial airship used for advertising, it has 2 large LED pannels on the side to display messages.", UnitSpawn.PlacementModes.Any, true));
        CustomScenarioAssets.instance.AddCustomUnit(new CustomUnit_CommercialAirship(Teams.Enemy, "Airships", "cheese_airships_commercialairship1_enemy", "Commercial Airship", "This is a commercial airship used for advertising, it has 2 large LED pannels on the side to display messages.", UnitSpawn.PlacementModes.Any, true));

        Debug.Log("Airships added!");

        addedAirships = true;
    }

    private void SceneLoaded(VTOLScenes scene)
    {
        TryAddAirships();
        switch (scene)
        {
            case VTOLScenes.Akutan:
            case VTOLScenes.CustomMapBase:
            case VTOLScenes.CustomMapBase_OverCloud:
                StartCoroutine("SetupScene");
                break;
        }
    }

    private IEnumerator SetupScene()
    {
        while (VTMapManager.fetch == null || !VTMapManager.fetch.scenarioReady || FlightSceneManager.instance.switchingScene)
        {
            yield return null;
        }

        if (addedAirships == false) {
            TutorialLabel.instance.DisplayLabel("You have loaded the Airships mod without loading the Custom Scenario Assets mod. The Airships mod does not work without CSA, so make sure to install and load it. This message will disappear in 60 seconds.",
                    null,
                    60);
        }
    }
}