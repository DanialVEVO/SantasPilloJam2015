using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using KeyManager;
using UnityEngine.UI;

public class menu : MonoBehaviour {
    public GameObject ValidationMenu, ValidatedMenu, LoadingScreen;
    public Button ValidateButton;
    public InputField KeyInput;
    public Text DurationText, LoadingText;

	private string key = "";
	private KeyManager.KeyManager key_object;
	
	void Start() {
        //Instantiate the key object for the validation process.
		key_object = gameObject.AddComponent<KeyManager.KeyManager>() as KeyManager.KeyManager;
		key_object.debug = false;

        //Insert (ProductID, SecretID).
        //These will be prodvided by Pillo Games.
        //If not yet received, please request via support@pillogames.com
		key_object.setIDs("Y8ds8Jy0FntiLZi", "Vs5N32kJtafqZObDnagVYqKPvy8UAX2V");
		key_object.validate();
	}
	
	void Update() {
		if(key_object.LoadingInformation != KeyManager.Loading.None){
            //Show the loading screen.
            ValidationMenu.SetActive(false);
            ValidatedMenu.SetActive(false);
            LoadingScreen.SetActive(true);

            //Show the validation process.
			if(key_object.LoadingInformation == KeyManager.Loading.Validation){
                LoadingText.text = "Loading validation..";
			} else if (key_object.LoadingInformation == KeyManager.Loading.Activation){
                LoadingText.text = "Loading activation..";
			} else if (key_object.LoadingInformation == KeyManager.Loading.Deactivation){
                LoadingText.text = "Loading deactivation..";
			}
		} else if (!key_object.IsValidated){
            //Activate the Activation overlay.
            ValidationMenu.SetActive(true);
            ValidatedMenu.SetActive(false);
            LoadingScreen.SetActive(false);

            //Get the key from the input field.
            if(KeyInput.text != "") {
                ValidateButton.enabled = true;
                this.key = KeyInput.text.ToString();
            } else {
                ValidateButton.enabled = false;
            }
		} else {
            //Show the Validated screen.
            ValidationMenu.SetActive(false);
            ValidatedMenu.SetActive(true);
            LoadingScreen.SetActive(false);

            //Deactivate key when holding K and D down.
			if (Input.GetKey(KeyCode.K) && Input.GetKeyDown(KeyCode.D)) {
                Debug.Log("Deactivate");
				key_object.deactivate();
			}

            //Show de key duration on the screen.
            DurationText.text = "Days passed since activation: " + key_object.getDaysPassed().ToString("0");
		}
	}

    public void ActivateKey() {
        //Activate the current key from the key input.
        if(KeyInput.text != "") {
            key_object.activate(this.key);
        }
    }

    public void Continue() {
        //Continue to the game.
        //Make sure the KeySystem is scene 0 in the build settings and the game is scene 1.
        Application.LoadLevel(1);
    }
}