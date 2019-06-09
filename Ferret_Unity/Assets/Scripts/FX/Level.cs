using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton pattern
public class Level : MonoBehaviour {
    [SerializeField] Transform m_fxRoot;
    [SerializeField] Transform m_vibrationRoot;

    static Level s_instance = null;

    static public Level GetInstance(){  // Permet de récupérer la référence de s_instance pour tout le monde sans la modifier
        return s_instance;
    }

    void Awake(){
        if(s_instance != null){
            Debug.LogError("There's more than one Level instance in the scene");    // Permet de vérifier si il n'y a pas plus d'une s_instance lancer dans le jeu
        }
        s_instance = this;  // "this" référence par rapport au script sur lequelle il est en train de "tourner/fonctionner"
    }

// ---------------------------------- Fonctions "static" permet de pouvoir les appeler de nimporte où ----------------------------------
    static public FX AddFX(GameObject model, Vector3 position, Quaternion rotation){
        if(model != null){	// Si la variable m_deadFX est différente de null alors :
			// Création d'une copie en mémoire de ce préfab dans la hiérarchie :
			GameObject go = Instantiate(model, position, rotation);	// On instentie l'objet original (model) avec une position (position) et une rotation (Quaternion.identity)
            go.transform.SetParent(GetInstance().m_fxRoot);
            return go.GetComponent<FX>();
		}
        return null;    // On instentie pas de nouveaux FX
    }
    
    static public Vibration AddVibration(GameObject model, Vector3 position){
        if(model != null){	// Si la variable model est différente de null alors :
			// Création d'une copie en mémoire de ce préfab dans la hiérarchie :
			GameObject go = Instantiate(model, position, Quaternion.identity);	// On instentie l'objet original (model) avec une position (position) et une rotation (Quaternion.identity)
            go.transform.SetParent(GetInstance().m_vibrationRoot);
            return go.GetComponent<Vibration>();
		}
        return null;    // On instentie pas de nouveaux FX
    }
    
}
