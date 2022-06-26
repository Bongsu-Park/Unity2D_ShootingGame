using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Follower : MonoBehaviour 
{
	public float maxShotDelay; //최대
	public float curShotDelay; //현재
	public ObjectManager objectManager;

	public Vector3 followPos;
	public int followDelay;
	public Transform parent;
	public Queue<Vector3> parentPos;

	void Awake()
	{
		parentPos = new Queue<Vector3>();
	}

	void Update()
	{
		Watch();
		Follw();
		Fire();
		Reload();
	}

	void Watch()
	{
		//Queue = FIFO (First Input First Out)
		//Input Position
		//부모위치가 가만히 있으면 저장하지 않도록 조건 추가
		if(!parentPos.Contains(parent.position))
			parentPos.Enqueue(parent.position);

		//Output Position
		//큐에 일정 데이터 갯수를 채워지면 그 때부터 반환하도록 작성
		if (parentPos.Count > followDelay)
			followPos = parentPos.Dequeue();
		else if (parentPos.Count < followDelay)
			//큐가 채워지기 전까진 부모 위치 적용
			followPos = parent.position;
	}

	void Follw()
	{
		transform.position = followPos;
	}

	void Fire()
	{
		if (!Input.GetButton("Fire1"))
			return;

		if (curShotDelay < maxShotDelay) //장전시간
			return;

		//Power One
		//Down Up은 그 찰나의 1프레임만 입력이되기때문에 Button으로 실행한다.
		GameObject bullet = objectManager.MakeObj("bulletFollower");
		bullet.transform.position = transform.position;
		//Instantiate(bulletObjA, transform.position, transform.rotation);
		Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
		rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

		curShotDelay = 0; //총알은 쏜 다음에는 딜레이 변수 0으로 초기화
	}

	void Reload()
	{
		curShotDelay = curShotDelay + Time.deltaTime;
		//딜레이변수에 deltaTime을 계속 더하여 시간 계산
	}

}
