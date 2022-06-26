using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO; //파일 읽기를 위한 헤더파일
using System.Runtime;

public class GameManager : MonoBehaviour 
{
	public int stage;
	public Animator stageAnim;
	public Animator clearAnim;
	public Animator fadeAnim;
	public Transform playerPos;

	public string[] enemyObjs;
	public Transform[] spawnPoints;

	public float nextSpawnDelay;
	public float curSpawnDelay;

	public GameObject player;

	public Text scoreText;
	public Image[] lifeImage;
	public GameObject gameOverSet;

	public Image[] boomImage;

	public ObjectManager objectManager;

	//적 출현에 관련된 변수 생성
	public List<Spawn> spawnList;//spawn에 대한 구조체가 담겨있는 List생성
	public int spawnIndex;
	public bool spawnEnd;


	void Awake()
	{
		spawnList = new List<Spawn>();
		enemyObjs = new string[] {"EnemyS", "EnemyM", "EnemyL", "EnemyB"};
		StageStart();
	}

	public void StageStart()
	{
		//Stage UI Load
		stageAnim.SetTrigger("On");
		stageAnim.GetComponent<Text>().text = "Stage " + stage + "\nStart";
		clearAnim.GetComponent<Text>().text = "Stage " + stage + "\nClear!";
		//Enemy Spawn File Read
		ReadSpawnFile();

		//Fade In
		fadeAnim.SetTrigger("In");
	}
	public void StageEnd()
	{
		//Clear UI Load
		clearAnim.SetTrigger("On");

		//Fade Out
		fadeAnim.SetTrigger("Out");

		//Player Repos
		player.transform.position = playerPos.position;

		//Stage Increament
		stage++;
		if (stage > 2)
			Invoke("GameOver", 6);
		else
			Invoke("StageStart", 6);

	}

	void ReadSpawnFile()
	{
		//변수 초기화
		spawnList.Clear(); //List내의 쓰레기변수들을 지워준다.
		spawnIndex = 0;
		spawnEnd = false;

		//리스폰 파일 읽기
		TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;
		//TextAsset : 텍스트 파일 에셋 클래스
		//Resource.Load() : Resource(Asset의 폴더이름)폴더 내 파일 불러오기
		//as : 앞에 불러온 파일이 뒷내용형식의 파일이아니면 null값이 나온다.

		StringReader stringReader = new StringReader(textFile.text);
		//StringReader : 파일 내의 문자열 데이터 읽기 클래스 -> system.io
		//textFile.text : textFile의 text를 불러오라는 뜻

		//while문으로 텍스트 데이터 끝에 다다를 때까지 반복
		while(stringReader != null)
		{
			string line = stringReader.ReadLine();
			//ReadLine : 텍스트 데이터를 한 줄씩 반환.
			Debug.Log(line);

			if (line == null)
				break;

			//리스폰 데이터 생성
			Spawn spawnData = new Spawn();
			spawnData.delay = float.Parse(line.Split(',')[0]);
			spawnData.type = line.Split(',')[1];
			spawnData.point = int.Parse(line.Split(',')[2]);
			//Split() : 지정한 구분 문자로 문자열을 나누는 함수
			//float.Parse() : float형으로 형변환을 시킨다.
			spawnList.Add(spawnData);
		}
		//텍스트 파일 닫기
		stringReader.Close();
		//stringReader로 열어둔 파일은 작업이 끝난 후 꼭 닫기

		//첫번째 스폰 딜레이 적용
		nextSpawnDelay = spawnList[0].delay;
		//미리 첫번째 출현 시간을 적용

	}

	void Update()
    {
		curSpawnDelay = curSpawnDelay + Time.deltaTime;

		if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
			SpawnEnemy();
			//nextSpawnDelay = UnityEngine.Random.Range(0.5f, 3f);
			//Random.Range(float, int)
			//Range()함수는 매개변수에 의해 반환 타입 결정
			curSpawnDelay = 0;
			//만약 curSpawnDelay을 초기화안할려면 텍스트파일에 delay를 누적해서 기록해야한다.
		}

		//UI Score Update
		Player playerLogic = player.GetComponent<Player>();
		scoreText.text = string.Format("{0:n0}",playerLogic.score);
		//string.format() : 지정된 양식으로 문자열을 변환해주는 함수\
		//{0:n0} : 세자리마다 쉼표로 나눠주는 숫자 양식
    }

	void SpawnEnemy()
    {
		int enemyIndex = 0;
		switch(spawnList[spawnIndex].type)
		{
			case "S":
				enemyIndex = 0;
				break;
			case "M":
				enemyIndex = 1;
				break;
			case "L":
				enemyIndex = 2;
				break;
			case "B":
				enemyIndex = 3;
				break;

		}
		//int randomEnemy = UnityEngine.Random.Range(0, 3);
		//int enemyPoint = UnityEngine.Random.Range(0, 9);
		int enemyPoint = spawnList[spawnIndex].point;

		GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
		enemy.transform.position = spawnPoints[enemyPoint].position;

		//Instantiate(enemyObjs[randomEnemy], spawnPoints[enemyPoint].position, spawnPoints[enemyPoint].rotation);

		Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
		Enemy enemyLogic = enemy.GetComponent<Enemy>();
		enemyLogic.player = player;
		enemyLogic.objectManager= objectManager;
		enemyLogic.gameManager = this;
		//this : 클래스 자신을 일컫는 키워드
		//프리펩은 아직 scene에 올라간게 아니기때문에 이미 올라온 오브젝트에 접근이 불가능하다
		//그래서 gamemanager에서 enemy가 생성된 직후에 플레이어변수를 넘겨주는것으로 해결한다.

		if (enemyPoint == 5 || enemyPoint == 6) //Right Spawn
		{
			enemy.transform.Rotate(Vector3.back * 90);
			rigid.velocity = new Vector2(enemyLogic.speed * (-1), (-1));
		}
		else if(enemyPoint == 7 || enemyPoint == 8) //Left Spawn
		{
			enemy.transform.Rotate(Vector3.forward * 90);
			rigid.velocity = new Vector2(enemyLogic.speed, (-1));
		}
		else //Front Spawn
		{
			rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
			                 //Vector2.down * enemyLogic.speed 둘다같다.
		}

		//리스폰 인덱스 증가
		spawnIndex++;
		if(spawnIndex == spawnList.Count)
		{
			spawnEnd = true;
			return;
		}
		//다음 리스폰 딜레이 갱신
		nextSpawnDelay = spawnList[spawnIndex].delay;
    }

	public void UpdateLifeIcon(int life)
	{
		//UI LIfe Init Disable
		for (int index = 0; index < 3; index++)
		{
			lifeImage[index].color = new Color(1, 1, 1, 0);
		}
		//UI LIfe Active
		for (int index = 0; index < life; index++)
		{
			lifeImage[index].color = new Color(1, 1, 1, 1);
		}
	}
	public void UpdateBoomIcon(int boom)
	{
		//UI Boom Init Disable
		for (int index = 0; index < 3; index++)
		{
			boomImage[index].color = new Color(1, 1, 1, 0);
		}
		//UI Boom Active
		for (int index = 0; index < boom; index++)
		{
			boomImage[index].color = new Color(1, 1, 1, 1);
		}
	}
	public void RespawnPlayer()
	{
		Invoke("RespawnPlayerExe", 2f);
	}
	void RespawnPlayerExe()
	{
		player.transform.position = Vector3.down * 3.5f;
		                        //new Vector3(0, -3.5f, 0) 같다.
		player.SetActive(true);

		Player playerLogic = player.GetComponent<Player>();
		playerLogic.isHit = false;
	}

	public void CallExplosion(Vector3 pos, string type)
	{
		GameObject explosion = objectManager.MakeObj("Explosion");
		Explosion explosionLogic = explosion.GetComponent<Explosion>();

		explosion.transform.position = pos;
		explosionLogic.StartExplosion(type);
	}

	public void GameOver()
	{
		gameOverSet.SetActive(true);
	}

	public void GameRetry()
	{
		SceneManager.LoadScene(0);
	}

}
