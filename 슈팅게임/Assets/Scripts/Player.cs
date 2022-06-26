using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour 
{
	public float speed;
	public bool isTouchTop;
	public bool isTouchBottom;
	public bool isTouchLeft;
	public bool isTouchRight;
	Animator anim;

	public GameObject bulletObjA;
	public GameObject bulletObjB;
	public GameObject boomEffect;
	public float maxShotDelay; //최대
	public float curShotDelay; //현재
	public int power;
	public int boom;
	public int maxBoom;
	public bool isBoomTime;

	public GameManager gameManager;
	public ObjectManager objectManager;

	public int life;
	public int score;
	public bool isHit;//피격 중복을 방지하기 위한 bool변수 

	public int maxPower;

	SpriteRenderer spriteRenderer;

	public GameObject[] followers;

	public bool[] joyControl; //어디를 눌렀는지 판단
	public bool isControl;  //눌렀는지 판단

	public bool isBuutonA;
	public bool isButtonB;

	void Awake()
	{
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update () 
	{
		Move();
		Fire();
		Reload();
		Boom();
	}

	public void JoyPanel(int type)
	{
		for(int index = 0; index < 9; index++)
		{
			joyControl[index] = index == type;
		}
	}

	public void JoyDown()
	{
		isControl = true;
	}

	public void JoyUp()
	{
		isControl = false;
	}
	void Move()
	{
		//Keyboard Control Value
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		
		//Joy Control Value
		if (joyControl[0]) { h = -1; v = 1; }
		if (joyControl[1]) { h = 0; v = 1; }
		if (joyControl[2]) { h = 1; v = 1; }
		if (joyControl[3]) { h = -1; v = 0; }
		if (joyControl[4]) { h = 0; v = 0; }
		if (joyControl[5]) { h = 1; v = 0; }
		if (joyControl[6]) { h = -1; v = -1; }
		if (joyControl[7]) { h = 0; v = -1; }
		if (joyControl[8]) { h = 1; v = -1; }

		//방향 Down변수 조건을 추가하여 UI누른 상태에서만 작동하게한다.
		if ((h == 1 && isTouchRight) || (h == -1 && isTouchLeft) || !isControl)
			h = 0;
		
		if ((v == 1 && isTouchTop) || (v == -1 && isTouchBottom) || !isControl)
			v = 0;

		Vector3 curPos = transform.position;
		Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
		//transform이동에는 time.deltatime을 꼭 사용해야한다.

		transform.position = curPos + nextPos;

		if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
		{
			anim.SetInteger("Input", (int)h);
		}
	}

	public void ButtonADown()
	{
		isBuutonA = true;
	}
	public void ButtonAUp()
	{
		isBuutonA = false;
	}
	//B는 누르는 순간 터지기때문에 up이 따로 필요가 없다.
	public void ButtonBDown()
	{
		isButtonB = true;
	}
	

	void Fire()
	{
		//if (!Input.GetButton("Fire1"))
		//	return;
		if (!isBuutonA)
			return;

		if (curShotDelay < maxShotDelay) //장전시간
			return;

		switch(power)
		{
			case 1:
				//Power One
				//Down Up은 그 찰나의 1프레임만 입력이되기때문에 Button으로 실행한다.
				GameObject bullet = objectManager.MakeObj("bulletPlayerA");
				bullet.transform.position = transform.position;
				//Instantiate(bulletObjA, transform.position, transform.rotation);
				Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
				rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				break;
			case 2:
				GameObject bulletR = objectManager.MakeObj("bulletPlayerA");
				bulletR.transform.position = transform.position + Vector3.right * 0.1f;
				
				GameObject bulletL = objectManager.MakeObj("bulletPlayerA");
				bulletL.transform.position = transform.position + Vector3.left * 0.1f;
				

				Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
				rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
				rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				break;
			default://위 모든경우가 아닌경우 default로 넘어온다
				GameObject bulletRR = objectManager.MakeObj("bulletPlayerA");
				bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
				
				GameObject bulletCC = objectManager.MakeObj("bulletPlayerB");
				bulletCC.transform.position = transform.position;
				
				GameObject bulletLL = objectManager.MakeObj("bulletPlayerA");
				bulletLL.transform.position = transform.position + Vector3.left * 0.35f;

				Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
				rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
				rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
				rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				break;
		}
		
		curShotDelay = 0; //총알은 쏜 다음에는 딜레이 변수 0으로 초기화
	}

	void Reload()
	{
		curShotDelay = curShotDelay + Time.deltaTime;
		//딜레이변수에 deltaTime을 계속 더하여 시간 계산
	}

	void Boom()
	{
		//if (!Input.GetButton("Fire2"))
		//	return;
		if (!isButtonB)
			return;

		if (isBoomTime)
			return;

		if (boom == 0)
			return;

		boom--;
		isBoomTime = true;
		gameManager.UpdateBoomIcon(boom);

		//Effect visible
		boomEffect.SetActive(true);
		Invoke("OffBoomEffect", 4f);

		//Remove Enemy
		GameObject[] enemyL = objectManager.GetPool("EnemyL");
		GameObject[] enemyM = objectManager.GetPool("EnemyM");
		GameObject[] enemyS = objectManager.GetPool("EnemyS");
		//GameObject.FindGameObjectsWithTag("Enemy");
		//FindGameObjectsWithTag : 태그로 장면의 모든 오브젝트를 추출
		for (int index = 0; index < enemyL.Length; index++)
		{
			if (enemyL[index].activeSelf)
			{
				Enemy enemyLogic = enemyL[index].GetComponent<Enemy>();
				enemyLogic.OnHit(1000);
			}
		}
		for (int index = 0; index < enemyM.Length; index++)
		{
			if (enemyM[index].activeSelf)
			{
				Enemy enemyLogic = enemyM[index].GetComponent<Enemy>();
				enemyLogic.OnHit(1000);
			}
		}
		for (int index = 0; index < enemyS.Length; index++)
		{
			if (enemyS[index].activeSelf)
			{
				Enemy enemyLogic = enemyS[index].GetComponent<Enemy>();
				enemyLogic.OnHit(1000);
			}
		}

		//Remove Enemy Bullet
		GameObject[] bulletsA = objectManager.GetPool("bulletEnemyA");
		GameObject[] bulletsB = objectManager.GetPool("bulletEnemyB");
		//GameObject.FindGameObjectsWithTag("Enemy");
		//FindGameObjectsWithTag : 태그로 장면의 모든 오브젝트를 추출
		for (int index = 0; index < bulletsA.Length; index++)
		{
			if(bulletsA[index].activeSelf)
			   bulletsA[index].SetActive(false);
			//Destroy(bullets[index]);
		}
		for (int index = 0; index < bulletsB.Length; index++)
		{
			if (bulletsB[index].activeSelf)
				bulletsB[index].SetActive(false);
		}
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Border")
		{
			switch(collision.gameObject.name)
			{
				case "Top":
					isTouchTop = true;
					break;
				case "Bottom":
					isTouchBottom = true;
					break;
				case "Right":
					isTouchRight = true;
					break;
				case "Left":
					isTouchLeft = true;
					break;

			}
		}
		else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
		{
			if (isHit)
				return;

			isHit = true;
			life--;
			gameManager.UpdateLifeIcon(life);
			gameManager.CallExplosion(transform.position, "P");

			if(life == 0)
			{
				gameManager.GameOver();
			}
			else
			{
				gameManager.RespawnPlayer();

				OnRespawn();
			}
			gameObject.SetActive(false);
			//false시켰으므로 여기서 다시 true로 변환하기는 힘드므로 gamegameManger에서 관리한다.
			collision.gameObject.SetActive(false);
			//Destroy(collision.gameObject);
		}
		else if(collision.gameObject.tag == "Item")
		{
			Item item = collision.gameObject.GetComponent<Item>();
			switch (item.type)
			{
				case "Coin":
					score = score + 1000;
					break;

				case "Power":
					if (power == maxPower)
						score = score + 500;
					else
					{
						power++;
						AddFollower();
					}
					break;

				case "Boom":
					if (boom == maxBoom)
						score = score + 500;
					else
					{
						boom++;
						gameManager.UpdateBoomIcon(boom);
					}
					break;
			}
			collision.gameObject.SetActive(false);
			//Destroy(collision.gameObject);
		}
	}
	void AddFollower()
	{
		if (power == 4)
			followers[0].SetActive(true);
		else if(power == 5)
			followers[1].SetActive(true);
		else if (power == 6)
			followers[2].SetActive(true);
	}


	void OffBoomEffect()
	{
		boomEffect.SetActive(false);
		isBoomTime = false;
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Border")
		{
			switch (collision.gameObject.name)
			{
				case "Top":
					isTouchTop = false;
					break;
				case "Bottom":
					isTouchBottom = false;
					break;
				case "Right":
					isTouchRight = false;
					break;
				case "Left":
					isTouchLeft = false;
					break;

			}
		}
	}

	void OnRespawn()
	{
		gameObject.layer = 9;

		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		//followers 무적타임
		for (int index = 0; index < followers.Length; index++)
			followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);

		Invoke("OffRespawn", 4f);
	}

	void OffRespawn()
	{
		gameObject.layer = 8;

		spriteRenderer.color = new Color(1, 1, 1, 1);

		//followers 무적타임
		for (int index = 0; index < followers.Length; index++)
			followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
	}

}
