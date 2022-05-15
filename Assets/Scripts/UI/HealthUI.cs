using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;	

public class HealthUI : MonoBehaviour
{
	[Header("UI Variables")]
	float health; // The current health of the player 
	float maxHealth; // The total health of the player
	
	public Image[] hearts; // The array of heart images
	public Sprite fullHeart; // The sprite that indicates a full heart
	public Sprite emptyHeart; // The sprite that indicates an empty heart
	

	void Start()
    {
         health = GetComponent<Health>().health ;
		 maxHealth = GetComponent<Health>().maxHealth ;
    }
    // Update is called once per frame
    void Update()
    {
		health = GetComponent<Health>().health ;
		maxHealth = GetComponent<Health>().maxHealth ;
		// If the current health is greater than the total health of the player
        if (health > maxHealth){
			// Set the current health to be the total health (aka don't overflow the allowed amount of health)
			health = maxHealth;
		}
		
		// Iterate through all the heart sprites
		for (int i = 0; i < hearts.Length; i++){
			
			if (i < health){
				// If the index of the heart sprite is less than the current health, show a full heart sprite
				hearts[i].sprite = fullHeart;
			} else { 
				// Otherwise, show the empty heart sprite
				hearts[i].sprite = emptyHeart;
			}
			
			if (i < maxHealth){
				// If the index of the heart sprite is less than the total health, then enable the sprite
				hearts[i].enabled = true;
			} else {
				// Otherwise, disable the sprite
				hearts[i].enabled = false;
			}
		}
    }
}
