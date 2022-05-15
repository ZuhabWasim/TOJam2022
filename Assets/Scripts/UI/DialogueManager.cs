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
		FindObjectOfType<PlayerController>().FreezeInput();
		
		sentences = new Queue<string>();
		nameText.text = dialogue.name;
		sentences.Clear();
		
		foreach(string sentence in dialogue.sentences){
			sentences.Enqueue(sentence);
		}
		
		DisplayNextSentence();
		
	}

	public void DisplayNextSentence(){
		if (sentences.Count == 0){
			playsound.GetComponent<AudioSource>().Stop ();
			EndDialogue();
			return;
		}
		
		string sentence = sentences.Dequeue();
		
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
		
	}
	
	IEnumerator TypeSentence (string sentence){
		dialogueText.text = "";
		
		playsound.GetComponent<AudioSource>().Play();	
		
		foreach (char letter in sentence.ToCharArray()){
			dialogueText.text += letter;
			if (Input.GetKey(KeyCode.Space)){
				yield return new WaitForSeconds(0.01f);
			} else {
				yield return new WaitForSeconds(0.04f);
			}
			
		}
		
		playsound.GetComponent<AudioSource>().Stop();
		
		while (!Input.GetKeyDown(KeyCode.Space))
			yield return null;
		
		yield return new WaitForSeconds(0.02f);
		DisplayNextSentence();
		
	}
	


	
	void EndDialogue(){
		
		animator.SetBool("isOpen",false);
		FindObjectOfType<PlayerController>().FreezeInput();
	}
}
