using UnityEngine;
using XInputDotNetPure; // Required in C#

public class XInputTestCS : MonoBehaviour{

    void Start(){
        GamePad.SetVibration(0, 0.15f, 0.15f);    // SetVibration
        //Debug.Log(playerIndex);
    }
    
}
