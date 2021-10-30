using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public bool showController = false;
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;

    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        TryInitilaize();  // Initialize controllers
    }

    void TryInitilaize()  //Initialize to controllers
    {
        List<InputDevice> devices = new List<InputDevice>(); //devices is list of all connected devices

        //controllerCharacteristics = InputDeviceCharacteristics.Controller; //characteristics are set in UNITY.
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices); //pare down list to include ONLY controllers


        foreach (var item in devices) //for each controller...
        {
            Debug.Log(item.characteristics);
        }

        if (devices.Count > 0) //if at least one controller...
        {
            targetDevice = devices[0]; // There should be only ONE device, left or right controller
            Debug.Log("targetDevice is " + targetDevice.characteristics);
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);

            Debug.Log("prefab is " + prefab.name);//there should be a prefab (controller or hand model)

            if (prefab)
            {
                spawnedController = Instantiate(prefab, transform);
                Debug.Log("spawnedController is " + spawnedController.name);

            }
            else
            {
                Debug.LogError("Could not find corresponding controller model.");
                spawnedController = Instantiate(controllerPrefabs[0], transform);
            }

            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    }

    void UpdateHandAnimation()
    {   //get trigger input
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue); //animate the hand based on trigger value
        }
        else
            handAnimator.SetFloat("Trigger", 0);

        //get grip button value
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue); //animate the hand based on grip value
        }
        else
            handAnimator.SetFloat("Grip", 0);
    }


        // Update is called once per frame
        void Update()
    {
        if (!targetDevice.isValid) //If no target device...
        {
            TryInitilaize();
        }
        else   //update controllers.
        {
            if (showController)  // Show controller or hands
            {
                spawnedHandModel.SetActive(false);
                spawnedController.SetActive(true);
            }
            else
            {
                spawnedHandModel.SetActive(true);
                spawnedController.SetActive(false);
                UpdateHandAnimation();
            }
        }

        
    }
}
