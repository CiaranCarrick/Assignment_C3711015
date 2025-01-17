﻿using UnityEngine;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	//Inheritance
	public Color[] EnemyType= {Color.red, Color.green, Color.yellow};// Enemy types(Colors) stored in array
	public static List <GameObject> EnemiesList= new List<GameObject>();

	AudioSource bulletsound;
	public static AudioSource explosionsound; //Accessed from ShipShoot
	AudioClip bulletaudioclip;
	AudioClip explosionaudioclip;

	GameObject background; //2d Background image

	public static GameObject ParticleManager;// Used to clean up Hierarchy
	GameObject StarManager;//Holds stars

	float EnemySpawnTime; // How long between each spawn.

	public float xPos;// These will be used to contain values for each methods constructor
	public float yPos;//
	public float xScale;//
	public float yScale;//
	public float speed;//
	public Color color;//
	public int Health;//

	public float Enemyrotatespeed=40f;// Speed of lock on enemy rotation
	public float Zangle;

	public static GameObject ship;
	public static int score; 
	public int mycooldown;
	public int Level;
	public int cooldown;//Weapon Cooldown
    public static int ScreenWidthLeft= -8;
	public static int ScreenWidthRight = 8;
	public int ScreenHeight = 20;

	public float Leveltime;
	public bool debugmode=false;

	void Player(){
		ship = GameObject.CreatePrimitive (PrimitiveType.Quad);//assign Ship gameobject with a Cube
		ship.name = "Ship";
		ship.AddComponent<Ship> (); //Attach Ship script to ship GameObject
		ship.AddComponent<Movement> ();
		Ship myship = ship.GetComponent<Ship> (); // Create Instance of Enemies called myenemies
		myship.GetComponent<Ship>().SetShip (0, -1, 1.0f, 1.0f, 0.4f, new Color(100,0f,255f, 1f));
	}//End Player
	
	
	void CreateEnemies(){
		for (int i=1; i<=Level; i++) {
			GameObject enemy = GameObject.CreatePrimitive (PrimitiveType.Quad);
			enemy.name = "Enemy";
			enemy.AddComponent<Enemies> ();
			Enemies myenemies = enemy.GetComponent<Enemies> (); // Create Instance of Enemies called myenemies
			myenemies.SetEnemies (0, 0, 1, 1, 0.1f, EnemyType [0], 1, Level);//_x, _y, _xScale, _yScale, _speed,  _color, _health _Level
			EnemiesList.Add (enemy);
		}
	}//End CreateEnemies


	public void CreateBonus(Vector3 _pos){//Insert Vector3 
		GameObject bonus=GameObject.CreatePrimitive(PrimitiveType.Capsule);
		bonus.AddComponent<Bonus> ();
		bonus.transform.localScale = new Vector3(0.2f, 0.2f, 0.1f);
		bonus.transform.position = _pos;
	}


	public void CreateBullets(){
			GameObject bullet = GameObject.CreatePrimitive (PrimitiveType.Quad);
			bullet.name = "Bullet";
			float _xpos = ship.transform.position.x;// gives same position of ship
			float _ypos = ship.transform.position.y;//sets bullet at tip of ship
			bullet.AddComponent<ShipShoot> ();
			bullet.AddComponent<Collisions> ();
			ShipShoot sbullet = bullet.GetComponent<ShipShoot> ();
			sbullet.SetBullet (_xpos, _ypos, 0.2f, 0.3f, 0.4f, explosionsound, mycooldown); //float _x, float _y, float _xScale, float _yScale, float _speed
	}//End CreateBullets


	void CreateStars(int _starCount){ //Takes in starCount from field above
		for (int i =1; i<=_starCount; i++) {
			GameObject stars=GameObject.CreatePrimitive(PrimitiveType.Quad);
			stars.name = "Star";
			stars.GetComponent<Collider>().enabled = false; 
			stars.AddComponent<Stars>();
			Stars sstars=stars.GetComponent<Stars>();
			sstars.SetStars(0, 0, 0.05f, 1.0f,Random.Range(-5f, -30f));
			stars.transform.parent=StarManager.transform;
		}
	}//End CreateStars

	
	void Background(){
		background = GameObject.CreatePrimitive (PrimitiveType.Quad); //Flat plane
		if (background != null) {
			background.AddComponent<BackgroundScroll> ();
			background.GetComponent<Renderer> ().material.mainTexture = Resources.Load<Texture2D> ("Level_1");//Quad Texture
			background.name = "BackGround";// Texture Name
			background.transform.position = new Vector3 (0f,5f, 5f);
			background.GetComponent<Renderer> ().material.mainTextureScale = new Vector2 (1, 1);//Controls tiling on tecture
			background.transform.localScale = new Vector3 (16f, 30f, 0f);
			background.GetComponent<Renderer> ().material.shader = Shader.Find ("Unlit/Texture");// Removes light effect on texture"Assets/StarSkyBox"
			}

	}//End Background
	

	public void ScoreM(){
		GameObject SM = new GameObject ();//assign Ship gameobject with a Cube
		SM.transform.position = new Vector3 (0, 0, -10f);
		SM.name = "ScoreM";
		SM.AddComponent<UI> ();
	}//End ScoreM


	public void CreateParticles(Vector3 pos, Color _col, float _spd, int part_amount){// Targets pos, Targets Color & Target speed
		for (int i = 0; i < part_amount; i++) {
			GameObject particle = GameObject.CreatePrimitive (PrimitiveType.Quad);
			particle.name = "Particle";
			particle.AddComponent<Particles> ();
			Particles party = particle.GetComponent<Particles> ();
			Vector3 Direction = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), 0); //Random directions for particles
			party.SetupParticle (pos.x, pos.y, Random.Range (0.05f, 0.1f), Random.Range (0.05f, 0.1f), _spd, Direction, _col, 3);
			particle.transform.parent=ParticleManager.transform;
		}
	}

	public void SubtractLife(GameObject _Tar) { //Method with GameObject, Parameter that decreses Health Variable, used in ShipShoot's update
		//Debug.Log ("Start health" + Health);
		Health--;
		if(Health > 0)
			return;
		else
			Destroy(this.gameObject);
		//Debug.Log ("HIT " + Health);
	}


	void NextLevel() {
		//Adjust Difficulty //Every new level, enemies will spawn quicker, regardless of player kills.
		//Refresh Invoke
		CancelInvoke ("CreateEnemies");
		if (EnemySpawnTime != 0.50f) {
			EnemySpawnTime -= 0.25f; //Decrease EnemySpawnTime
		}
		InvokeRepeating ("CreateEnemies", 1f, EnemySpawnTime); //Re-intialize Invoke with new EnemySpawnTime value
		//end Adjust
		foreach (GameObject enemy in EnemiesList)//Clear Screen of enemies
		{
			EnemiesList.Remove(gameObject); //Remove enemy Gameobject from List, also avoids missingexception
			Destroy(enemy.gameObject);
		}
		Leveltime = 50; 
		if (Level!=6)
			Level++;
		if(Level==5){
			background.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D> ("Level_final"); //Apply texture to level 5 from resource folder
		} 
		if (Level == 6) {
			Level= 1;
			GameOver();
			InvokeRepeating("Partytime", 1f, 1f); //Call after final level is complete
		}
	}//end NextLevel


	void Partytime(){//No game is complete without some Confetti!
		Vector3 Party = new Vector3 (Random.Range (ScreenWidthLeft, ScreenWidthRight), Random.Range (0, ScreenHeight), 0.1f);
		CreateParticles(Party, new Color(Random.Range(0.1f, 1f),Random.Range(0.1f, 1f),Random.Range(0.1f, 1f),0), 0.1f, 20); // Shoot randomly coloured particles around
	}


	public void GameOver(){
		Destroy (ship.gameObject);
		CreateParticles (transform.position, ship.GetComponent<Renderer>().material.color, speed, 100);
	}
	
	
	public void ResetEnemies(){
		if (transform.position.y <= -5f) {// Resets position once it reachs -1
			EnemiesList.Remove(gameObject);
			Destroy(this.gameObject);
		}
	}

	public void ChangeScore (int NewScore) // Add Score Method
	{
		score += NewScore;
		return;
	}


	void Start () {
		//SET AUDIO
		bulletsound = gameObject.AddComponent<AudioSource> ();//Adding AudioSource Components
		bulletsound.volume = 0.05f;
		explosionsound = gameObject.AddComponent<AudioSource> ();//
		explosionsound.volume = 0.1f;

		bulletaudioclip = (AudioClip)Resources.Load ("Sounds/Shoot1");// Loading the tracks from Resources
		explosionaudioclip = (AudioClip)Resources.Load ("Sounds/Explosion4");//

		bulletsound.clip = bulletaudioclip; //Assigning the bullet clips to the AudioSource Components
		explosionsound.clip = explosionaudioclip;//
		//
		StarManager = new GameObject ();// Contains all stars in a scene
		StarManager.name="Stars";
		ParticleManager = new GameObject ();// Contains all particles in a scene
		ParticleManager.name="Particles";
		mycooldown = 15;
		EnemySpawnTime = 2.00f;
		Leveltime = 30;
		Level =1;
		score = 0;
		Background ();
		Player ();
		CreateStars(20); //set _starCount amount here
		ScoreM ();
		InvokeRepeating ("CreateEnemies", 1f,EnemySpawnTime);
	}//End Start
	

	void Update () {
		if (ship) //Ship must exist or not be null for bullets to be fired
		{
			if (cooldown>0)
			{
				cooldown--;
			}
			if (Input.GetKey(KeyCode.Space)&& cooldown ==0)
			{
				bulletsound.Play();
				CreateBullets(); //call the CreateBullet function
				cooldown=mycooldown;
				if (mycooldown<=3)
					mycooldown=3; //max speed of bullets after all bonus' are picked up
			}
			if (debugmode!=true) {
				if (Leveltime <= 0)
				{
					NextLevel();
				} 
				
				else 
				{
					Leveltime -= Time.deltaTime;
				}
			}//end Debug
		}//end ship
	}//End Update

}//End Main Class
