using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
	public float speed;
	public int health;
	public int enemyScore;

	//Rigidbody2D rigid;
	SpriteRenderer spriteRenderer;
	public Sprite[] sprites;

	public GameObject bulletObjA;
	public GameObject bulletObjB;
	public float maxShotDelay; //최대
	public float curShotDelay; //현재
	public string enemyName;

	public GameObject player;
	public ObjectManager objectManager;
	public GameManager gameManager;

	public GameObject itemCoin;
	public GameObject itemBoom;
	public GameObject itemPower;

	Animator anim;

	public int patternIndex;
	public int curPatternCount;
	public int[] maxPatternCount;

	void Awake()
	{
		
		spriteRenderer = GetComponent<SpriteRenderer>();
		/*
		rigid = GetComponent<Rigidbody2D>();
		rigid.velocity = Vector2.down * speed;
		*/
		if(enemyName == "B")
			anim = GetComponent<Animator>();
	}

	void OnEnable() //컴포넌트가 활성화 될 때 호출되는 생명주기함수
	{
		switch(enemyName)
		{
			case "B":
				health = 200;
				Invoke("Stop", 1.5f);
				break;
			case "L":
				health = 40;
				break;
			case "M":
				health = 10;
				break;
			case "S":
				health = 3;
				break;
		}
	}

	void Stop()
	{
		if (!gameObject.activeSelf)
			return;
		Rigidbody2D rigid = GetComponent<Rigidbody2D>();
		rigid.velocity = Vector2.zero;

		Invoke("Think", 2);
	}

	void Think()
	{
		patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;

		//패턴 변경 시, 패턴 실행 횟수 변수 초기화
		curPatternCount = 0;

		switch(patternIndex)
		{
			case 0:
				FireForward();
				break;
			case 1:
				FireShot();
				break;
			case 2:
				FireArc();
				break;
			case 3:
				FireAround();
				break;

		}
	}

	void FireForward()
	{
		if (health <= 0)
			return;

		//Fire 4 Bullet Foward
		GameObject bulletRR = objectManager.MakeObj("bulletEnemyC");
		bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
		GameObject bulletR = objectManager.MakeObj("bulletEnemyC");
		bulletR.transform.position = transform.position + Vector3.right * 0.3f;
		GameObject bulletL = objectManager.MakeObj("bulletEnemyC");
		bulletL.transform.position = transform.position + Vector3.left * 0.3f;
		GameObject bulletLL = objectManager.MakeObj("bulletEnemyC");
		bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

		Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
		Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
		Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
		Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

		rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
		rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
		rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
		rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
		
		//Pattern Counting
		//각 패턴별 횟수를 실행하고 다음 패턴으로 넘어가도록 구현
		curPatternCount++;

		if(curPatternCount < maxPatternCount[patternIndex])
			Invoke("FireForward", 2);
		else
			Invoke("Think", 3);
	}
	void FireShot()
	{
		if (health <= 0)
			return;

		//Fire 5 Random Shotgun Bullet to Player
		for (int index = 0; index < 5; index++)
		{
			GameObject bullet = objectManager.MakeObj("bulletEnemyB");
			bullet.transform.position = transform.position;
			//Instantiate(bulletObjA, transform.position, transform.rotation);
			Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
			Vector2 dirVec = player.transform.position - transform.position;
			//목표물로 방향 = 목표물 위치 - 자신의 위치
			Vector2 ranVec = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0f, 2f));
			dirVec = dirVec + ranVec;

			rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
		}

		//Pattern Counting
		curPatternCount++;

		if (curPatternCount < maxPatternCount[patternIndex])
			Invoke("FireShot", 3.5f);
		else
			Invoke("Think", 3);
	}
	void FireArc()
	{
		if (health <= 0)
			return;

		//Fire 5 Arc Continue Fire
		GameObject bullet = objectManager.MakeObj("bulletEnemyA");
		bullet.transform.position = transform.position;
		bullet.transform.rotation = Quaternion.identity;
		//Quaternion.identity : rotation을 0로한다.

		//Instantiate(bulletObjA, transform.position, transform.rotation);
		Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
		Vector2 dirVec = new Vector2(Mathf.Sin(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), -1);
		//Mathf.Sin() : 삼각함수sin
		//Cos을 해도 시작부분만 다를뿐 똑같다.

		rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
		

		//Pattern Counting
		curPatternCount++;

		if (curPatternCount < maxPatternCount[patternIndex])
			Invoke("FireArc", 0.15f);
		else
			Invoke("Think", 3);
	}
	void FireAround()
	{
		if (health <= 0)
			return;

		//Fire Around
		int roundNumA = 50;
		int roundNumB = 40;
		//패턴 횟수에 따라 생성되는 총알 갯수 조절로 난이도 상승
		int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;

		for (int index = 0; index < roundNum; index++)
		{
			GameObject bullet = objectManager.MakeObj("bulletEnemyD");
			bullet.transform.position = transform.position;
			bullet.transform.rotation = Quaternion.identity;
			//Quaternion.identity : rotation을 0로한다.

			//Instantiate(bulletObjA, transform.position, transform.rotation);
			Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
			//생성되는 총알의 순번을 활용하여 방향 결정
			Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum),
										 Mathf.Sin(Mathf.PI * 2 * index / roundNum));

			rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

			Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
			//Vector3.forward * 90 : 보정값
 			bullet.transform.Rotate(rotVec);
		}

		//Pattern Counting
		curPatternCount++;

		if (curPatternCount < maxPatternCount[patternIndex])
			Invoke("FireAround", 0.7f);
		else
			Invoke("Think", 3);
	}

	void Update()
	{
		if (enemyName == "B")
			return;

		Fire();
		Reload();
	}

	public void OnHit(int dmg)
	{
		if (health <= 0)
			return;

		health = health - dmg;
		if(enemyName == "B")
		{
			anim.SetTrigger("OnHit");
		}
		else
		{
			spriteRenderer.sprite = sprites[1];
			Invoke("ReturnSprite", 0.1f);
		}


		if(health <= 0)
		{
			Player playerLogic = player.GetComponent<Player>();
			playerLogic.score = playerLogic.score + enemyScore;

			//Random Ration Item Drop
			int ran = enemyName =="B" ? 0 : UnityEngine.Random.Range(0, 10);
			if (ran < 3)
				Debug.Log("Not Item");
			else if(ran < 6) //Coin
			{
				GameObject itemCoin = objectManager.MakeObj("itemCoin");
				itemCoin.transform.position = transform.position;
				//Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);
			}
			else if(ran < 8) //Power
			{
				GameObject itemPower = objectManager.MakeObj("itemPower");
				itemPower.transform.position = transform.position;
			}
			else if (ran < 10) //Boom
			{
				GameObject itemBoom = objectManager.MakeObj("itemBoom");
				itemBoom.transform.position = transform.position;
			}

			gameObject.SetActive(false);
			//Destroy(gameObject);
			transform.rotation = Quaternion.identity;
			//Quaternion.identity = 기본회전값을 0으로 설정
			gameManager.CallExplosion(transform.position, enemyName );

			//Boss Kill
			if (enemyName == "B")
				gameManager.StageEnd();

		}
	}

	void ReturnSprite()
	{
		spriteRenderer.sprite = sprites[0];
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "BorderBullet" && enemyName != "B")
		{
			gameObject.SetActive(false);
			transform.rotation = Quaternion.identity;
			//Destroy(gameObject);
		}
		else if(collision.gameObject.tag == "PlayerBullet")
		{
			Bullet bullet = collision.gameObject.GetComponent<Bullet>();
			OnHit(bullet.dmg);

			collision.gameObject.SetActive(false);
			//Destroy(collision.gameObject);
		}
	}

	void Fire()
	{
		if (curShotDelay < maxShotDelay) //장전시간
			return;
		if(enemyName == "S")
		{
			GameObject bullet = objectManager.MakeObj("bulletEnemyA");
			bullet.transform.position = transform.position;
			//Instantiate(bulletObjA, transform.position, transform.rotation);
			Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
			Vector3 dirVec = player.transform.position - transform.position;
			//목표물로 방향 = 목표물 위치 - 자신의 위치
			rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
		}
		else if(enemyName == "L")
		{
			GameObject bulletR = objectManager.MakeObj("bulletEnemyB");
			bulletR.transform.position = transform.position + Vector3.right * 0.3f;
			Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
			Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
			//목표물로 방향 = 목표물 위치 - 자신의 위치
			rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
			//normalized : 벡터가 단위값(1)로 변환된 변수

			GameObject bulletL = objectManager.MakeObj("bulletEnemyB");
			bulletL.transform.position = transform.position + Vector3.left * 0.3f;
			Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
			Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
			//목표물로 방향 = 목표물 위치 - 자신의 위치
			rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
			//normalized : 벡터가 단위값(1)로 변환된 변수
		}

		curShotDelay = 0; //총알은 쏜 다음에는 딜레이 변수 0으로 초기화
	}

	void Reload()
	{
		curShotDelay = curShotDelay + Time.deltaTime;
		//딜레이변수에 deltaTime을 계속 더하여 시간 계산
	}

}
