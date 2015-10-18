﻿using UnityEngine;
using System.Collections;

public class Enemies : Main {
	bool Targetlocked;
	public int pointvalue;
	public GameObject _Target; //ship referance
	public void SetEnemies(float _x, float _y, float _xScale, float _yScale, float _speed, int _health, int _level, bool _alive, int _pointvalue) {
		xPos = _x;
		yPos = _y;
		xScale = _xScale;
		yScale = _yScale;
		speed = _speed;
		Health = _health;
		Level = _level;
		alive = _alive;
		pointvalue = _pointvalue;

		if (Level <= 1 ) {
			GetComponent<Renderer>().material.color = EnemyType [0];//Spawn only reds in level 1
		}
		if (Level == 2) {
			GetComponent<Renderer>().material.color = EnemyType [Random.Range (0, 2)]; //red and green
		}
		if (Level > 2) {
			GetComponent<Renderer>().material.color = EnemyType [Random.Range (0, EnemyType.Length)];// red green and yellow
		}
		
		//ENEMY TYPES
		
		//Regular
		if (GetComponent<Renderer>().material.color == EnemyType [0]) {
			name="Enemy_R";//Uses default varibles supplied in Createnemy in main
			color=EnemyType[0];
		}
		if (GetComponent<Renderer>().material.color == EnemyType [1]) {
			name="Enemy_G";
			Health = 3;
			color=EnemyType[1];
			pointvalue=25;
			speed= Random.Range (0.08f, 0.08f);
		}
		//Lock on
		if (GetComponent<Renderer>().material.color == EnemyType [2]) {
			name="Enemy_Y";
			color=EnemyType[2];
			pointvalue=25;
			speed = Random.Range (0.08f, 0.14f);
		}
		GetComponent<Renderer> ().enabled = true;//Reset renderer after object is Respawned in Main class

		Resetpos ();

		LoadParticles(transform.position,color, speed,5,transform);//Once everything is set, create particles for each ship

		Vector3 scale = new Vector3(xScale, yScale, 0.1f);
		transform.localScale = scale;
		Targetlocked = false;
		EnemiesList.Add (gameObject);

	}

	public void Resetpos(){
		xPos = Random.Range (ScreenWidthLeft+xScale, ScreenWidthRight);//Spawns objects in range of -8, 8 as ints
		yPos = ScreenHeight + yScale;// Spawns above range of bullets
		Vector3 pos = new Vector3 (Mathf.Round((xPos - xScale / 2)*10)/10, yPos, 0);// so to prevent spawning of screen the equation is My spawn areaa(pos)-half of the enemies widthx-xscale/2, then add its size again to keep it going 1 left and push it 1 right
		transform.eulerAngles = new Vector3 (0, 0, 180f);
		transform.position = pos;
	}
	
	public void Findplayer() {
		if (alive==true) {
			if (ship != null) {//Only excute when ship exists
				if (_Target == null) { //If no Target, keep searching for one
					//Targetlocked = false;
					if (GameObject.FindGameObjectWithTag ("Player")) {
						_Target = GameObject.FindGameObjectWithTag ("Player");// Assigns referance Ship
					} 
					else {
						return; // once found, exit if statement
					}// end else
				}//end _Target
				if (_Target != null) {//Once Target is found, execute below
					float distance = (transform.position - _Target.transform.position).magnitude;//creates a float which stores position between A & B
					if (distance <= 1f)//If less than 1f..
						killplayer();//Activate GameOver method
				}
				//HOMING ENEMIES
				if (GetComponent<Renderer>().material.color == EnemyType [2] && this.gameObject.transform.position.y >= yScale/2){//If enemy is of type yellow, call lock on method and target enemy if its centre position is equal to half the players height
					Targetlocked = true;
					if (Targetlocked == true) {
						Vector3 Dir = _Target.transform.position - transform.position;// Get a direction Vector from Target to enemy
						Dir.Normalize ();// Normalize it so that it's a unit direction Vector, gives it a size of 1
						
						//ROTATE Enemy ship towards player
						Zangle = Mathf.Atan2 (Dir.y, Dir.x) * Mathf.Rad2Deg - 90; // Draws an angle facing the players position
						Quaternion AngleRotation = Quaternion.Euler (0, 0, Zangle);// Which axis the rotation will take place, in this case the X-Axis
						transform.rotation = Quaternion.RotateTowards (transform.rotation, AngleRotation, Enemyrotatespeed * Time.deltaTime); //How fast enemy rotates towards player

					}//end targetlocked if
				}//end enemytype2 if
				else
					Targetlocked = false;//Sends enemy downwards when its centre position is equal to half the players height
			} 
		}//end alive if
		else
			return;
	}//end FindPlayer
	
	
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
//		if (Input.GetKey (KeyCode.Space) && cooldown == 0) {
//			LoadShip (1);
//			FireBullets (this.transform);
//		}
		transform.Translate (Vector3.up * speed);
		ResetEnemies (gameObject.GetComponent<Enemies>());
		//Enemy Updates
		if (debugmode!=true)
			Findplayer (); //Find distance between player and enemy
	}//end Update
}//end main class
