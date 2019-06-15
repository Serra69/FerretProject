using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class ProtectMouseInput : MonoBehaviour {
    
    EventSystem m_eventSystem;

    void Start(){
        m_eventSystem = GetComponent<EventSystem>();
    }

    void Update(){
        if(m_eventSystem.currentSelectedGameObject == null){
            m_eventSystem.SetSelectedGameObject(m_eventSystem.firstSelectedGameObject);
        }
    }

}