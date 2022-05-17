using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
	
	void Start(){
		if (dialogue.onEnd == true){
			TriggerDialogue();
		}
	}
	
	public void TriggerDialogue(){
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}
}
