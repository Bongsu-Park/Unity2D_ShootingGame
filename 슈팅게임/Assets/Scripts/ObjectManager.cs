using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	//프리펩 변수 생성후, 할당
	public GameObject enemyBPrefab;
	public GameObject enemyLPrefab;
	public GameObject enemyMPrefab;
	public GameObject enemySPrefab;
	public GameObject itemCoinPrefab;
	public GameObject itemPowerPrefab;
	public GameObject itemBoomPrefab;
	public GameObject bulletPlayerAPrefab;
	public GameObject bulletPlayerBPrefab;
	public GameObject bulletEnemyAPrefab;
	public GameObject bulletEnemyBPrefab;
	public GameObject bulletFollowerPrefab;
	public GameObject bulletEnemyCPrefab;
	public GameObject bulletEnemyDPrefab;
	public GameObject explosionPrefab;


	GameObject[] enemyB;
	GameObject[] enemyL;
	GameObject[] enemyM;
	GameObject[] enemyS;

	GameObject[] itemCoin;
	GameObject[] itemPower;
	GameObject[] itemBoom;

	GameObject[] bulletPlayerA;
	GameObject[] bulletPlayerB;
	GameObject[] bulletEnemyA;
	GameObject[] bulletEnemyB;
	GameObject[] bulletEnemyC;
	GameObject[] bulletEnemyD;


	GameObject[] bulletFollower;

	GameObject[] targetPool;

	GameObject[] explosion;
	void Awake()
	{
		//한번에 등장할 개수를 고려하여 배열 길이 할당
		enemyB = new GameObject[1];
		enemyL = new GameObject[10];
		enemyM = new GameObject[10];
		enemyS = new GameObject[20];

		itemCoin = new GameObject[20];
		itemPower = new GameObject[10];
		itemBoom = new GameObject[10];

		bulletPlayerA = new GameObject[100];
		bulletPlayerB = new GameObject[100];
		bulletEnemyA = new GameObject[100];
		bulletEnemyB = new GameObject[100];
		bulletEnemyC = new GameObject[50];
		bulletEnemyD = new GameObject[1000];

		bulletFollower = new GameObject[100];

		explosion = new GameObject[20];

		Generate();
	}

	void Generate()
	{
		//Enemy
		for (int index = 0; index < enemyB.Length; index++)
		{
			enemyB[index] = Instantiate(enemyBPrefab);
			enemyB[index].SetActive(false);
		}
		for (int index = 0; index < enemyL.Length; index++)
		{
			enemyL[index] = Instantiate(enemyLPrefab);
			enemyL[index].SetActive(false);
		}
		for (int index = 0; index < enemyM.Length; index++)
		{
			enemyM[index] = Instantiate(enemyMPrefab);
			enemyM[index].SetActive(false);
		}
		for (int index = 0; index < enemyS.Length; index++)
		{
			enemyS[index] = Instantiate(enemySPrefab);
			enemyS[index].SetActive(false);
		}
		//Item
		for (int index = 0; index < itemCoin.Length; index++)
		{
			itemCoin[index] = Instantiate(itemCoinPrefab);
			itemCoin[index].SetActive(false);
		}

		for (int index = 0; index < itemPower.Length; index++)
		{
			itemPower[index] = Instantiate(itemPowerPrefab);
			itemPower[index].SetActive(false);
		}
		for (int index = 0; index < itemBoom.Length; index++)
		{
			itemBoom[index] = Instantiate(itemBoomPrefab);
			itemBoom[index].SetActive(false);
		}
		//Bullet
		for (int index = 0; index < bulletPlayerA.Length; index++)
		{
			bulletPlayerA[index] = Instantiate(bulletPlayerAPrefab);
			bulletPlayerA[index].SetActive(false);
		}
		for (int index = 0; index < bulletPlayerB.Length; index++)
		{
			bulletPlayerB[index] = Instantiate(bulletPlayerBPrefab);
			bulletPlayerB[index].SetActive(false);
		}
		for (int index = 0; index < bulletEnemyA.Length; index++)
		{
			bulletEnemyA[index] = Instantiate(bulletEnemyAPrefab);
			bulletEnemyA[index].SetActive(false);
		}
		for (int index = 0; index < bulletEnemyB.Length; index++)
		{
			bulletEnemyB[index] = Instantiate(bulletEnemyBPrefab);
			bulletEnemyB[index].SetActive(false);
		}
		for (int index = 0; index < bulletEnemyC.Length; index++)
		{
			bulletEnemyC[index] = Instantiate(bulletEnemyCPrefab);
			bulletEnemyC[index].SetActive(false);
		}
		for (int index = 0; index < bulletEnemyD.Length; index++)
		{
			bulletEnemyD[index] = Instantiate(bulletEnemyDPrefab);
			bulletEnemyD[index].SetActive(false);
		}

		for (int index = 0; index < bulletFollower.Length; index++)
		{
			bulletFollower[index] = Instantiate(bulletFollowerPrefab);
			bulletFollower[index].SetActive(false);
		}

		for (int index = 0; index < explosion.Length; index++)
		{
			explosion[index] = Instantiate(explosionPrefab);
			explosion[index].SetActive(false);
		}
	}

	//오브젝트 풀에 접근할 수 있는 함수 생성
	public GameObject MakeObj(string type)
	{
		switch(type)
		{
			case "EnemyB":
				targetPool = enemyB;
				break;
			case "EnemyL":
				targetPool = enemyL;
				break;
			case "EnemyM":
				targetPool = enemyM;
				break;
			case "EnemyS":
				targetPool = enemyS;
				break;
			case "itemCoin":
				targetPool = itemCoin;
				break;
			case "itemPower":
				targetPool = itemPower;
				break;
			case "itemBoom":
				targetPool = itemBoom;
				break;
			case "bulletPlayerA":
				targetPool = bulletPlayerA;
				break;
			case "bulletPlayerB":
				targetPool = bulletPlayerB;
				break;
			case "bulletEnemyA":
				targetPool = bulletEnemyA;
				break;
			case "bulletEnemyB":
				targetPool = bulletEnemyB;
				break;
			case "bulletEnemyC":
				targetPool = bulletEnemyC;
				break;
			case "bulletEnemyD":
				targetPool = bulletEnemyD;
				break;


			case "bulletFollower":
				targetPool = bulletFollower;
				break;

			case "Explosion":
				targetPool = explosion;
				break;

		}

		for (int index = 0; index < targetPool.Length; index++)
		{
			if (!targetPool[index].activeSelf)
			{
				targetPool[index].SetActive(true);
				return targetPool[index];
			}
		}

		return null;
	}

	public GameObject[] GetPool(string type)
	{
		switch (type)
		{
			case "EnemyB":
				targetPool = enemyB;
				break;
			case "EnemyL":
				targetPool = enemyL;
				break;
			case "EnemyM":
				targetPool = enemyM;
				break;
			case "EnemyS":
				targetPool = enemyS;
				break;
			case "itemCoin":
				targetPool = itemCoin;
				break;
			case "itemPower":
				targetPool = itemPower;
				break;
			case "itemBoom":
				targetPool = itemBoom;
				break;
			case "bulletPlayerA":
				targetPool = bulletPlayerA;
				break;
			case "bulletPlayerB":
				targetPool = bulletPlayerB;
				break;
			case "bulletEnemyA":
				targetPool = bulletEnemyA;
				break;
			case "bulletEnemyB":
				targetPool = bulletEnemyB;
				break;
			case "bulletEnemyC":
				targetPool = bulletEnemyC;
				break;
			case "bulletEnemyD":
				targetPool = bulletEnemyD;
				break;

			case "bulletFollower":
				targetPool = bulletFollower;
				break;

			case "Explosion":
				targetPool = explosion;
				break;
		}
		return targetPool;
	}

}
