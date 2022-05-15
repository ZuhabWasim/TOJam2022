using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI dialogueText;
	
	public Animator animator;
	public AudioSource playsound;
	
	private Queue<string> sentences;
	
	public void StartDialogue (Dialogue dialogue){
		animator.SetBool("isOpen",true);
		if (FindObjectOfType<PlayerController>() == true) {
			FindObjectOfType<PlayerController>().FreezeInput();
		}
		
		sentences = new Queue<string>();
		nameText.text = dialogue.name;
		sentences.Clear();
		
		foreach(string sentence in dialogue.sentences){
			sentences.Enqueue(sentence);
		}
		
		DisplayNextSentence(dialogue);
		
	}

	public void DisplayNextSentence(Dialogue dialogue){
		if (sentences.Count == 0){
			playsound.GetComponent<AudioSource>().Stop ();
			EndDialogue(dialogue);
			return;
		}
		
		string sentence = sentences.Dequeue();
		
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence, dialogue));
		
	}
	
	IEnumerator TypeSentence (string sentence, Dialogue dialogue){
		dialogueText.text = "";
		
		playsound.GetComponent<AudioSource>().Play();	
		
		foreach (char letter in sentence.ToCharArray()){
			dialogueText.text += letter;
			if (Input.GetKey(KeyCode.Space)){
				yield return null;
			} else {
				yield return new WaitForSeconds(0.04f);
			}
		}
		
		playsound.GetComponent<AudioSource>().Stop();
		
		while (!Input.GetKeyDown(KeyCode.Space))
			yield return null;
		
		yield return new WaitForSeconds(0.02f);
		DisplayNextSentence(dialogue);
		
	}
	


	
	void EndDialogue(Dialogue dialogue){
		animator.SetBool("isOpen",false);
		if (dialogue.onEnd == true){
			FindObjectOfType<PauseMenuUIManager>().StartFinalCutscene();
		}
		else{
			animator.SetBool("isOpen",false);
			if (FindObjectOfType<PlayerController>() == true) {
				FindObjectOfType<PlayerController>().FreezeInput();
			}
		}
	}
}
