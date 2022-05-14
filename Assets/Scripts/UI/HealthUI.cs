using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;	

public class HealthUI : MonoBehaviour
{
	[Header("UI Variables")]
	public int health; // The current health of the player 
	public int numOfHearts; // The total health of the player
	
	public Image[] hearts; // The array of heart images
	public Sprite fullHeart; // The sprite that indicates a full heart
	public Sprite emptyHeart; // The sprite that indicates an empty heart
	
    // Update is called once per frame
    void Update()
    {
		// If the current health is greater than the total health of the player
        if (health > numOfHearts){
			// Set the current health to be the total health (aka don't overflow the allowed amount of health)
			health = numOfHearts;
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
			
			if (i < numOfHearts){
				// If the index of the heart sprite is less than the total health, then enable the sprite
				hearts[i].enabled = true;
			} else {
				// Otherwise, disable the sprite
				hearts[i].enabled = false;
			}
		}
    }
}
