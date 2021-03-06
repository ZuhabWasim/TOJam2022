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
	public int phase;
	
	private Queue<string> sentences;
	
	public void StartDialogue (Dialogue dialogue){
		animator.SetBool("isOpen",true);
		if (FindObjectOfType<PlayerController>() == true) {
			FindObjectOfType<PlayerController>().FreezeInput(true);
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
			FindObjectOfType<PlayerController>().FreezeInput(false);
			FindObjectOfType<BossCutsceneManager>().KillBoss();
		}
		if(dialogue.onStart == true){
			GameObject go2 = GameObject.Find("DialogueTrigger");
			DialogueTrigger trigger2 = (DialogueTrigger) go2.GetComponent(typeof(DialogueTrigger));
			trigger2.TriggerDialogue();
			FindObjectOfType<SoundManager>().PlayBossLaugh();
		}
		if (dialogue.onBoss == true){
			FindObjectOfType<PlayerController>().FreezeInput(false);
			FindObjectOfType<BossAI>().StartBossFight();
		}
		
		else{
			if (phase == 0){
				FindObjectOfType<BossCutsceneManager>().MoveToArena();
				phase += 1;
			}
			animator.SetBool("isOpen",false);
			if (FindObjectOfType<PlayerController>() == true) {
				FindObjectOfType<PlayerController>().FreezeInput(false);
			}
		}
	}
}
