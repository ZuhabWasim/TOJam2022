using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
	
	void Start(){
		if (dialogue.onStart == true){
			TriggerDialogue();
		} else {return;}
	}
	
	public void TriggerDialogue(){
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}
}
